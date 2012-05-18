#include "stdafx.h"
#include "LoggerHelper.h"

namespace logs
{
	const int kDebugBufferSize = 4*1024;
	
	Logger* Logger::s_LoggerInstance = NULL;

	std::wstring GetErrString(DWORD errorCode)
	{
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | 
					FORMAT_MESSAGE_IGNORE_INSERTS, NULL, errorCode, 0, (LPTSTR) &lpMsgBuf,
				0, NULL);
		
		OutputDebugString( (LPTSTR)lpMsgBuf );
		OutputDebugString( _T("\n") );

		std::wstring text((const wchar_t*)lpMsgBuf);
		LocalFree( lpMsgBuf );
		return std::wstring(text.begin(), text.end());
	}

	void LogWinErrCodeDebugString(DWORD errorCode)
	{
		LPVOID lpMsgBuf;
		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | 
					FORMAT_MESSAGE_IGNORE_INSERTS, NULL, errorCode, 0, (LPTSTR) &lpMsgBuf,
				0, NULL);
		OutputDebugString( (LPTSTR)lpMsgBuf );
		OutputDebugString( _T("\n") );
		LocalFree( lpMsgBuf );
	}

	void OutputDebug::WriteMessage(const wstring& str)
	{
		OutputDebugString((LPCWSTR)str.c_str());
	}

	OutputFile::OutputFile(const wstring& logFileName)
	{
		m_FileName = logFileName;
		m_FileHandle = NULL;
	}

	bool OutputFile::Open()
	{
		if (m_FileHandle != NULL && m_FileHandle != INVALID_HANDLE_VALUE)
			return true;
		m_FileHandle = CreateFile(m_FileName.c_str(), GENERIC_READ | GENERIC_WRITE,
					FILE_SHARE_READ, NULL, OPEN_ALWAYS, NULL, NULL);
		_ASSERT(m_FileHandle != INVALID_HANDLE_VALUE);
		return m_FileHandle != INVALID_HANDLE_VALUE;
	}

	void OutputFile::Close()
	{
		if (m_FileHandle != NULL && m_FileHandle != INVALID_HANDLE_VALUE)
			CloseHandle(m_FileHandle);
		m_FileHandle = NULL;
	}

	wstring OutputFile::GetLogFileName(const wstring& prefixFileName)
	{
		wchar_t tempPath[MAX_PATH];
		DWORD lenPath = /*GetTempPath*/GetCurrentDirectory(MAX_PATH, tempPath);
		_ASSERT(lenPath > 0 && lenPath < MAX_PATH);
		if (lenPath == 0 || lenPath > MAX_PATH)
		{
			return NULL;
		}

		SYSTEMTIME currentTime;
		GetLocalTime(&currentTime);

		TCHAR logFileName[MAX_PATH];
		int lenTimeStamp = swprintf_s(logFileName, MAX_PATH,
					_T("%s\\%s_%04u.%02u.%02u_%02u-%02u-%02u.%03u.log"), tempPath,  prefixFileName.c_str(),
					currentTime.wYear, currentTime.wMonth, currentTime.wDay, currentTime.wHour,
					currentTime.wMinute, currentTime.wSecond, currentTime.wMilliseconds);

		_ASSERT(lenTimeStamp > 0);
		if (lenTimeStamp <= 0)
		{
			return NULL;
		}

		return wstring(logFileName, lenTimeStamp);
	}

	void OutputFile::WriteMessage(const wstring& str)
	{
		if (!Open())
			return;
		DWORD lengthStr = (DWORD)str.length()*sizeof(wchar_t);
		if (!WriteFile(m_FileHandle, str.c_str(), lengthStr, &lengthStr, NULL)) 
			LogWinErrCodeDebugString(GetLastError());
	}

	Logger::Logger(IOutput* outStream)
	{
		::InitializeCriticalSection((LPCRITICAL_SECTION)&m_WriteMonitor);
		m_Stream = outStream;
	}

	Logger::~Logger()
	{
		::DeleteCriticalSection((LPCRITICAL_SECTION)&m_WriteMonitor);
	}

	Logger* Logger::CreateLogger(IOutput* logStream)
	{
		if (Logger::s_LoggerInstance == NULL)
			Logger::s_LoggerInstance = new Logger(logStream);
		return Logger::s_LoggerInstance;
	}

	void Logger::Release()
	{
		delete Logger::s_LoggerInstance;
		Logger::s_LoggerInstance = NULL;
	}


	void Logger::OutputDebug(LPCWSTR formatStr, va_list argList)
	{
		if (Logger::s_LoggerInstance == NULL) return;
	
		if (!formatStr) return;
	
		::EnterCriticalSection(&s_LoggerInstance->m_WriteMonitor);

		wchar_t theBuffer[kDebugBufferSize];

		SYSTEMTIME theTime;
		GetLocalTime(&theTime);
		int theTimeStampLength = swprintf_s(theBuffer, kDebugBufferSize,
					_T("[%04u.%02u.%02u %02u:%02u:%02u.%03u]"), theTime.wYear,
					theTime.wMonth, theTime.wDay, theTime.wHour, theTime.wMinute,
					theTime.wSecond, theTime.wMilliseconds);
		_ASSERT(theTimeStampLength > 0);
		if (theTimeStampLength <= 0)
		{
			::LeaveCriticalSection(&s_LoggerInstance->m_WriteMonitor);
			return;
		}
	
		int theLogStringLength = vswprintf_s(theBuffer + theTimeStampLength,
					kDebugBufferSize - theTimeStampLength, formatStr, argList);
	
		_ASSERT(theLogStringLength > 0);
		if (theLogStringLength <= 0)
		{
			::LeaveCriticalSection(&s_LoggerInstance->m_WriteMonitor);
			return;
		}

		wcscat_s(theBuffer, kDebugBufferSize, _T("\r\n"));
		s_LoggerInstance->m_Stream->WriteMessage(wstring(theBuffer, wcslen(theBuffer)));
		::LeaveCriticalSection(&s_LoggerInstance->m_WriteMonitor);
	}

	void Logger::OutputInfo(LPCWSTR formatStr, ...)
	{
#if LOG_LEVEL > 2
		va_list theVarArgs;
		va_start(theVarArgs, formatStr);

		OutputDebug(formatStr, theVarArgs);
#endif
	}

	void Logger::OutputWarning(LPCWSTR formatStr, ...)
	{
#if LOG_LEVEL > 1
		va_list theVarArgs;
		va_start(theVarArgs, formatStr);

		OutputDebug(formatStr, theVarArgs);
#endif
	}

	void Logger::OutputError(LPCWSTR formatStr, ...)
	{
#if LOG_LEVEL > 0
		va_list theVarArgs;
		va_start(theVarArgs, formatStr);

		OutputDebug(formatStr, theVarArgs);
#endif
	}
}