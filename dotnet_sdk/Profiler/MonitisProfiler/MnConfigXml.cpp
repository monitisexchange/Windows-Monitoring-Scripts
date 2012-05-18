#include "stdafx.h"

#include <algorithm>
#include <sstream>

namespace Monitis
{
	const WCHAR kRegistryMonitis[] = L"SOFTWARE\\Monitis";
	const WCHAR kInstalledPath[] = L"InstalledPath";
	const WCHAR kExtensionSubDir[] = L"Extensions";
	const WCHAR kXmlFileMask[] = L"*.xml";

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
				wstringstream wss;
				wss << wstring(monitisPath, sz).c_str() << L"\\" << kExtensionSubDir;
				installPath = wss.str();
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
			LoadXmlFile(*xmlItem);
		}

		for (std::map<std::wstring, std::list<std::wstring> >::iterator item = m_methodsAccept.begin(); item != m_methodsAccept.end(); item++)
		{
			logs::Logger::OutputInfo(_T("\t%s"), item->first.c_str());
			for (std::list<std::wstring>::iterator mitem = item->second.begin(); mitem != item->second.end(); mitem++)
				logs::Logger::OutputInfo(_T("\t\t%s"), (*mitem).c_str());
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
			return;
		}

		CComPtr<IXMLDOMNodeList> spXMLNodeList;

		CComBSTR bstrSS(L"extension/instrumentation/tracerFactory");
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
			CComPtr<IXMLDOMNode> xmlNode;
			hr = spXMLNodeList->get_item(i, &xmlNode);
			if (SUCCEEDED(hr))
			{
				CComPtr<IXMLDOMNode> match;
				hr = xmlNode->get_firstChild(&match);
				if (SUCCEEDED(hr))
				{
					//logs::Logger::OutputInfo(_T("\t Node %u Text %s\n"), i, std::wstring(nodeText).c_str());
					CComPtr<IXMLDOMNamedNodeMap> attributes;
					wstring assemblyName;
					wstring className;
					std::list<std::wstring> methodsName;
					hr = match->get_attributes(&attributes);
					if (SUCCEEDED(hr))
					{
						assemblyName = GetAttributeText(attributes, std::wstring(L"assemblyName"));
						className = GetAttributeText(attributes, std::wstring(L"className"));
						//logs::Logger::OutputInfo(_T("%s:%s"), assemblyName.c_str(), className.c_str());
					}
					CComPtr<IXMLDOMNodeList> methodList;
					hr = match->get_childNodes(&methodList);
					if (SUCCEEDED(hr))
					{
						long methodNumber;
						hr = methodList->get_length(&methodNumber);
						if (SUCCEEDED(hr))
						{
							for (long j = 0; j < methodNumber; j++)
							{
								CComPtr<IXMLDOMNode> methodNode;
								hr = methodList->get_item(j, &methodNode);
								if (SUCCEEDED(hr) && methodNode.p != NULL)
								{
									CComPtr<IXMLDOMNamedNodeMap> methodAttributes;
									hr = methodNode->get_attributes(&methodAttributes);
									if (SUCCEEDED(hr))
									{
										wstring methodName = GetAttributeText(methodAttributes, L"methodName");
										methodsName.push_back(methodName);
									}
								}
							}
						}
					}
					wstring first = assemblyName + L":" + className;
					std::map<std::wstring, std::list<std::wstring> >::iterator classItem = m_methodsAccept.find(first.c_str());
					if (classItem != m_methodsAccept.end())
					{
						for (std::list<std::wstring>::iterator methodItem = methodsName.begin(); methodItem != methodsName.end(); methodItem++)
						{
							if (classItem->second.end() == std::find(classItem->second.begin(), classItem->second.end(), *methodItem))
							{
								classItem->second.push_back(*methodItem);
							}
						}
					}
					else
					{
						m_methodsAccept.insert(std::pair<std::wstring, std::list<std::wstring> >(first, methodsName));	
					}
				}
			}
			else
			{
				logs::Logger::OutputInfo(_T("\t Node %u get_item FAIL"), i);
			}
		}
		return;
	}

	bool MnConfigXml::FindMethod(wstring assemblyName, wstring className, wstring methodName)
	{
		//
		if (m_methodsAccept.size() > 0)
		{
			wstringstream wss;
			wss << assemblyName.c_str() << L":" << className.c_str();

			//logs::Logger::OutputWarning(_T("%s ClassName: %s"), _T(__FUNCTION__), wss.str().c_str());
			std::map<std::wstring, std::list<std::wstring> >::iterator classItem = m_methodsAccept.find(wss.str().c_str());
			if (classItem != m_methodsAccept.end())
			{
				std::list<std::wstring>::iterator methodItem = std::find(classItem->second.begin(), classItem->second.end(), methodName.c_str());
				return (methodItem != classItem->second.end());
			}
		}
		return false;
	}
}