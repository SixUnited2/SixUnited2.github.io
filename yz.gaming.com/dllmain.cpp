// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "inc/wpptrace.h"
#include "dllmain.tmh"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        WPP_INIT_TRACING(NULL);
        WPPTrace(TRACE_LEVEL_VERBOSE, INIT, L"Loading yz.gaming.com.dll...");
        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        WPPTrace(TRACE_LEVEL_VERBOSE, INIT, L"Unloading yz.gaming.com.dll...");
        WPP_CLEANUP();
        break;
    }
    return TRUE;
}

