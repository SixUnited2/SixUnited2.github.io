/*++

Module Name:

    wpptrace.h

Abstract:

    Header file for the debug tracing related function defintions and macros.

--*/
#pragma once

#include <evntrace.h>

//
// To prevent exceptions due to NULL strings
//
#define WPP_CHECK_FOR_NULL_STRING

//
// The GUID of the DLL is
//	 {B1A11339-9A41-4C8C-89C2-973865753CDD}
//   (0xb1a11339, 0x9a41, 0x4c8c, 0x89, 0xc2, 0x97, 0x38, 0x65, 0x75, 0x3c, 0xdd)
//
// We can have up to 32 defines for GUID. More than that, we will have to provide
// another trace control GUID.
//
#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(YzGamingComGuid, (b1a11339,9a41,4c8c,89c2,973865753cdd), \
        WPP_DEFINE_BIT(INIT)                \
        WPP_DEFINE_BIT(CONTRACT)            \
        WPP_DEFINE_BIT(WINDOW)              \
        WPP_DEFINE_BIT(PROFILE)             \
        WPP_DEFINE_BIT(PNP)                 \
        WPP_DEFINE_BIT(WMI)                 \
        WPP_DEFINE_BIT(UPDATE)              \
        )

#define WPP_LEVEL_FLAGS_LOGGER(lvl, flags) WPP_LEVEL_LOGGER(flags)
#define WPP_LEVEL_FLAGS_ENABLED(lvl, flags) (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= lvl)

//
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// USEPREFIX (IfFailGoToExit, "%!STDPREFIX!");
// USEPREFIX (IfFailContinue, "%!STDPREFIX!");
// USEPREFIX (IfFalseGoToExit, "%!STDPREFIX!");
// USEPREFIX (IfFalseErrorGoToExit, "%!STDPREFIX!");
// USEPREFIX (IfFailErrorGoToExit, "%!STDPREFIX!");
// FUNC IfFailErrorGoToExit{LEVEL=TRACE_LEVEL_ERROR}(IfFailErrorGoToExit_EXP,FLAGS,MSG,...);
// FUNC IfFailGoToExit{LEVEL=TRACE_LEVEL_ERROR}(IfFailGoToExit_EXP,FLAGS,MSG,...);
// FUNC IfFailContinue{LEVEL=TRACE_LEVEL_ERROR}(IfFailContinue_EXP,FLAGS,MSG,...);
// FUNC IfFalseGoToExit{LEVEL=TRACE_LEVEL_ERROR}(IfFalseGoToExit_EXP,STATUS,FLAGS,MSG,...);
// FUNC IfFalseErrorGoToExit{LEVEL=TRACE_LEVEL_ERROR}(IfFalseErrorGoToExit_EXP,ERROR,FLAGS,MSG,...);
// FUNC WppTraceFuncEntry(FLAGS);
// FUNC WppTraceFuncExit(FLAGS);
// USESUFFIX (IfFailErrorGoToExit, " with error = %!WINERROR!", dwRetVal);
// USESUFFIX (IfFalseErrorGoToExit, " with error = %!WINERROR!", dwRetVal);
// USESUFFIX (IfFailGoToExit, " with status = %!STATUS!", status);
// USESUFFIX (IfFailContinue, " with status = %!STATUS! but we don't fail %!FUNC! due to it", status);
// USESUFFIX (WppTraceFuncEntry, ">>%!FUNC!");
// USESUFFIX (WppTraceFuncExit, "<<%!FUNC!");
// end_wpp
//

// Map the null flags used by Entry/Exit to a function called FuncTrace
#define WPP_FLAGS_ENABLED(flags) WPP_LEVEL_ENABLED(flags)
#define WPP_FLAGS_LOGGER(flags) WPP_LEVEL_LOGGER(flags)

//
//	IfFailGoToExit(STATUS, FLAGS, MSG, ...)
//	{
//		NTSTATUS _status = STATUS;
//		if (!NT_SUCCESS(_status))
//		{
//			WppTrace(TRACE_LEVEL_ERROR, FLAGS, MSG, ...);
//			goto Exit;
//		}
//	}
//
#define WPP_LEVEL_IfFailGoToExit_EXP_FLAGS_PRE(LEVEL,IfFailGoToExit_EXP,FLAGS) { NTSTATUS _status = (IfFailGoToExit_EXP); if(!NT_SUCCESS(_status)) {
#define WPP_LEVEL_IfFailGoToExit_EXP_FLAGS_POST(LEVEL,IfFailGoToExit_EXP,FLAGS) ; goto Exit; } }
#define WPP_LEVEL_IfFailGoToExit_EXP_FLAGS_ENABLED(LEVEL,IfFailGoToExit_EXP,FLAGS) WPP_LEVEL_FLAGS_ENABLED(LEVEL,FLAGS)
#define WPP_LEVEL_IfFailGoToExit_EXP_FLAGS_LOGGER(LEVEL,IfFailGoToExit_EXP,FLAGS) WPP_LEVEL_FLAGS_LOGGER(LEVEL,FLAGS)

