#ifndef MNPIPE_H
#define MNPIPE_H

#include <list>
#include <vector>

using namespace std;

namespace Monitis
{
    const WCHAR kPipeNameFormat[] = L"\\\\.\\pipe\\%s";
    const WCHAR kPipeName[] = L"\\\\.\\pipe\\7682B383-2874-4422-AA8E-AFAEBE089F15";
    const int kMarkerOffset = 0;
    const int kLengthOffset = 1;

    __declspec(align(1)) typedef struct 
    {
        DWORD marker;
        DWORD length;
    }MnTransferHeader;

    typedef list<vector<unsigned char>* > MnQueue;

    class MnPipe
    {
    private:
        MnPipe(void);
    public:
        ~MnPipe(void);
    public:
        static MnPipe* CreateMonitisPipe();
        static void PutIntoPipe(LPCWSTR formatStr, ...);
        void Shutdown();
    private:
        static DWORD __stdcall PipeThread(LPVOID lpParameter);

    private:
        static MnPipe* s_InstancePipe;
        //HANDLE m_PipeHandle;
        DWORD m_PipeThreadId;
        HANDLE m_PipeThreadHandle;
        CRITICAL_SECTION m_CsQueue;
        HANDLE m_QueueEvent;
        MnQueue m_Queue;
        //bool m_ConnectedPipe;
    };
}
#endif

