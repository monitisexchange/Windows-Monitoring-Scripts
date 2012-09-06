// PipeServer.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

const WCHAR kPipeName[] = L"\\\\.\\pipe\\7682B383-2874-4422-AA8E-AFAEBE089F15";

bool ThreadCheckQuit()
{
    MSG msg;
    if (::PeekMessage(&msg, NULL, WM_QUIT, WM_QUIT, PM_REMOVE))
    {
        return (msg.message == WM_QUIT);
    }
    return false;
}

static wchar_t sText[2048];
static unsigned char sBuffer[4096]; 

DWORD __stdcall PipeServerThread(LPVOID lpParameter)
{    
    OVERLAPPED overlapped;
    bool quitThread = false;
    bool connected = false;

    HANDLE pipeHandle = CreateNamedPipe(kPipeName, PIPE_ACCESS_DUPLEX | FILE_FLAG_OVERLAPPED, PIPE_TYPE_BYTE, PIPE_UNLIMITED_INSTANCES, 4096, 4096, 0, NULL);
    if (pipeHandle == INVALID_HANDLE_VALUE) 
    {
        return 1;
    }

    HANDLE handleEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
    if (handleEvent == INVALID_HANDLE_VALUE) 
    {
        return 2;
    }

    while (!quitThread)
    {
        DWORD written;
        ResetEvent(handleEvent);
        memset(&overlapped, 0, sizeof(OVERLAPPED));
        overlapped.hEvent = handleEvent;

        connected = ConnectNamedPipe(pipeHandle, &overlapped) ?  true : (GetLastError() == ERROR_PIPE_CONNECTED); 

        while (!connected && !quitThread)
        {
            if (ERROR_IO_PENDING == ::GetLastError())
            {
                DWORD state = ::WaitForSingleObject(handleEvent, 1000);
                switch (state)
                {
                    case WAIT_OBJECT_0:
                        connected = !!GetOverlappedResult(pipeHandle, &overlapped, &written, false);
                        printf("Connect is %u\n", (int)connected);
                        break;

                    case WAIT_TIMEOUT:
                        quitThread = ThreadCheckQuit();
                        break;

                    default:
                        break;
                }
            }
            else
            {
                return 1;
            }
        }

        while (connected && !quitThread)
        {
            ResetEvent(handleEvent);
            memset(&overlapped, 0, sizeof(OVERLAPPED));
            overlapped.hEvent = handleEvent;
            bool isDataRead = !!ReadFile(pipeHandle, sBuffer, sizeof(sBuffer), &written, &overlapped);
            if (!isDataRead)
            {
                if (ERROR_IO_PENDING == ::GetLastError())
                {
                    DWORD state = WaitForSingleObject(overlapped.hEvent, INFINITE);
                    if (state == WAIT_OBJECT_0)
                    {
                        isDataRead = !!GetOverlappedResult(pipeHandle, &overlapped, &written, false);
                    }
                }
            }
            if (isDataRead)
            {
                memcpy(sText, sBuffer, written);
                sText[written] = L'\0';
                wprintf(L"%s\n", sText);
            }
        }
    }

    if (connected)
    {
        DisconnectNamedPipe(pipeHandle);
    }
    CloseHandle(handleEvent);
    CloseHandle(pipeHandle);
    return 0;
}

int _tmain(int argc, _TCHAR* argv[])
{
    DWORD pipeThreadId;
    HANDLE pipeThreadHandle = CreateThread(NULL, 0, PipeServerThread, NULL, 0, &pipeThreadId);
    getchar();
    PostThreadMessage(pipeThreadId, WM_QUIT, 0, 0);
    DWORD state = WaitForSingleObject(pipeThreadHandle, 1000*60);
    if (state != WAIT_OBJECT_0)
    {
        TerminateThread(pipeThreadHandle, 3);
    }
    CloseHandle(pipeThreadHandle);
    return 0;
}