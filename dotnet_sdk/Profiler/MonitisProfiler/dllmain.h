// dllmain.h : Declaration of module class.

class CMonitisProfilerModule : public ATL::CAtlDllModuleT< CMonitisProfilerModule >
{
public :
	DECLARE_LIBID(LIBID_MonitisProfilerLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_MONITISPROFILER, "{45ADD53E-98DA-42C6-BC6C-1D6997173F83}")
};

extern class CMonitisProfilerModule _AtlModule;
