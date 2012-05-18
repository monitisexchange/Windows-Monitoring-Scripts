#ifndef MNFUNCTIONINFO_H
#define MNFUNCTIONINFO_H

#include <string>
#include <list>
#include <map>

using namespace std;

namespace Monitis
{
	class MnFunctionInfo
	{
	public:
		MnFunctionInfo(FunctionID functionID,  const wstring& assemblyName, const wstring& className, const wstring& methodName);

		wstring& GetAssemblyName();
		wstring& GetClassName();
		wstring& GetFunctionName();
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
		long m_callCount;
		std::list<SYSTEMTIME> m_Enters;
		ULONGLONG m_Duration;
	};

	typedef std::map<FunctionID, MnFunctionInfo*> MnFunctionMap;
}
#endif
