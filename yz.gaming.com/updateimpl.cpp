#include "pch.h"
#include "updateimpl.h"
#include "inc/updatemanager.h"
#include "inc/wpptrace.h"
#include "hidutils.h"
#include "updateimpl.tmh"

#ifdef _AMD64_
    #ifdef _DEBUG
        #pragma comment(lib, ".\\lib\\x64\\CUpdateManager.lib")
    #else
        #pragma comment(lib, ".\\lib\\x64\\CUpdateManager.lib")
    #endif
#else
    #ifdef _DEBUG
        #pragma comment(lib, ".\\lib\\x86\\debug\\CUpdateManager.lib")
    #else
        #pragma comment(lib, ".\\lib\\x86\\release\\CUpdateManager.lib")
    #endif
#endif

using namespace YZ::Gaming::Com;

namespace
{
    typedef struct tagDEVICEUPDATECONTEXT
    {
        HANDLE                  updateThread;
        LPVOID                  caller;
        PFN_DEVICEUPDATEVALUECB updateProgress;
        PFN_DEVICEUPDATEVALUECB updateStatus;
    } DEVICEUPDATECONTEXT, *LPDEVICEUPDATECONTEXT;

    unique_ptr<DEVICEUPDATECONTEXT> ptrDeviceUpdateContext;
    CUpdateManager updateManager;

    VOID UpdateProgress(LPVOID caller, int progress);
    VOID UpdateStatus(LPVOID caller, int status);
    HANDLE OpenDeviceForBootLoad(VOID);
    BOOL BootLoadUpdateDevice(HANDLE hDevice);
    DWORD WINAPI UpdateThreadRontine(LPVOID lpParameter);
}

YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Initialize(
    LPCSTR                  filePath,
    LPVOID                  caller,
    PFN_DEVICEUPDATEVALUECB pfnUpdateProgress,
    PFN_DEVICEUPDATEVALUECB pfnUpdateStatus
    )
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (filePath == nullptr)
    {
        dwRetVal = ERROR_INVALID_PARAMETER;
        return dwRetVal;
    }

    if (bool{ ptrDeviceUpdateContext })
    {
        dwRetVal = ERROR_ALREADY_INITIALIZED;
        return dwRetVal;
    }

    ptrDeviceUpdateContext = make_unique<DEVICEUPDATECONTEXT>();
    if (!bool{ ptrDeviceUpdateContext })
    {
        WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"Failed in instantiating the device update context");
        dwRetVal = ERROR_CREATE_FAILED;
        return dwRetVal;
    }

    ptrDeviceUpdateContext->caller = caller;
    ptrDeviceUpdateContext->updateProgress = pfnUpdateProgress;
    ptrDeviceUpdateContext->updateStatus = pfnUpdateStatus;

    if (!updateManager.initParam(const_cast<LPSTR>(filePath),
                                 ptrDeviceUpdateContext.get(),
                                 UpdateProgress,
                                 UpdateStatus))
    {
        dwRetVal = ERROR_DLL_INIT_FAILED;
        WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"Failed in calling CUpdateManager::initParam");

        ptrDeviceUpdateContext.reset();
    }

    return dwRetVal;
}

YZGAMINGUPDATE_API
VOID
YZGamingUpdate_Uninitialize(VOID)
{
    if (bool{ ptrDeviceUpdateContext })
    {
        YZGamingUpdate_Stop();

        ptrDeviceUpdateContext.reset();
    }
}

YZGAMINGUPDATE_API
int
YZGamingUpdate_GetDeviceVersionInUpdatePackage(VOID)
{
    int version = -1;

    if (bool{ ptrDeviceUpdateContext })
    {
        version = updateManager.getDeviceVersionInUpdatePackage();
        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"The device version in update package is 0x%x", version);
    }
    else
    {
        WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"The device update context is not initialized");
    }

    return version;
}

YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Start(VOID)
{
    DWORD retVal = ERROR_SUCCESS;

    if (bool{ ptrDeviceUpdateContext })
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"Launch the device update thread");

        DWORD threadId = 0;
        HANDLE hThread = ::CreateThread(
            nullptr,
            0,
            UpdateThreadRontine,
            ptrDeviceUpdateContext.get(),
            0,
            &threadId);
        if (hThread != nullptr)
        {
            ptrDeviceUpdateContext->updateThread = hThread;
            WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"The device update thread is launched: %d", threadId);
        }
        else
        {
            retVal = ::GetLastError();
            WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"Failed in calling CreateThread due to %!WINERROR!", retVal);
        }
    }
    else
    {
        retVal = ERROR_NOT_READY;
        WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"The device update context is not initialized");
    }

    return retVal;
}

YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Stop(VOID)
{
    DWORD retVal = ERROR_SUCCESS;

    if (bool{ ptrDeviceUpdateContext })
    {
        HANDLE hThread{ ptrDeviceUpdateContext->updateThread };

        if (DWORD exitCode = 0;
            ::GetExitCodeThread(hThread, &exitCode) != FALSE)
        {
            WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"The thread exit code: %d", exitCode);

            if (exitCode == STILL_ACTIVE)
            {
                //CUpdateManager::stopUpdate();

                if (::WaitForSingleObject(hThread, 1000) == WAIT_TIMEOUT)
                {
                    WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"Can't stop the thread in 1s, terminate it!");
                    ::TerminateThread(hThread, -1);
                }                
            }
        }
        else
        {
            retVal = ::GetLastError();
            WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"Failed in calling GetExitCodeThread due to %!WINERROR!", retVal);
        }

        ::CloseHandle(hThread);
    }
    else
    {
        retVal = ERROR_NOT_READY;
        WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"The device update context is not initialized");
    }

    return retVal;
}

