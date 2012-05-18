#ifndef LOGSHELPER_H
#define LOGSHELPER_H

#include <string>
#include <Windows.h>

#ifndef LOG_LEVEL
	#ifdef _DEBUG
		#define LOG_LEVEL 3
	#else
		#define LOG_LEVEL 1
	#endif
#endif

using namespace std;

namespace logs
{
	class IOutput
	{
	public:
		virtual void WriteMessage(const wstring& str) = NULL;
		virtual bool Open() {return true;};
		virtual void Close() {};
	};

	class OutputDebug: public IOutput
	{
	public:
		void WriteMessage(const wstring& str);
	};

	class OutputFile: public IOutput
	{
	private:
		wstring m_FileName;
		HANDLE m_FileHandle;
	public:
		OutputFile(const wstring& logFileName);
		void WriteMessage(const wstring& str);
		bool Open();
		void Close();

	public:
		static wstring GetLogFileName(const wstring& prefixFileName);
	};

	class Logger
	{
	private:
		static Logger* s_LoggerInstance;
		CRITICAL_SECTION m_WriteMonitor;
		IOutput* m_Stream;
		Logger(IOutput* logStream);
		~Logger();
		static void OutputDebug(LPCWSTR formatStr, va_list argList);

	public:
		static Logger* CreateLogger(IOutput* logStream);
		static void Release();
		static void OutputInfo(LPCWSTR formatStr, ...);
		static void OutputWarning(LPCWSTR formatStr, ...);
		static void OutputError(LPCWSTR formatStr, ...);	
	};

	std::wstring GetErrString(DWORD errorCode);
	void LogWinErrCodeDebugString(DWORD errorCode);
}
#endif
