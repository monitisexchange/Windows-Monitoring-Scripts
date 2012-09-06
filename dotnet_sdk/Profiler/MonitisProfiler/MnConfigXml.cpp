#include "stdafx.h"

#include <algorithm>
#include <sstream>

namespace Monitis
{
	const WCHAR kRegistryMonitis[] = L"SOFTWARE\\Monitis";
	const WCHAR kInstalledPath[] = L"ExtensionsPath";
	const WCHAR kXmlFileMask[] = L"*.xml";
	//const for XML 
	const WCHAR kUnitProfilerSelect[] = L"extensionMonitisProfiler/configuration/unitProfile";
	const WCHAR kAttributeAssemblyName[] = L"assemblyName";
	const WCHAR kAttributeClassName[] = L"className";
	const WCHAR kAttributeMethodName[] = L"methodName";
	const WCHAR kAttributeParametersName[] = L"parameters";

    MnUnitProfile::MnUnitProfile(const wstring& unitProfileName)
    {
        m_unitProfileName = unitProfileName;
    }

    /*MnSignature::MnSignature(const wstring& signature)
    {
        m_text = signature;
    }

    void MnSignature::AddUnitProfile(MnUnitProfileShrPtr& unitProfile)
    {
        if (unitProfile != NULL)
        {
            m_UnitProfiles.push_back(unitProfile);
        }
    }

    

    bool MnSignature::operator ==(const MnSignature& b) const
    {
        return (strcmp(GetText().c_str(), b.GetText().c_str()) == 0);
    }
*/

	MnMethod::MnMethod(const wstring& methodName)
	{
		m_methodName = methodName;
	}

    void MnMethod::AddSignature(const wstring& parameters, MnUnitProfile* unitProfile)
    {
        MnSignatureMap::iterator item = m_signatureMap.find(parameters);
        if (item != m_signatureMap.end())
        {
            if (find(item->second.begin(), item->second.end(), unitProfile) == item->second.end())
            {
                item->second.push_back(unitProfile);
            }
        }
        else
        {
            list<MnUnitProfile *> listUnitProfile;
            listUnitProfile.push_back(unitProfile);
            m_signatureMap[parameters] = listUnitProfile;
        }
    }

    wstring MnMethod::GetUnitProfileString(const wstring& parameters)
    {
        wstring str(L"");
        MnSignatureMap::const_iterator signatureItem = m_signatureMap.find(parameters);
        if (signatureItem != m_signatureMap.end())
        {
            for (list<MnUnitProfile*>::const_iterator itemUnit = signatureItem->second.begin(); itemUnit != signatureItem->second.end(); itemUnit++)
            {
                if (itemUnit != signatureItem->second.begin())
                {
                    str += L",";
                }
                str += (*itemUnit)->GetNameUnitProfile().c_str();
            }
        }
        return str;
    }

    void MergeUnitProfiles(const list<MnUnitProfile*>& sourceUnitProfileList, list<MnUnitProfile*>& destUnitProfileList)
    {
        for (list<MnUnitProfile*>::const_iterator itemUnit = sourceUnitProfileList.begin(); itemUnit != sourceUnitProfileList.end(); itemUnit++)
        {
            if (find(destUnitProfileList.begin(), destUnitProfileList.end(), *itemUnit) == destUnitProfileList.end())
            {
                destUnitProfileList.push_back(*itemUnit);
            }
        }

    }

    void MnMethod::SelectUnitProfiles(const wstring& parameters, list<MnUnitProfile*>& unitProfileList)
    {

        MnSignatureMap::const_iterator signatureItem = m_signatureMap.find(parameters);
        if (signatureItem != m_signatureMap.end())
        {
            MergeUnitProfiles(signatureItem->second, unitProfileList);
        }
        if (parameters.length() > 0)
        {
            signatureItem = m_signatureMap.find(L"");
            if (signatureItem != m_signatureMap.end())
            {
                MergeUnitProfiles(signatureItem->second, unitProfileList);
            }
        }
    }

    MnClass::MnClass(const wstring& className, const wstring& assemblyName)
    {
        m_className = className;
        wstringstream wss;
        wss << assemblyName << L":" << assemblyName;
        m_FullName = wss.str();
    }

