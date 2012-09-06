#include "StdAfx.h"
#include "MnPipe.h"

namespace Monitis
{
    #define WM_PIPESHUTDOWN WM_USER + 100
    MnPipe* MnPipe::s_InstancePipe = NULL;
    const DWORD kStartMarker = 0x03F703F7;

    MnPipe* MnPipe::CreateMonitisPipe()
    {
        //DebugBreak();
        try
        {
            if (MnPipe::s_InstancePipe == NULL)
            {
                MnPipe::s_InstancePipe = new MnPipe();
            }
            return MnPipe::s_InstancePipe;
        }
        catch (...)
        {
            return NULL;
        }
    }

    MnPipe::MnPipe(void)
    {
        InitializeCriticalSection(&m_CsQueue);
        m_QueueEvent = ::CreateEvent(NULL, TRUE, FALSE, NULL);

        m_PipeThreadHandle = ::CreateThread(NULL, 0, PipeThread, this, 0, &m_PipeThreadId);
        if (m_PipeThreadHandle == INVALID_HANDLE_VALUE)
        {
            throw exception();
        }
    }

    MnPipe::~MnPipe(void)
    {
        PostThreadMessage(m_PipeThreadId, WM_QUIT, 0, 0);
        WaitForSingleObject(m_PipeThreadHandle, 2000);
        DeleteCriticalSection(&m_CsQueue);
        CloseHandle(m_QueueEvent);
        CloseHandle(m_PipeThreadHandle);
    }

    bool ThreadCheckQuit()
    {
        MSG msg;
        if (::PeekMessage(&msg, NULL, WM_QUIT, WM_QUIT, PM_REMOVE))
        {
            //DebugBreak();
            return (msg.message == WM_QUIT);
        }
        return false;
    }

    bool CompleteAsync(HANDLE handle, OVERLAPPED* lpOverlapped)
    {
        DWORD numberOfBytesTransferred;
        return !!::GetOverlappedResult(handle, lpOverlapped, &numberOfBytesTransferred, false);
    }