//
//	IfFailContinue(STATUS, FLAGS, MSG, ...)
//	{
//      NTSTATUS _status = STATUS;
//		if (!NT_SUCCESS(_status))
//		{
//			WppTrace(TRACE_LEVEL_ERROR, FLAGS, MSG, ...);
//			STATUS = STATUS_SUCCESS;
//		}
//	}
//
#define WPP_LEVEL_IfFailContinue_EXP_FLAGS_PRE(LEVEL,IfFailContinue_EXP,FLAGS) { NTSTATUS _status = (IfFailContinue_EXP); if(!NT_SUCCESS(_status)) {
#define WPP_LEVEL_IfFailContinue_EXP_FLAGS_POST(LEVEL,IfFailContinue_EXP,FLAGS) ; IfFailContinue_EXP = STATUS_SUCCESS; } }
#define WPP_LEVEL_IfFailContinue_EXP_FLAGS_ENABLED(LEVEL,IfFailContinue_EXP,FLAGS) WPP_LEVEL_FLAGS_ENABLED(LEVEL,FLAGS)
#define WPP_LEVEL_IfFailContinue_EXP_FLAGS_LOGGER(LEVEL,IfFailContinue_EXP,FLAGS) WPP_LEVEL_FLAGS_LOGGER(LEVEL,FLAGS)

//
//	IfFalseGoToExit(RESULT, STATUS, FLAGS, MSG ...)
//	{
//      BOOLEAN _result = RESULT;
//		if (!_result)
//		{
//			WppTrace(TRACE_LEVEL_ERROR, FLAGS, MSG, ...);
//			status = STATUS;
//          goto Exit;
//		}
//	}
//
#define WPP_LEVEL_IfFalseGoToExit_EXP_STATUS_FLAGS_PRE(LEVEL,IfFalseGoToExit_EXP,STATUS,FLAGS) { BOOLEAN _result = (IfFalseGoToExit_EXP); if(!(_result)) {
#define WPP_LEVEL_IfFalseGoToExit_EXP_STATUS_FLAGS_POST(LEVEL,IfFalseGoToExit_EXP,STATUS,FLAGS) ; status = STATUS; goto Exit; } }
#define WPP_LEVEL_IfFalseGoToExit_EXP_STATUS_FLAGS_ENABLED(LEVEL,IfFalseGoToExit_EXP,STATUS,FLAGS) WPP_LEVEL_FLAGS_ENABLED(LEVEL,FLAGS)
#define WPP_LEVEL_IfFalseGoToExit_EXP_STATUS_FLAGS_LOGGER(LEVEL,IfFalseGoToExit_EXP,STATUS,FLAGS) WPP_LEVEL_FLAGS_LOGGER(LEVEL,FLAGS)

//
//	IfFalseErrorGoToExit(RESULT, ERROR, FLAGS, MSG ...)
//	{
//      BOOLEAN _result = RESULT;
//		if (!_result)
//		{
//			WppTrace(TRACE_LEVEL_ERROR, FLAGS, MSG, ...);
//			dwRetVal = ERROR;
//			goto Exit;
//		}
//	}
//
#define WPP_LEVEL_IfFalseErrorGoToExit_EXP_ERROR_FLAGS_PRE(LEVEL,IfFalseErrorGoToExit_EXP,ERROR,FLAGS) { BOOLEAN _result = (IfFalseErrorGoToExit_EXP); if(!(_result)) {
#define WPP_LEVEL_IfFalseErrorGoToExit_EXP_ERROR_FLAGS_POST(LEVEL,IfFalseErrorGoToExit_EXP,ERROR,FLAGS) ; dwRetVal = ERROR; goto Exit; } }
#define WPP_LEVEL_IfFalseErrorGoToExit_EXP_ERROR_FLAGS_ENABLED(LEVEL,IfFalseErrorGoToExit_EXP,ERROR,FLAGS) WPP_LEVEL_FLAGS_ENABLED(LEVEL,FLAGS)
#define WPP_LEVEL_IfFalseErrorGoToExit_EXP_ERROR_FLAGS_LOGGER(LEVEL,IfFalseErrorGoToExit_EXP,ERROR,FLAGS) WPP_LEVEL_FLAGS_LOGGER(LEVEL,FLAGS)

//
//	IfFailErrorGoToExit(ERROR, FLAGS, MSG ...)
//	{
//      DWORD _dwRetVal = ERROR;
//		if (_dwRetVal != ERROR_SUCCESS)
//		{
//			WppTrace(TRACE_LEVEL_ERROR, FLAGS, MSG, ...);
//			goto Exit;
//		}
//	}
//
#define WPP_LEVEL_IfFailErrorGoToExit_EXP_FLAGS_PRE(LEVEL,IfFailErrorGoToExit_EXP,FLAGS) { DWORD _dwRetVal = (IfFailErrorGoToExit_EXP); if(_dwRetVal != ERROR_SUCCESS) {
#define WPP_LEVEL_IfFailErrorGoToExit_EXP_FLAGS_POST(LEVEL,IfFailErrorGoToExit_EXP,FLAGS) ; goto Exit; } }
#define WPP_LEVEL_IfFailErrorGoToExit_EXP_FLAGS_ENABLED(LEVEL,IfFailErrorGoToExit_EXP,FLAGS) WPP_LEVEL_FLAGS_ENABLED(LEVEL,FLAGS)
#define WPP_LEVEL_IfFailErrorGoToExit_EXP_FLAGS_LOGGER(LEVEL,IfFailErrorGoToExit_EXP,FLAGS) WPP_LEVEL_FLAGS_LOGGER(LEVEL,FLAGS)

//
// Define the 'xstr' structure for logging buffer and length pairs
// and the 'log_xstr' function which returns it to create one in-place.
// this enables logging of complex data types.
//
typedef struct xstr { char * _buf; unsigned long _len; } xstr_t;
__inline xstr_t log_xstr(void * p, unsigned long l) { xstr_t xs; xs._buf = (char*)p; xs._len = l; return xs; }

//
// Define the macro required for a hexdump use as:
//
//   Hexdump((FLAG, "%!HEXDUMP!", log_xstr((char *)buffer, buffersize)));
//
#define WPP_LOGHEXDUMP(x) WPP_LOGPAIR(2, &((x)._len)) WPP_LOGPAIR((x)._len, (x)._buf)
