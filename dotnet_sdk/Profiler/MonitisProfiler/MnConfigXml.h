#ifndef MNCONFIGXML_H
#define MNCONFIGXML_H

using namespace std;

namespace Monitis
{
	typedef std::list<wstring> MnFileList;
	class MnConfigXml
	{
	public:
		void LoadConfigurationXmls();
		bool FindMethod(wstring assemblyName, wstring className, wstring methodName);
	protected:
		wstring GetConfigPath();
		void FillFileList(wstring configPath, MnFileList& fileList);
		void LoadXmlFile(wstring fileName);
	private:
		std::map<std::wstring, std::list<std::wstring> > m_methodsAccept;
	};
}
#endif