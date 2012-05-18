#include "StdAfx.h"

namespace Monitis
{
	MnFunctionInfo::MnFunctionInfo(FunctionID functionID,  const wstring& assemblyName, const wstring& className, const wstring& methodName)
	{
		m_functionID = functionID;
		m_AssemblyName = assemblyName;
		m_ClassName = className;
		m_MethodName = methodName;
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
}