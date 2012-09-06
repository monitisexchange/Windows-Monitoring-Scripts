// MnProfiler.cpp : Implementation of CMnProfiler

#include "stdafx.h"
#include "MnProfiler.h"
#include <algorithm>
#include "MnPipe.h"

CMnProfiler* g_pICorProfilerCallback = NULL;

using namespace logs;

void __stdcall FunctionEnterGlobal(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo, COR_PRF_FUNCTION_ARGUMENT_INFO *argInfo)
{
	//logs::Logger::OutputWarning(_T("%s FunctionID: %u"), _T(__FUNCTION__), (long)functionID);
	if (g_pICorProfilerCallback != NULL)
		g_pICorProfilerCallback->EnterHandle(functionID, clientData, frameInfo, argInfo);
}

void __stdcall FunctionLeaveGlobal(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange)
{
	//logs::Logger::OutputWarning(_T("%s FunctionID: %u"), _T(__FUNCTION__), (long)functionID);
	if (g_pICorProfilerCallback != NULL)
		g_pICorProfilerCallback->LeaveHandle(functionID, clientData, frameInfo, retvalRange);
}

void __stdcall FunctionTailcallGlobal(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo)
{
	//logs::Logger::OutputWarning(_T("%s FunctionID: %u"), _T(__FUNCTION__), (long)functionID);
	if (g_pICorProfilerCallback != NULL)
		g_pICorProfilerCallback->TailcallHandle(functionID,clientData,frameInfo);
}

// CMnProfiler
IOutput* CMnProfiler::s_LogFile = NULL;

CMnProfiler::CMnProfiler()
{
	if (CMnProfiler::s_LogFile == NULL)
		s_LogFile = static_cast<IOutput*>
		(new OutputFile(OutputFile::GetLogFileName(wstring(_T("mn")))));
	Logger::CreateLogger(s_LogFile);
	m_configXml = std::tr1::shared_ptr<Monitis::MnConfigXml>(new MnConfigXml());
    m_pipe = std::tr1::shared_ptr<Monitis::MnPipe>(Monitis::MnPipe::CreateMonitisPipe());
}

HRESULT CMnProfiler::FinalConstruct()
{
	m_configXml->LoadConfigurationXmls();
	return S_OK;
}

void CMnProfiler::FinalRelease()
{
}

STDMETHODIMP CMnProfiler::Initialize(IUnknown *pICorProfilerInfoUnk)
{
	g_pICorProfilerCallback = this;

	//DebugBreak();

	//logs::Logger::OutputInfo(_T("Start %s"), _T(__FUNCTION__));

	SetHandleProcs((FunctionCallback)&FunctionEnterGlobal, (FunctionCallback)&FunctionLeaveGlobal, &FunctionTailcallGlobal);

	HRESULT hr = pICorProfilerInfoUnk->QueryInterface(IID_ICorProfilerInfo, (LPVOID*)&m_pICorProfilerInfo);
	if (FAILED(hr))
		return E_FAIL;

	hr = pICorProfilerInfoUnk->QueryInterface(IID_ICorProfilerInfo2, (LPVOID*)&m_pICorProfilerInfo2);
	if (FAILED(hr))
	{
		// we still want to work if this call fails, might be an older .NET version
		m_pICorProfilerInfo2.p = NULL;
	}

	hr = SetEventMask();
	if (FAILED(hr))
		logs::Logger::OutputWarning(_T("Error setting the event mask: %s"), _T(__FUNCTION__));

	if (m_pICorProfilerInfo2.p == NULL)
	{
		// note that we are casting our functions to the definitions for the callbacks
		hr = m_pICorProfilerInfo->SetEnterLeaveFunctionHooks((FunctionEnter*)&EnterNakedProc, (FunctionLeave*)&LeaveNakedProc, (FunctionTailcall*)&TailNakedProc);
		if (SUCCEEDED(hr))
			hr = m_pICorProfilerInfo->SetFunctionIDMapper((FunctionIDMapper*)&FunctionMapper);
	}
	else
	{
		hr = m_pICorProfilerInfo2->SetEnterLeaveFunctionHooks2((FunctionEnter2*)&EnterNakedProc, (FunctionLeave2*)&LeaveNakedProc, (FunctionTailcall2*)&TailNakedProc);
		if (SUCCEEDED(hr))
			hr = m_pICorProfilerInfo2->SetFunctionIDMapper(FunctionMapper);
	}
	//logs::Logger::OutputInfo(_T("End %s"), _T(__FUNCTION__));
	return S_OK;
}

