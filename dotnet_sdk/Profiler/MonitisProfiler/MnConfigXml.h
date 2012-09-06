#ifndef MNCONFIGXML_H
#define MNCONFIGXML_H

#include <set>
using namespace std;

namespace Monitis
{
	typedef std::list<wstring> MnFileList;

    class MnUnitProfile
    {
    public:
        MnUnitProfile(const wstring& unitProfileName);
        inline wstring& GetNameUnitProfile() {return m_unitProfileName;};
    private:
        wstring m_unitProfileName;
    };

    typedef tr1::shared_ptr<MnUnitProfile> MnUnitProfileShrPtr;
    typedef std::list <MnUnitProfileShrPtr> MnUnitProfiles;

    typedef std::map<wstring, list<MnUnitProfile*> > MnSignatureMap;

	class MnMethod
	{
	public:
		MnMethod(const wstring& methodName);
        void AddSignature(const wstring& parameters, MnUnitProfile* unitProfile);
        wstring GetUnitProfileString(const wstring& parameters);
        void SelectUnitProfiles(const wstring& parameters, list<MnUnitProfile*>& unitProfileList);
#ifdef DEBUG
        MnSignatureMap& GetSignatureMap() {return m_signatureMap;};
#endif
	private:
		wstring m_methodName;
		MnSignatureMap m_signatureMap;
	};

    typedef tr1::shared_ptr<MnMethod> MnMethodShrPtr;
	typedef std::map <wstring, MnMethodShrPtr > MnMethodsMap;

	class MnClass
	{
	public:
		MnClass(const wstring& className, const wstring& assemblyName);
		
		MnMethod* GetMnMethod(const wstring& methodName);
		MnMethod* CreateMnMethod(const wstring& methodName, const wstring& params, MnUnitProfile* unitProfile);
        wstring& GetName() {return m_className;};

		MnMethodsMap& GetMnMethods() {return m_methods;};
	private:
		wstring m_className;
		wstring m_FullName;
		MnMethodsMap m_methods;
	};

	typedef std::map<wstring, tr1::shared_ptr<MnClass> > MnClassesMap;

	class MnAssembly
	{
	public:
		MnAssembly(const wstring& assemblyName);
		MnClass* GetMnClass(const wstring& className);
		MnClass* CreateMnClass(const wstring& className);

		wstring GetMnAssemblyName() {return m_assemblyName;};
		MnClassesMap& GetMnClasses() {return m_classes; };
	private:
		wstring m_assemblyName;
		MnClassesMap m_classes;
	};

	typedef std::map<wstring, tr1::shared_ptr<MnAssembly> > MnAssembliesMap;

    class MnConfigXml
	{
	public:
		void LoadConfigurationXmls();
		void SelectMethods(wstring assemblyName, wstring className, wstring methodName, wstring params, list<MnUnitProfile*>& methodList);
	protected:
		wstring GetConfigPath();
		void FillFileList(wstring configPath, MnFileList& fileList);
		void ParseUnitProfiler(IXMLDOMNode *upNode);
		void LoadXmlFile(wstring fileName);
		MnAssembly* GetMnAssembly(wstring assemblyName);
		MnAssembly* CreateMnAssembly(wstring assemblyName);

        void SelectClassMethods(MnClass* classItem, const wstring& methodName, const wstring& params, list<MnUnitProfile*>& methodList);

	private:
		MnAssembliesMap m_assembliesMap;
        MnUnitProfiles m_UnitProfiles;
	};

	class XmlHelper
	{
	public:
		static HRESULT GetChildNodes(IXMLDOMNode* currentNode, IXMLDOMNodeList **nodeList, long& lengthList)
		{
			HRESULT hr = currentNode->get_childNodes(nodeList);
			if (SUCCEEDED(hr))
			{
				long length;
				hr = (*nodeList)->get_length(&length);
				if (SUCCEEDED(hr))
				{
					lengthList = length;
				}
			}
			return hr;
		}
	};
}
#endif