	MnMethod* MnClass::GetMnMethod(const wstring& methodName)
	{
		MnMethodsMap::iterator method = m_methods.find(methodName.c_str());
		return (method != m_methods.end() ? method->second.get() : NULL);
	}

    MnMethod* MnClass::CreateMnMethod(const wstring& methodName, const wstring& params, MnUnitProfile* unitProfile)
    {
		MnMethod* method = GetMnMethod(methodName);
		if (method == NULL)
		{
			method = new MnMethod(methodName);
 			m_methods[methodName] = tr1::shared_ptr<MnMethod>(method);
		}
        method->AddSignature(params, unitProfile);
		return method;
	}

	MnAssembly::MnAssembly(const wstring& assemblyName)
	{
		m_assemblyName = assemblyName;
	}

	MnClass* MnAssembly::GetMnClass(const wstring& className)
	{
		MnClassesMap::iterator classItem = m_classes.find(className.c_str());
		return (classItem != m_classes.end() ? classItem->second.get() : NULL);
	}

	MnClass* MnAssembly::CreateMnClass(const wstring& className)
	{
		MnClass* classInstance = GetMnClass(className);
		if (classInstance == NULL)
		{
			classInstance = new MnClass(className, m_assemblyName);
			m_classes[className] = tr1::shared_ptr<MnClass>(classInstance);
		}
		return classInstance;
	}

