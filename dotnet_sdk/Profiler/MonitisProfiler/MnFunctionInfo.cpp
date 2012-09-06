#include "StdAfx.h"

namespace Monitis
{
    const wchar_t kForbiddenSymbols[] = L"\"&<>'";
    
    bool CheckMethodName(wstring methodName)
    {
        for (int i = 0; i < (sizeof(kForbiddenSymbols)/sizeof(wchar_t) - 1); i++)
        {
            if ( methodName.find(kForbiddenSymbols[i]) != wstring::npos)
            {
                //logs::Logger::OutputInfo(_T("%s: Method name %s is fail"), _T(__FUNCTION__), methodName.c_str());
                return false;
            }
        }
        return true;
    }

    void GetArrayTypeString(PCCOR_SIGNATURE& sigBlob, wstring& typeName)
    {
        COR_SIGNATURE signature = *sigBlob++;
        ULONG rank = CorSigUncompressData(sigBlob);													
        if ( rank == 0 ) 
        {
            typeName.append(L"[?]");
            return;
        }

        ULONG arraysize = (sizeof(ULONG) * 2 * rank);
        ULONG *lower = (ULONG *)_alloca(arraysize);
        memset(lower, 0, arraysize); 
        ULONG *sizes = &lower[rank];
        ULONG numsizes = CorSigUncompressData(sigBlob);
        for (ULONG i = 0; i < numsizes && i < rank; i++)
        {
            sizes[i] = CorSigUncompressData(sigBlob);	
        }

        ULONG numlower = CorSigUncompressData(sigBlob);
        for (ULONG i = 0; i < numlower && i < rank; i++)
        {
            lower[i] = CorSigUncompressData(sigBlob); 
        }

        typeName.append(L"[");
        for (ULONG i = 0; i < rank; ++i)
        {					
            if (i > 0) 
            {
                typeName.append(L",");
            }
            
            if (lower[i] == 0)
            {
                if (sizes[i] != 0)
                {
                    WCHAR *size = new WCHAR[1024];
                    size[0] = '\0';
                    wsprintf(size, L"%d", sizes[i]);
                    typeName.append(size);
                }                
            }
            else
            {
                WCHAR *low = new WCHAR[1024];
                low[0] = '\0';
                wsprintf(low, L"%d", lower[i]);
                typeName.append(low);
                typeName.append(L"...");
                if (sizes[i] != 0)
                {
                    WCHAR *size = new WCHAR[1024];
                    size[0] = '\0';
                    wsprintf(size, L"%d", (lower[i] + sizes[i] + 1));
                    typeName.append(size);                    
                }
            }
        }
        typeName.append( L"]");
        return;
    }

    bool GetTypeString(CorElementType elementType, wstring& typeName)
    {
        switch (elementType)
        {
            case ELEMENT_TYPE_VOID:
                typeName.append(L"void");
                break;
            
            case ELEMENT_TYPE_BOOLEAN:
                typeName.append(L"System.Boolean");
                break;
            case ELEMENT_TYPE_CHAR:
                typeName.append(L"System.Char");
                break;

            case ELEMENT_TYPE_I1:
                typeName.append(L"System.SByte");
                break;

            case ELEMENT_TYPE_U1:
                typeName.append(L"System.Byte");
                break;

            case ELEMENT_TYPE_I2:
                typeName.append(L"System.Int16");
                break;

            case ELEMENT_TYPE_U2:
                typeName.append(L"System.UInt16");
                break;

            case ELEMENT_TYPE_I4:
                typeName.append(L"System.Int32");
                break;

            case ELEMENT_TYPE_U4:
                typeName.append(L"System.UInt32");
                break;

            case ELEMENT_TYPE_I8:
                typeName.append(L"System.Int64");
                break;

            case ELEMENT_TYPE_U8:
                typeName.append(L"System.UInt64");
                break;

            case ELEMENT_TYPE_R4:
                typeName.append(L"System.Single");
                break;

            case ELEMENT_TYPE_R8:
                typeName.append(L"System.Double");
                break;

            case ELEMENT_TYPE_STRING:
                typeName.append(L"System.String");
                break;

            case ELEMENT_TYPE_VAR:
                typeName.append(L"class variable");
                break;

            case ELEMENT_TYPE_MVAR:
                typeName.append(L"method variable");
                break;

            case ELEMENT_TYPE_TYPEDBYREF:
                typeName.append(L"refany");
                break;

            case ELEMENT_TYPE_I:
                typeName.append(L"native integer");
                break;

            case ELEMENT_TYPE_U:
                typeName.append(L"native unsigned integer");
                break;

            case ELEMENT_TYPE_OBJECT:
                typeName.append(L"Object");
                break;

            default:
                //DebugBreak();
                return false;

        }
        return true;

    }

