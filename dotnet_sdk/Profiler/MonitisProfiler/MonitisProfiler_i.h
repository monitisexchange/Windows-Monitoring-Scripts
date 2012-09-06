

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Tue Sep 04 17:30:26 2012
 */
/* Compiler settings for MonitisProfiler.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __MonitisProfiler_i_h__
#define __MonitisProfiler_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IMnProfiler_FWD_DEFINED__
#define __IMnProfiler_FWD_DEFINED__
typedef interface IMnProfiler IMnProfiler;
#endif 	/* __IMnProfiler_FWD_DEFINED__ */


#ifndef __MnProfiler_FWD_DEFINED__
#define __MnProfiler_FWD_DEFINED__

#ifdef __cplusplus
typedef class MnProfiler MnProfiler;
#else
typedef struct MnProfiler MnProfiler;
#endif /* __cplusplus */

#endif 	/* __MnProfiler_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "CorProf.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IMnProfiler_INTERFACE_DEFINED__
#define __IMnProfiler_INTERFACE_DEFINED__

/* interface IMnProfiler */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IMnProfiler;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("73ADAB0B-8F1F-498A-8FFF-A186628BA530")
    IMnProfiler : public IUnknown
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct IMnProfilerVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IMnProfiler * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IMnProfiler * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IMnProfiler * This);
        
        END_INTERFACE
    } IMnProfilerVtbl;

    interface IMnProfiler
    {
        CONST_VTBL struct IMnProfilerVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IMnProfiler_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IMnProfiler_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IMnProfiler_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IMnProfiler_INTERFACE_DEFINED__ */



#ifndef __MonitisProfilerLib_LIBRARY_DEFINED__
#define __MonitisProfilerLib_LIBRARY_DEFINED__

/* library MonitisProfilerLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_MonitisProfilerLib;

EXTERN_C const CLSID CLSID_MnProfiler;

#ifdef __cplusplus

class DECLSPEC_UUID("71EDB19D-4F69-4A2C-A2F5-BE783F543A7E")
MnProfiler;
#endif
#endif /* __MonitisProfilerLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