namespace
{
    VOID UpdateProgress(LPVOID caller, int progress)
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L">> UpdateProgress: %d", progress);

        try
        {
            LPDEVICEUPDATECONTEXT pDeviceUpdateContext = reinterpret_cast<LPDEVICEUPDATECONTEXT>(caller);
            if (pDeviceUpdateContext != nullptr && pDeviceUpdateContext->updateProgress != nullptr)
            {
                pDeviceUpdateContext->updateProgress(progress);
            }
        }
        catch (exception const& ex)
        {
            WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"!!!Exception while updateProgress: %s", ex.what());
        }

        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"<< UpdateProgress");
    }

    VOID UpdateStatus(LPVOID caller, int status)
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L">> UpdateStatus: %d", status);

        try
        {
            LPDEVICEUPDATECONTEXT pDeviceUpdateContext = reinterpret_cast<LPDEVICEUPDATECONTEXT>(caller);
            if (pDeviceUpdateContext != nullptr && pDeviceUpdateContext->updateStatus != nullptr)
            {
                pDeviceUpdateContext->updateStatus(status);
            }
        }
        catch (exception const& ex)
        {
            WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"!!!Exception while updateStatus: %s", ex.what());
        }

        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"<< UpdateStatus");
    }

    HANDLE OpenDeviceForBootLoad(VOID)
    {
        HANDLE h = nullptr;

        auto hidDevices{ HidUtils::EnumerateHIDDeviceNames() };
        for (auto const& hidDevice : hidDevices)
        {
            HANDLE hDevice = ::CreateFile(hidDevice.c_str(),
                                          GENERIC_READ | GENERIC_WRITE,
                                          FILE_SHARE_READ | FILE_SHARE_WRITE,
                                          nullptr,
                                          OPEN_EXISTING,
                                          FILE_ATTRIBUTE_NORMAL,
                                          nullptr);
            if (hDevice != INVALID_HANDLE_VALUE)
            {   
                if (HIDD_ATTRIBUTES hidAttributes;
                    ::HidD_GetAttributes(hDevice, &hidAttributes) == TRUE)
                {
                    if (hidAttributes.VendorID == CUpdateManager::Vid &&
                        hidAttributes.ProductID == CUpdateManager::Gp_Pid)
                    {
                        if (PHIDP_PREPARSED_DATA pHidpPreparedData = nullptr;
                            ::HidD_GetPreparsedData(hDevice, &pHidpPreparedData) == TRUE)
                        {
                            if (HIDP_CAPS hidpCaps;
                                ::HidP_GetCaps(pHidpPreparedData, &hidpCaps) == HIDP_STATUS_SUCCESS)
                            {
                                if (hidpCaps.Usage == 0x0001 && hidpCaps.UsagePage == 0xff00)
                                {
                                    ::HidD_FreePreparsedData(pHidpPreparedData);

                                    WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"Find the device to be boot load: %ws", hidDevice.c_str());
                                    
                                    h = hDevice;
                                    break;
                                }
                            }

                            ::HidD_FreePreparsedData(pHidpPreparedData);
                        }
                    }
                }

                ::CloseHandle(hDevice);
            }
        }

        return h;
    }

    BOOL BootLoadUpdateDevice(HANDLE hDevice)
    {
        UCHAR rdBuf[MAX_PATH];
        memset(rdBuf, 0, sizeof(rdBuf));

        if (::HidD_GetIndexedString(hDevice, 0xFE, &rdBuf, (0x10 - 2 + 1)) == FALSE)
        {
            return FALSE;
        }

        if (::HidD_GetIndexedString(hDevice, 0xFE, &rdBuf, (0x10 - 2 + 2)) == FALSE)
        {
            return FALSE;
        }

        if (::HidD_GetIndexedString(hDevice, 0xFE, &rdBuf, (0x10 - 2 + 3)) == FALSE)
        {
            return FALSE;
        }

        if (::HidD_GetIndexedString(hDevice, 0xFE, &rdBuf, (0x10 - 2 + 4)) == FALSE)
        {
            return FALSE;
        }
                
        return TRUE;
    }

    DWORD WINAPI UpdateThreadRontine(LPVOID lpParameter)
    {
        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L">> The device update thread starts");

        try
        {
            LPDEVICEUPDATECONTEXT pDeviceUpdateContext = reinterpret_cast<LPDEVICEUPDATECONTEXT>(lpParameter);
            if (pDeviceUpdateContext != nullptr)
            {
                // If the device is in the update status, the device for boot load can't be found.
                // In this case, directly start updating. 
                if (HANDLE hDevice = OpenDeviceForBootLoad();
                    hDevice != nullptr)
                {
                    WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"Boot loading the device for update");

                    if (BootLoadUpdateDevice(hDevice) == FALSE)
                    {
                        BootLoadUpdateDevice(hDevice);
                    }

                    ::CloseHandle(hDevice);
                    ::Sleep(1000);
                }
                else
                {
                    WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"Can't find the device for boot load, the device can be in the updating status. Start updating directly in this case");
                }

                updateManager.startUpdate();
            }
        }
        catch (exception const& ex)
        {
            WPPTrace(TRACE_LEVEL_ERROR, UPDATE, L"!!!Exception while calling CUpdateManager::startUpdate: %s", ex.what());
        }

        WPPTrace(TRACE_LEVEL_INFORMATION, UPDATE, L"<< The device update thread is done and quits");

        return 0L;
    }
}