    void GetType(IMetaDataImport* metadata, PCCOR_SIGNATURE& sigBlob, wstring& typeParametr)
    {
        CorElementType coreType = (CorElementType) *sigBlob++;
        if (GetTypeString(coreType, typeParametr))
            return;
        switch (coreType)
        {
            case ELEMENT_TYPE_PTR:
                GetType(metadata, sigBlob,  typeParametr);
                typeParametr.append(L"*");
                break;

            case ELEMENT_TYPE_BYREF:
                GetType(metadata, sigBlob,  typeParametr);
                typeParametr.append(L"&");
                break;

            case ELEMENT_TYPE_ARRAY:
                GetType(metadata, sigBlob,  typeParametr);
                GetArrayTypeString(sigBlob, typeParametr);
                break;

            case ELEMENT_TYPE_SZARRAY:
                GetType(metadata, sigBlob,  typeParametr);
                typeParametr.append(L"[]");
                break;

            case ELEMENT_TYPE_PINNED:
                GetType(metadata, sigBlob,  typeParametr);
                typeParametr.append(L"pinned");
                break;

            case ELEMENT_TYPE_GENERICINST:
            {
                GetType(metadata, sigBlob,  typeParametr);
                typeParametr.append(L"<");
                ULONG arguments = CorSigUncompressData(sigBlob);
                for (ULONG i = 0; i < arguments; ++i)
                {
                    
                    if (i != 0)
                    {
                        typeParametr.append(L",");
                    }
                    GetType(metadata, sigBlob,  typeParametr);
                }
                typeParametr.append(L">");
                break;
            }
            
            case ELEMENT_TYPE_VALUETYPE:
            case ELEMENT_TYPE_CLASS:
            case ELEMENT_TYPE_CMOD_REQD:
            case ELEMENT_TYPE_CMOD_OPT:
            {
                HRESULT hr;
                mdTypeDef typeDef;
                WCHAR parameterName[1024];
                sigBlob += CorSigUncompressToken(sigBlob, &typeDef);
                if ( TypeFromToken( typeDef ) == mdtTypeRef )
                {
                    hr = metadata->GetTypeRefProps(typeDef, NULL, parameterName, sizeof(parameterName)/sizeof(WCHAR), NULL);
                }
                else
                {
                    hr = metadata->GetTypeDefProps(typeDef, parameterName, sizeof(parameterName)/sizeof(WCHAR), NULL, NULL, NULL );
                }

                if (SUCCEEDED(hr))
                {
                    typeParametr = wstring(parameterName);
                }
                break;
            }
            default:
                break;
        }
        return;
    }

    CorCallingConvention ReadCorCallingConvention(PCCOR_SIGNATURE& methodSignatureBlob)
    {
        CorCallingConvention callingConvention;
        methodSignatureBlob += CorSigUncompressData(methodSignatureBlob, (ULONG*) &callingConvention);
        //logs::Logger::OutputInfo(_T("%s: CorCallingConvention %u"), _T(__FUNCTION__), (ULONG)callingConvention);
        return callingConvention;
    }

    ULONG ReadArgumentCount(PCCOR_SIGNATURE& methodSignatureBlob)
    {
        ULONG argumentCount;
        methodSignatureBlob += CorSigUncompressData(methodSignatureBlob, &argumentCount);
        //logs::Logger::OutputInfo(_T("%s: Argument Count %u"), _T(__FUNCTION__), argumentCount);
        return argumentCount;
    }

    void ReadReturnType(IMetaDataImport* metadata, PCCOR_SIGNATURE& methodSignatureBlob)
    {
        wstring typeName;
        GetType(metadata, methodSignatureBlob, typeName);
        //logs::Logger::OutputInfo(_T("%s: Return type name %s"), _T(__FUNCTION__), typeName.c_str());
        return;
    }