	wstring MnConfigXml::GetConfigPath()
	{
		HKEY key;
		wstring installPath(L"");
		if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, kRegistryMonitis, 0, KEY_READ, &key) == ERROR_SUCCESS)
		{
			WCHAR monitisPath[_MAX_PATH];
			DWORD sz = _MAX_PATH;
			if (RegQueryValueEx(key, kInstalledPath, NULL, NULL, (LPBYTE)monitisPath, &sz) == ERROR_SUCCESS)
			{
				installPath = wstring(monitisPath);
			}
			else
			{
				logs::Logger::OutputInfo(_T("%s: Read registry: %s"), _T(__FUNCTION__), logs::GetErrString(GetLastError()).c_str());
			}
			RegCloseKey(key);
		}
		else
		{
			logs::Logger::OutputInfo(_T("%s: Open registry %s Error %s"), _T(__FUNCTION__), kRegistryMonitis, logs::GetErrString(GetLastError()).c_str());
		}
		logs::Logger::OutputInfo(_T("%s: Monitis path: %s"), _T(__FUNCTION__), installPath.c_str());
		return installPath;
	}

	void MnConfigXml::FillFileList(wstring configPath, MnFileList& fileList)
	{
		logs::Logger::OutputInfo(_T("%s: %s"), _T(__FUNCTION__), configPath.c_str());
		if (configPath.length() == 0)
		{
			return;
		}

		WIN32_FIND_DATA findFileData;
		wstring mask = configPath + L"\\" + kXmlFileMask;
		wstring itemName;
		fileList.clear();
		HANDLE findHandle = FindFirstFile(mask.c_str(), &findFileData);
		if (findHandle == INVALID_HANDLE_VALUE) 
		{
			logs::LogWinErrCodeDebugString(::GetLastError());
			return;
		}

		do
		{
			itemName = configPath + _T("\\") + wstring(findFileData.cFileName);
			logs::Logger::OutputInfo(_T("%s: %s"), _T(__FUNCTION__), itemName.c_str());
			if ((findFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != FILE_ATTRIBUTE_DIRECTORY)
			{
				fileList.push_back(itemName);
			}
		}
		while (FindNextFile(findHandle, &findFileData) != 0);
		::FindClose(findHandle);
	}

	void MnConfigXml::LoadConfigurationXmls()
	{
		HRESULT hr = CoInitialize(NULL); 
		if (FAILED(hr))
		{
			logs::Logger::OutputInfo(_T("%s: Error CoInitialize"), _T(__FUNCTION__));
			return;
		}

		MnFileList xmlFiles;
		FillFileList(GetConfigPath(), xmlFiles);

		for (MnFileList::iterator xmlItem = xmlFiles.begin(); xmlItem != xmlFiles.end(); xmlItem++)
		{
            try 
            {
                LoadXmlFile(*xmlItem);
            }
			catch (...)
			{
				//logs::Logger::OutputInfo(_T("%s Error load xml file % s:\"%s\""), _T(__FUNCTION__), (*xmlItem).c_str(), );
			}
		}

		for (MnAssembliesMap::iterator assemblyItem = m_assembliesMap.begin(); assemblyItem != m_assembliesMap.end(); assemblyItem++)
		{
			logs::Logger::OutputInfo(_T("Assembly %s"), assemblyItem->first.c_str());
			for (MnClassesMap::iterator classItem = assemblyItem->second->GetMnClasses().begin(); classItem != assemblyItem->second->GetMnClasses().end(); classItem++)
            {
                logs::Logger::OutputInfo(_T("\tClasses \"%s\""), classItem->first.c_str());
                for (MnMethodsMap::iterator methodItem = classItem->second->GetMnMethods().begin(); methodItem != classItem->second->GetMnMethods().end(); methodItem++)
                {
					logs::Logger::OutputInfo(_T("\t\tMethod %s "), methodItem->first.c_str());
#ifdef DEBUG
                    MnSignatureMap signatures = methodItem->second->GetSignatureMap();
                    for (MnSignatureMap::const_iterator signatureItem = signatures.begin(); signatureItem != signatures.end(); signatureItem++ )
                    {
                        logs::Logger::OutputInfo(_T("\t\t\tSignature %s UnitProfiles %s"), signatureItem->first.c_str(),  methodItem->second->GetUnitProfileString(signatureItem->first).c_str());
                    }
#endif
				}
			}
			/*for (std::list<std::wstring>::iterator mitem = item->second.begin(); mitem != item->second.end(); mitem++)
				logs::Logger::OutputInfo(_T("\t\t%s"), (*mitem).c_str());*/
		}
	}

	wstring GetAttributeText(IXMLDOMNamedNodeMap *attributeMap, std::wstring attributeName)
	{
		CComPtr<IXMLDOMNode> attribute;
		HRESULT hr = attributeMap->getNamedItem(CComBSTR(attributeName.c_str()), &attribute);
		if (SUCCEEDED(hr))
		{
			CComBSTR nodeText;
			hr = attribute->get_text(&nodeText);
			if (SUCCEEDED(hr))
				return std::wstring(nodeText);
		}
		return std::wstring(L"");
	}

    wstring GetNodeName(IXMLDOMNode *node)
    {
        CComPtr<IXMLDOMNamedNodeMap> attributes;
        if (SUCCEEDED(node->get_attributes(&attributes)))
        {
            return GetAttributeText(attributes, L"name");
        }
        return wstring(L"");
    }

	MnAssembly* MnConfigXml::GetMnAssembly(wstring assemblyName)
	{
		MnAssembliesMap::iterator assembly = m_assembliesMap.find(assemblyName.c_str());
		return (assembly != m_assembliesMap.end() ? assembly->second.get() : NULL);
	}

	MnAssembly* MnConfigXml::CreateMnAssembly(wstring assemblyName)
	{
		MnAssembly* assembly = GetMnAssembly(assemblyName);
		if (assembly == NULL)
		{
			assembly = new MnAssembly(assemblyName);
			m_assembliesMap[assemblyName] = tr1::shared_ptr<MnAssembly>(assembly);
		}
		return assembly;
	}

	void MnConfigXml::ParseUnitProfiler(IXMLDOMNode *upNode)
	{
		if (upNode == NULL)
			return;
		CComPtr<IXMLDOMNodeList> nodesCoincidence;
        tr1::shared_ptr<MnUnitProfile> unitProfile(new MnUnitProfile(GetNodeName(upNode)));
        m_UnitProfiles.push_back(unitProfile);
		long sizeCoincidences = 0;
		XmlHelper::GetChildNodes(upNode, &nodesCoincidence, sizeCoincidences);
		for (long i = 0; i < sizeCoincidences; i++)
		{
			CComPtr<IXMLDOMNode> coincidenceNode;
			HRESULT hr = nodesCoincidence->get_item(i, &coincidenceNode);
			if (SUCCEEDED(hr))
			{
				CComPtr<IXMLDOMNamedNodeMap> attributes;
				MnClass* classItem;
				hr = coincidenceNode->get_attributes(&attributes);
				if (SUCCEEDED(hr))
				{
					MnAssembly* assembly = CreateMnAssembly(GetAttributeText(attributes, std::wstring(kAttributeAssemblyName)));
					if (assembly != NULL)
					{
						classItem = assembly->CreateMnClass(GetAttributeText(attributes, std::wstring(kAttributeClassName)));
					}
					else
					{
						// TODO try exception
					}
				}

				CComPtr<IXMLDOMNodeList> methodList;
				long numberMethods = 0;
				XmlHelper::GetChildNodes(coincidenceNode, &methodList, numberMethods);
				for (long j = 0; j < numberMethods; j++)
				{
					CComPtr<IXMLDOMNode> methodNode;
					hr = methodList->get_item(j, &methodNode);
					if (SUCCEEDED(hr) && methodNode.p != NULL)
					{
						CComPtr<IXMLDOMNamedNodeMap> methodAttributes;
						hr = methodNode->get_attributes(&methodAttributes);
						if (SUCCEEDED(hr))
						{
                            classItem->CreateMnMethod(GetAttributeText(methodAttributes, kAttributeMethodName),
                                GetAttributeText(methodAttributes, kAttributeParametersName), unitProfile.get());
						}
					}
				}
			}
		}
	}

	void MnConfigXml::LoadXmlFile(wstring fileName)
	{
		CComPtr<IXMLDOMDocument> xmlDom;
		HRESULT hr = xmlDom.CoCreateInstance(__uuidof(DOMDocument));
		if (FAILED(hr) || xmlDom.p == NULL)
			return;
		//DebugBreak();
		VARIANT_BOOL bSuccess = false;
		hr = xmlDom->load(CComVariant(fileName.c_str()), &bSuccess);
		if (FAILED(hr) || !bSuccess)
		{
			logs::Logger::OutputInfo(_T("%s Not load xml"), _T(__FUNCTION__));
			return throw std::exception();
		}

		CComPtr<IXMLDOMNodeList> spXMLNodeList;

		CComBSTR bstrSS(kUnitProfilerSelect);
		hr = xmlDom->selectNodes(bstrSS,&spXMLNodeList);

		if (FAILED(hr))
			logs::Logger::OutputInfo(_T("%s selectNodes FAIL"), _T(__FUNCTION__));

		long length;
		hr = spXMLNodeList->get_length(&length);
		if (FAILED(hr))
			logs::Logger::OutputInfo(_T("%s spXMLNodeList->get_length FAIL"), _T(__FUNCTION__));

		logs::Logger::OutputInfo(_T("spXMLNodeList->get_length %u"), length);

		for (long i = 0; i < length; i++)
		{
			CComPtr<IXMLDOMNode> nodeUnitProfiler;
			hr = spXMLNodeList->get_item(i, &nodeUnitProfiler);
            if (SUCCEEDED(hr))
            {
				ParseUnitProfiler(nodeUnitProfiler);
			}
			else
			{
				logs::Logger::OutputInfo(_T("\t Node %u get_item FAIL"), i);
			}
		}
		return;
	}

    void MnConfigXml::SelectClassMethods(MnClass* classItem, const wstring& methodName, const wstring& params, list<MnUnitProfile*>& methodList)
    {
        if (classItem != NULL)
        {
            MnMethod* methodConfigInfo = classItem->GetMnMethod(methodName);
            if  (methodConfigInfo != NULL)
            {
                methodConfigInfo->SelectUnitProfiles(params, methodList);
            }

            methodConfigInfo = classItem->GetMnMethod(L"");
            if  (methodConfigInfo != NULL)
            {
                methodConfigInfo->SelectUnitProfiles(params, methodList);
            }
        }
    }

	void MnConfigXml::SelectMethods(wstring assemblyName, wstring className, wstring methodName, wstring params, list<MnUnitProfile*>& methodList)
	{
		/*logs::Logger::OutputInfo(_T("%s assemblyName \"%s\", className \"%s\", methodName \"%s\", params \"%s\""), 
			_T(__FUNCTION__), assemblyName.c_str(), className.c_str(), methodName.c_str(), params.c_str() );*/
		if (m_assembliesMap.size() > 0 && methodName[0] != L'.')
        {
            MnAssembly* assembly = GetMnAssembly(assemblyName);
			if (assembly != NULL)
            {
                SelectClassMethods(assembly->GetMnClass(className), methodName, params, methodList);
                SelectClassMethods(assembly->GetMnClass(L""), methodName, params, methodList);
			}
		}
		return;
	}
}