STDMETHODIMP CMnProfiler::Shutdown()
{
// 	std::map<FunctionID, MnFunctionInfo*>::iterator iter;
// 	for (iter = m_functionMap.begin(); iter != m_functionMap.end(); iter++)
// 	{
// 		// log the function's info
// 		MnFunctionInfo* functionInfo = iter->second;
// 		//LogString("%s : call count = %d\r\n", functionInfo->GetName(), functionInfo->GetCallCount());
// 		// free the memory for the object
// 		delete iter->second;
// 	}
	// clear the map
	m_functionMap.clear();

	// tear down our global access pointers
	g_pICorProfilerCallback = NULL;
    m_pipe.get()->Shutdown();
    return S_OK;
}

STDMETHODIMP CMnProfiler::AppDomainCreationStarted(AppDomainID appDomainID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AppDomainCreationFinished(AppDomainID appDomainID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AppDomainShutdownStarted(AppDomainID appDomainID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AppDomainShutdownFinished(AppDomainID appDomainID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AssemblyLoadStarted(AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AssemblyLoadFinished(AssemblyID assemblyID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AssemblyUnloadStarted(AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::AssemblyUnloadFinished(AssemblyID assemblyID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ModuleLoadStarted(ModuleID moduleID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ModuleLoadFinished(ModuleID moduleID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ModuleUnloadStarted(ModuleID moduleID)
{
    return S_OK;
}
	  
STDMETHODIMP CMnProfiler::ModuleUnloadFinished(ModuleID moduleID, HRESULT hrStatus)
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::ModuleAttachedToAssembly(ModuleID moduleID, AssemblyID assemblyID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ClassLoadStarted(ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ClassLoadFinished(ClassID classID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ClassUnloadStarted(ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ClassUnloadFinished(ClassID classID, HRESULT hrStatus)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::FunctionUnloadStarted(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITCompilationStarted(FunctionID functionID, BOOL fIsSafeToBlock)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITCompilationFinished(FunctionID functionID, HRESULT hrStatus, BOOL fIsSafeToBlock)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITCachedFunctionSearchStarted(FunctionID functionID, BOOL *pbUseCachedFunction)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITCachedFunctionSearchFinished(FunctionID functionID, COR_PRF_JIT_CACHE result)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITFunctionPitched(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::JITInlining(FunctionID callerID, FunctionID calleeID, BOOL *pfShouldInline)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::UnmanagedToManagedTransition(FunctionID functionID, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ManagedToUnmanagedTransition(FunctionID functionID, COR_PRF_TRANSITION_REASON reason)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ThreadCreated(ThreadID threadID)
{
    logs::Logger::OutputInfo(_T("%s ThreadID %X"), _T(__FUNCTION__), threadID);
    if (m_ThreadMap.find(threadID) == m_ThreadMap.end())
    {
        m_ThreadMap[threadID] = 0;
    }
    else
    {
        logs::Logger::OutputError(_T("Duplicate ThreadID %X"), threadID);
    }
    return S_OK;
}

STDMETHODIMP CMnProfiler::ThreadDestroyed(ThreadID threadID)
{
    logs::Logger::OutputInfo(_T("%s ThreadID %X"), _T(__FUNCTION__), threadID);
    m_ThreadMap.erase(threadID);
    return S_OK;
}

STDMETHODIMP CMnProfiler::ThreadAssignedToOSThread(ThreadID managedThreadID, DWORD osThreadID) 
{
    logs::Logger::OutputInfo(_T("%s managedThreadID %X osThreadID %X"), _T(__FUNCTION__), managedThreadID, osThreadID);
    if (m_ThreadMap.find(managedThreadID) != m_ThreadMap.end())
    {
        m_ThreadMap[managedThreadID] = osThreadID;
    }
    else
    {
        logs::Logger::OutputError(_T("Duplicate ThreadID %X"), managedThreadID);
    }
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingClientInvocationStarted()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingClientSendingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingClientReceivingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingClientInvocationFinished()
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingServerReceivingMessage(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingServerInvocationStarted()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingServerInvocationReturned()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RemotingServerSendingReply(GUID *pCookie, BOOL fIsAsync)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeSuspendStarted(COR_PRF_SUSPEND_REASON suspendReason)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeSuspendFinished()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeSuspendAborted()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeResumeStarted()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeResumeFinished()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeThreadSuspended(ThreadID threadID)
{
    logs::Logger::OutputError(_T("%s ThreadID %X"), _T(__FUNCTION__), threadID);
    return S_OK;
}

STDMETHODIMP CMnProfiler::RuntimeThreadResumed(ThreadID threadID)
{
    logs::Logger::OutputError(_T("%s ThreadID %X"), _T(__FUNCTION__), threadID);
    return S_OK;
}

STDMETHODIMP CMnProfiler::MovedReferences(ULONG cmovedObjectIDRanges, ObjectID oldObjectIDRangeStart[], ObjectID newObjectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ObjectAllocated(ObjectID objectID, ClassID classID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ObjectsAllocatedByClass(ULONG classCount, ClassID classIDs[], ULONG objects[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ObjectReferences(ObjectID objectID, ClassID classID, ULONG objectRefs, ObjectID objectRefIDs[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RootReferences(ULONG rootRefs, ObjectID rootRefIDs[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionThrown(ObjectID thrownObjectID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionUnwindFunctionEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionUnwindFunctionLeave()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionSearchFunctionEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionSearchFunctionLeave()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionSearchFilterEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionSearchFilterLeave()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionSearchCatcherFound(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionCLRCatcherFound()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionCLRCatcherExecute()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionOSHandlerEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionOSHandlerLeave(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionUnwindFinallyEnter(FunctionID functionID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionUnwindFinallyLeave()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionCatcherEnter(FunctionID functionID,
    											 ObjectID objectID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ExceptionCatcherLeave()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::COMClassicVTableCreated(ClassID wrappedClassID, REFGUID implementedIID, void *pVTable, ULONG cSlots)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::COMClassicVTableDestroyed(ClassID wrappedClassID, REFGUID implementedIID, void *pVTable)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::ThreadNameChanged(ThreadID threadID, ULONG cchName, WCHAR name[])
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::GarbageCollectionStarted(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason)
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::SurvivingReferences(ULONG cSurvivingObjectIDRanges, ObjectID objectIDRangeStart[], ULONG cObjectIDRangeLength[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::GarbageCollectionFinished()
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::FinalizeableObjectQueued(DWORD finalizerFlags, ObjectID objectID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::RootReferences2(ULONG cRootRefs, ObjectID rootRefIDs[], COR_PRF_GC_ROOT_KIND rootKinds[], COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIDs[])
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::HandleCreated(GCHandleID handleID, ObjectID initialObjectID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::HandleDestroyed(GCHandleID handleID)
{
    return S_OK;
}

STDMETHODIMP CMnProfiler::InitializeForAttach(IUnknown* pCorProfilerInfoUnk, void* pvClientData, UINT cbClientData)
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::ProfilerAttachComplete()
{
	return S_OK;
}

STDMETHODIMP CMnProfiler::ProfilerDetachSucceeded()
{
	return S_OK;
}

HRESULT CMnProfiler::SetEventMask()
{
	DWORD eventMask = (DWORD)(COR_PRF_MONITOR_ENTERLEAVE | COR_PRF_MONITOR_THREADS);
	return m_pICorProfilerInfo->SetEventMask(eventMask);
}

UINT_PTR CMnProfiler::FunctionMapper(FunctionID functionID, BOOL *pbHookFunction)
{
	//logs::Logger::OutputWarning(_T("%s FunctionID: %u"), _T(__FUNCTION__), (long)functionID);
	if (g_pICorProfilerCallback != NULL)
		*pbHookFunction = g_pICorProfilerCallback->MapFunction(functionID);

	return (UINT_PTR)functionID;
}

//void CMnProfiler::SelectMethods(MnFunctionInfo *methodInfo, MnMethods& methods)
//{
//    if (m_configXml.get() == NULL || methodInfo == NULL)
//    {
//        return;
//    }
//    
//}

wstring GetUnitProfileString(const list<MnUnitProfile*>& methodList)
{
    wstring str(L"");
    for (list<MnUnitProfile*>::const_iterator itemUnit = methodList.begin(); itemUnit != methodList.end(); itemUnit++)
    {
        if (itemUnit != methodList.begin())
        {
            str += L",";
        }
        str += (*itemUnit)->GetNameUnitProfile().c_str();
    }
    return str;
}

bool CMnProfiler::MapFunction(FunctionID functionID)
{
	MnFunctionMap::iterator iter = m_functionMap.find(functionID);
	if (iter == m_functionMap.end())
	{
		const WCHAR* p = NULL;
		USES_CONVERSION;

		wstring assemblyName(L"");
		wstring className(L"");
		wstring methodName(L"");
        wstring methodParameters(L"");
		
		HRESULT hr = GetMethodInfo(functionID, assemblyName, className, methodName, methodParameters); 
		if (FAILED(hr))
		{
			return false;
		}
		
		//logs::Logger::OutputWarning(_T("Assembly %s Class %s Method %s(%s)"), assemblyName.c_str(), className.c_str(), methodName.c_str(), methodParameters.c_str());

        list<MnUnitProfile*> unitProfiles;
        m_configXml->SelectMethods(assemblyName, className, methodName, methodParameters, unitProfiles);
		if (unitProfiles.size() > 0)
		{
            tr1::shared_ptr<MnFunctionInfo> functionInfo(new MnFunctionInfo(functionID, assemblyName, className, methodName, methodParameters, GetUnitProfileString(unitProfiles)));
			m_functionMap.insert(std::pair<FunctionID, tr1::shared_ptr<MnFunctionInfo> >(functionID, functionInfo));
			return true;
		}
		return false;
	}
	return true;
}

HRESULT CMnProfiler::GetMethodInfo(FunctionID functionID, wstring& assemblyName, wstring& className, wstring& methodName, wstring& methodParameters)
{
	IMetaDataImport* pIMetaDataImport = 0;
	HRESULT hr = S_OK;
	mdToken funcToken = 0;
	WCHAR szName[NAME_BUFFER_SIZE];
	ClassID classId;
	ModuleID moduleId;

	hr = m_pICorProfilerInfo->GetFunctionInfo(functionID, &classId, &moduleId, &funcToken);
	if(SUCCEEDED(hr))
	{
		AssemblyID assemblyID;
		ULONG cchModule;
		LPCBYTE baseLoadAddress;
		hr = m_pICorProfilerInfo->GetModuleInfo(moduleId, &baseLoadAddress, NAME_BUFFER_SIZE, &cchModule, szName, &assemblyID);
		if(SUCCEEDED(hr))
		{
			AppDomainID appDomainId;
			hr = m_pICorProfilerInfo->GetAssemblyInfo(assemblyID, NAME_BUFFER_SIZE, &cchModule, szName, &appDomainId, &moduleId);
			if(SUCCEEDED(hr))
			{
				assemblyName = wstring(szName, cchModule);
				hr = m_pICorProfilerInfo->GetTokenAndMetaDataFromFunction(functionID, IID_IMetaDataImport, (LPUNKNOWN *) &pIMetaDataImport, &funcToken);
				if(SUCCEEDED(hr))
				{
					mdTypeDef classTypeDef;
					ULONG cchFunction;
					ULONG cchClass;
					PCCOR_SIGNATURE methodSignatureBlob;
					ULONG methodSignatureBlobSize;
					hr = pIMetaDataImport->GetMethodProps(funcToken, &classTypeDef, szName, NAME_BUFFER_SIZE, &cchFunction, 0,
								&methodSignatureBlob, &methodSignatureBlobSize, 0, 0);
					if (SUCCEEDED(hr))
					{
						methodName = wstring(szName, cchFunction);
                        if (CheckMethodName(methodName))
                        {
						    hr = pIMetaDataImport->GetTypeDefProps(classTypeDef, szName, NAME_BUFFER_SIZE, &cchClass, 0, 0);
						    if (SUCCEEDED(hr))
						    {
							    className = wstring(szName, cchClass);
                                if (CheckMethodName(className))
                                {
                                    hr = MnFunctionInfo::ParseFunctionParameters(pIMetaDataImport, funcToken, methodParameters);
                                }
                                else
                                {
                                    hr = E_FAIL;
                                }
                                /*logs::Logger::OutputInfo(_T("%s: Method \"%s\" Class %s Signature %s"), _T(__FUNCTION__), methodName.c_str(), 
                                className.c_str(), methodParameters.c_str());*/
                            }
                        }
                        else
                        {
                            hr = E_FAIL;
                        }
					}
					pIMetaDataImport->Release();
				}
			}
		}
	}
	return hr;
}

void CMnProfiler::EnterHandle(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo, COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo)
{
	MnFunctionMap::iterator iter = m_functionMap.find(functionID);
	if (iter == m_functionMap.end())
	{
		logs::Logger::OutputError(_T("%s FunctionID %u is not found"), _T(__FUNCTION__), functionID);
		return;
	}
    //DebugBreak();
    ThreadID threadId = GetCurrentThread();
    MnPipe::PutIntoPipe(_T("command=start;MethodName=%s.%s;Assembly=%s;UnitProfiles=%s;ManagedThreadId=%u;OSThreadId=%u"), 
        iter->second->GetClassName().c_str(), iter->second->GetFunctionName().c_str(), iter->second->GetAssemblyName().c_str(), 
        iter->second->GetUnitProfiles().c_str(), threadId, GetOSThreadId(threadId));

	//logs::Logger::OutputError(_T("Start: %s.%s  Assembly \"%s\""), iter->second->GetClassName().c_str(), iter->second->GetFunctionName().c_str(), iter->second->GetAssemblyName().c_str());
	iter->second->Enter();
}

void CMnProfiler::LeaveHandle(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo, COR_PRF_FUNCTION_ARGUMENT_RANGE *argumentRange)
{
	MnFunctionMap::iterator iter = m_functionMap.find(functionID);
	if (iter == m_functionMap.end())
	{
		logs::Logger::OutputError(_T("%s FunctionID %u is not found"), _T(__FUNCTION__), functionID);
		return;
	}
	ULONGLONG spend = iter->second->Leave();
    ThreadID threadId = GetCurrentThread();
    MnPipe::PutIntoPipe(_T("command=end;MethodName=%s.%s;Assembly=%s;UnitProfiles=%s;ManagedThreadId=%u;OSThreadId=%u"), 
        iter->second->GetClassName().c_str(), iter->second->GetFunctionName().c_str(), iter->second->GetAssemblyName().c_str(), 
        iter->second->GetUnitProfiles().c_str(), threadId, GetOSThreadId(threadId));
}

void CMnProfiler::TailcallHandle(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo)
{
	MnFunctionMap::iterator iter = m_functionMap.find(functionID);
	if (iter == m_functionMap.end())
	{
		logs::Logger::OutputError(_T("%s FunctionID %u is not found"), _T(__FUNCTION__), functionID);
		return;
	}
	iter->second->Tailcall();
    ThreadID threadId = GetCurrentThread();
    MnPipe::PutIntoPipe(_T("command=end;MethodName=%s.%s;Assembly=%s;UnitProfiles=%s;ManagedThreadId=%u;OSThreadId=%u"), 
        iter->second->GetClassName().c_str(), iter->second->GetFunctionName().c_str(), iter->second->GetAssemblyName().c_str(), 
        iter->second->GetUnitProfiles().c_str(), threadId, GetOSThreadId(threadId));
	/*logs::Logger::OutputError(_T("End: %s.%s Assembly \"%s\" signature %s"), iter->second->GetClassName().c_str(), iter->second->GetFunctionName().c_str(), 
        iter->second->GetAssemblyName().c_str(), iter->second->GetParametrs().c_str());*/
}

ThreadID CMnProfiler::GetCurrentThread()
{
    ThreadID threadId;
    if (m_pICorProfilerInfo2.p == NULL)
    {        
        if (SUCCEEDED(m_pICorProfilerInfo->GetCurrentThreadID(&threadId)))
        {
            return threadId;
        }
    }
    else
    {
        if (SUCCEEDED(m_pICorProfilerInfo2->GetCurrentThreadID(&threadId)))
        {
            return threadId;
        }
    }
    return 0;
}

DWORD CMnProfiler::GetOSThreadId(ThreadID managedThreadID)
{
    if (m_ThreadMap.find(managedThreadID) != m_ThreadMap.end())
    {
        return m_ThreadMap[managedThreadID];
    }
    return 0;
}