    HRESULT  MnFunctionInfo::ParseFunctionParameters(IMetaDataImport* metadata, const mdToken& funcToken, wstring& parameters)
    {
        PCCOR_SIGNATURE methodSignatureBlob;
        ULONG methodSignatureBlobSize;
        //DebugBreak();
        HRESULT hr = metadata->GetMethodProps(funcToken, 0, 0, 0, 0, 0, &methodSignatureBlob, &methodSignatureBlobSize, 0, 0);
        if (SUCCEEDED(hr))
        {
            CorCallingConvention callingConvention = ReadCorCallingConvention(methodSignatureBlob);
            ULONG argumentCount = ReadArgumentCount(methodSignatureBlob);
            ReadReturnType(metadata, methodSignatureBlob);

            HCORENUM argumentsEnum = NULL;
            mdParamDef* paramsTokens = new mdParamDef[argumentCount];
            hr = metadata->EnumParams(&argumentsEnum, funcToken, paramsTokens, argumentCount, NULL);
            if (SUCCEEDED(hr))
            {
                parameters.clear();
                for (ULONG i = 0; (methodSignatureBlob != NULL) && (i < argumentCount); i++)
                {
                    if (parameters.length() > 0)
                    {
                        parameters.append(L";");
                    }
                    GetType(metadata, methodSignatureBlob, parameters);                   
                }
                if (parameters.length() == 0)
                {
                    parameters = L"void";
                }
                //logs::Logger::OutputInfo(_T("%s: Signature %s"), _T(__FUNCTION__), parameters.c_str());
            }
            delete [] paramsTokens;
        }
		return hr;
	}

	MnFunctionInfo::MnFunctionInfo(FunctionID functionID,  const wstring& assemblyName, const wstring& className, const wstring& methodName, 
        const wstring& methodParameters, const wstring& unitProfilesName)
	{
		m_functionID = functionID;
		m_AssemblyName = assemblyName;
		m_ClassName = className;
		m_MethodName = methodName;
        m_Parametrs = methodParameters;
        m_UnitProfileString = unitProfilesName;
		m_callCount = 0;
		m_Duration = 0;
	}

	wstring& MnFunctionInfo::GetClassName()
	{
		return m_ClassName;
	}

	wstring& MnFunctionInfo::GetFunctionName()
	{
		return m_MethodName;
	}

	FunctionID MnFunctionInfo::GetFunctionID()
	{
		return m_functionID;
	}

	long MnFunctionInfo::GetCallCount()
	{
		return m_callCount;
	}

	void MnFunctionInfo::Enter()
	{
		m_callCount++;
		SYSTEMTIME st;
		GetLocalTime(&st);
		m_Enters.push_back(st);
	}

	ULONGLONG SystemTimeToInterval(SYSTEMTIME systemTime)
	{
		FILETIME ft;
		SystemTimeToFileTime(&systemTime, &ft);
		ULARGE_INTEGER large;
		large.LowPart = ft.dwLowDateTime;
		large.HighPart = ft.dwHighDateTime;
		return large.QuadPart;
	}

	ULONGLONG MnFunctionInfo::Leave()
	{
		std::list<SYSTEMTIME>::reverse_iterator item = m_Enters.rbegin();
		if (item == m_Enters.rend())
			return 0;

		SYSTEMTIME st;
		GetLocalTime(&st);

		ULONGLONG startTime = SystemTimeToInterval((*item));
		ULONGLONG endTime = SystemTimeToInterval(st);
		ULONGLONG spend = endTime - startTime;
		m_Duration += spend;
		m_Enters.pop_front();
		return spend/1250;
	}

	ULONGLONG MnFunctionInfo::Tailcall()
	{
		return Leave();
	}

	wstring& MnFunctionInfo::GetAssemblyName()
	{
		return m_AssemblyName;
	}

    wstring& MnFunctionInfo::GetParametrs()
    {
        return m_Parametrs;
    }

    wstring& MnFunctionInfo::GetUnitProfiles()
    {
        return m_UnitProfileString;
    }
    
}