    DWORD __stdcall MnPipe::PipeThread(LPVOID lpParameter)
    {
        //DebugBreak();
        static wchar_t readPipeName[512];
        static wchar_t pipeName[512];
        OVERLAPPED overlapped;
        HANDLE asyncEvent = ::CreateEvent(NULL, TRUE, FALSE, NULL);
        HANDLE pipeHandle = INVALID_HANDLE_VALUE;

        MnPipe* instancePipe = (MnPipe*)lpParameter;
        bool quitThread = false;
        bool shutdownn = false; 

        while (!quitThread)
        {
            ::ResetEvent(asyncEvent);
            while (pipeHandle == INVALID_HANDLE_VALUE && !quitThread)
            {
                pipeHandle = ::CreateFile(kPipeName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
                if (pipeHandle == INVALID_HANDLE_VALUE && ::GetLastError() == ERROR_FILE_NOT_FOUND)
                {
                    if (WAIT_FAILED != ::WaitForSingleObject(asyncEvent, 1000))
                    {
                        quitThread = ThreadCheckQuit();
                    }
                    else
                    {
                        quitThread = true;
                    }
                }
            }

            DWORD position = 0;
            LPBYTE buffer = (LPBYTE)readPipeName;
            memset(buffer, 0, 512*sizeof(wchar_t));

            while (!quitThread)
            {
                DWORD written;
                ResetEvent(asyncEvent);
                memset(&overlapped, 0, sizeof(OVERLAPPED));
                overlapped.hEvent = asyncEvent;
                bool isDataRead = !!ReadFile(pipeHandle, buffer + position, sizeof(readPipeName)*sizeof(wchar_t) - position, &written, &overlapped);
                
                if (!isDataRead)
                {
                    if (ERROR_IO_PENDING == ::GetLastError())
                    {
                        while (!quitThread)
                        {
                            DWORD state = WaitForSingleObject(overlapped.hEvent, 1000);
                            if (state == WAIT_OBJECT_0)
                            {
                                isDataRead = !!GetOverlappedResult(pipeHandle, &overlapped, &written, false);
                                if (isDataRead)
                                {
                                    position += written;
                                }
                                break;
                            }
                            else if (state == WAIT_TIMEOUT)
                            {
                                quitThread = ThreadCheckQuit();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    position += written;
                }
            }
            CloseHandle(pipeHandle);
            pipeHandle = INVALID_HANDLE_VALUE;

            swprintf_s(pipeName, sizeof(pipeName), kPipeNameFormat, readPipeName);
            logs::Logger::OutputInfo(_T("%s Read Pipe name %s"), _T(__FUNCTION__), pipeName);
            //DebugBreak();
            while (pipeHandle == INVALID_HANDLE_VALUE && !quitThread)
            {
                pipeHandle = ::CreateFile(pipeName, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
                if (pipeHandle == INVALID_HANDLE_VALUE && ::GetLastError() == ERROR_FILE_NOT_FOUND)
                {
                    if (WAIT_FAILED != ::WaitForSingleObject(instancePipe->m_QueueEvent, 1000))
                    {
                        quitThread = ThreadCheckQuit();
                    }
                    else
                    {
                        quitThread = true;
                    }
                }
            }

            if (!quitThread)
            {
                quitThread = ThreadCheckQuit();
            }
            logs::Logger::OutputInfo(_T("%s Conented"), _T(__FUNCTION__));
            memset(&overlapped, 0, sizeof(OVERLAPPED));
            overlapped.hEvent = asyncEvent;

            while (!quitThread)
            {
                DWORD waitState = ::WaitForSingleObject(instancePipe->m_QueueEvent, 1000);
                logs::Logger::OutputInfo(_T("%s State wait event %u"), _T(__FUNCTION__), waitState);
                if (waitState == WAIT_TIMEOUT)
                {
                    quitThread = ThreadCheckQuit(); 
                }
                else if (waitState == WAIT_OBJECT_0)
                {
                    //DebugBreak();
                    while (instancePipe->m_Queue.size() > 0 /*&& !quitThread*/)
                    {
                        EnterCriticalSection(&instancePipe->m_CsQueue);
                        tr1::shared_ptr< vector<unsigned char> > data(*(instancePipe->m_Queue.begin()));
                        instancePipe->m_Queue.pop_front();
                        LeaveCriticalSection(&instancePipe->m_CsQueue);
                        memset(&overlapped, 0, sizeof(OVERLAPPED));
                        ::ResetEvent(asyncEvent);
                        overlapped.hEvent = asyncEvent;
                        logs::Logger::OutputInfo(_T("%s Write pipe \"%s\""), _T(__FUNCTION__), data.get()->data()+8);
                        if (!WriteFile(pipeHandle, data.get()->data(), (DWORD)data.get()->size(), NULL, &overlapped))
                        {
                            if (ERROR_IO_PENDING == ::GetLastError())
                            {
                                while (true)
                                {
                                    DWORD writeState = WaitForSingleObject(asyncEvent, 1000);
                                    if (writeState == WAIT_OBJECT_0)
                                    {
                                        CompleteAsync(pipeHandle, &overlapped);
                                        break;
                                    }
                                    else if (writeState == WAIT_TIMEOUT)
                                    {
                                        quitThread = ThreadCheckQuit(); 
                                    }
                                    else if (writeState == WAIT_FAILED)
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                        logs::Logger::OutputInfo(_T("%s Write is complete"), _T(__FUNCTION__));
                    }
                    ::ResetEvent(instancePipe->m_QueueEvent);
                }
                else if (waitState == WAIT_FAILED)
                {
                    logs::Logger::OutputInfo(_T("%s Quit 1"), _T(__FUNCTION__));
                    return 1;
                }
            }
            CloseHandle(pipeHandle);
        }
        CloseHandle(asyncEvent);
        logs::Logger::OutputInfo(_T("%s Quit 0"), _T(__FUNCTION__));
        return 0;
    }

    const int kBufferLength = 2048;

    void MnPipe::PutIntoPipe(LPCWSTR formatStr, ...)
    {
        MnTransferHeader header;
        if (!formatStr) return;

        if (MnPipe::s_InstancePipe == NULL) return;

        va_list varArgs;
        va_start(varArgs, formatStr);

        tr1::shared_ptr<wchar_t> theBuffer(new wchar_t [kBufferLength]);

        SYSTEMTIME theTime;
        GetLocalTime(&theTime);
        int timeStampLength = swprintf_s(theBuffer.get(), kBufferLength,
            _T("DateTime=\"%04u.%02u.%02u %02u:%02u:%02u.%03u\";"), theTime.wYear,
            theTime.wMonth, theTime.wDay, theTime.wHour, theTime.wMinute,
            theTime.wSecond, theTime.wMilliseconds);

        int stringLength = vswprintf_s(theBuffer.get() + timeStampLength, kBufferLength - timeStampLength, formatStr, varArgs);
        //wcscat_s(theBuffer, kBufferLength, _T(""));
        header.marker = kStartMarker;
        header.length = (timeStampLength + stringLength + 1) * sizeof(wchar_t);
        logs::Logger::OutputError(_T("%s Add pipe message %s"), _T(__FUNCTION__), theBuffer.get());
        vector<unsigned char>* buffer = new vector<unsigned char>(sizeof(MnTransferHeader) + header.length);
        memcpy(buffer->data(), &header, sizeof(MnTransferHeader));
        memcpy(buffer->data() + sizeof(MnTransferHeader), theBuffer.get(), header.length);
        EnterCriticalSection(&MnPipe::s_InstancePipe->m_CsQueue);
        MnPipe::s_InstancePipe->m_Queue.push_back(buffer);
        LeaveCriticalSection(&MnPipe::s_InstancePipe->m_CsQueue);
        ::SetEvent(MnPipe::s_InstancePipe->m_QueueEvent);
    }

    void MnPipe::Shutdown()
    {
        PostThreadMessage(m_PipeThreadId, WM_QUIT, 0, 0);
    }

}