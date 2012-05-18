#ifndef NAKED_H
#define NAKED_H

#include <cor.h>
#include <corprof.h>

typedef void (__stdcall *FunctionCallback)(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo, COR_PRF_FUNCTION_ARGUMENT_INFO *argInfo);
typedef void (__stdcall *FunctionTailCallback)(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO frameInfo);

EXTERN_C void EnterNakedProc(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func, COR_PRF_FUNCTION_ARGUMENT_INFO *argumentInfo);
EXTERN_C void LeaveNakedProc(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func, COR_PRF_FUNCTION_ARGUMENT_RANGE *retvalRange);
EXTERN_C void TailNakedProc(FunctionID functionID, UINT_PTR clientData, COR_PRF_FRAME_INFO func);

EXTERN_C void SetHandleProcs (FunctionCallback enter, FunctionCallback leave, FunctionTailCallback tail);
#endif