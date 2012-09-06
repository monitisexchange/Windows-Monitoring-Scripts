#ifndef MNFUNCTIONINFO_H
#define MNFUNCTIONINFO_H

#include <string>
#include <list>
#include <map>
#include "MnConfigXml.h"

using namespace std;

namespace Monitis
{
    bool CheckMethodName(wstring MethodName);

	class MnFunctionInfo
	{
	public:
        static HRESULT ParseFunctionParameters(IMetaDataImport* metadata, const mdToken& funcToken, wstring& parameters);

	public:
		MnFunctionInfo(FunctionID functionID,  const wstring& assemblyName, const wstring& className, const wstring& methodName, 
            const wstring& methodParameters, const wstring& unitProfilesName);

		wstring& GetAssemblyName();
		wstring& GetClassName();
		wstring& GetFunctionName();
        wstring& GetParametrs();
        wstring& GetUnitProfiles();
		FunctionID GetFunctionID();
		long GetCallCount();
		void Enter();
		ULONGLONG Leave();
		ULONGLONG Tailcall();

	private:
		FunctionID m_functionID;
		wstring m_AssemblyName;
		wstring m_ClassName;
		wstring m_MethodName;
        wstring m_Parametrs;
        wstring m_UnitProfileString;
		long m_callCount;
		std::list<SYSTEMTIME> m_Enters;
		ULONGLONG m_Duration;
	};

	typedef std::map<FunctionID, tr1::shared_ptr<MnFunctionInfo> > MnFunctionMap;
}
#endif
