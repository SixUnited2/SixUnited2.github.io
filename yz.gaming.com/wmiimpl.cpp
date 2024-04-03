#include "pch.h"
#include "wmiimpl.h"
#include "wmiinfo.h"

using namespace YZ::Gaming::Com;

namespace
{
    unique_ptr<CWmiInfo> ptrWmiInfo;
}

YZGAMINGWMI_API
DWORD
YZGamingWmi_Initialize(VOID)
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (bool{ ptrWmiInfo })
    {
        dwRetVal = ERROR_ALREADY_INITIALIZED;
        return dwRetVal;
    }

    ptrWmiInfo = make_unique<CWmiInfo>();
    if (!bool{ ptrWmiInfo })
    {
        dwRetVal = ERROR_CREATE_FAILED;
        return dwRetVal;
    }

    return dwRetVal;
}

YZGAMINGWMI_API
VOID
YzGamingWmi_Uninitialize(VOID)
{
    if (bool{ ptrWmiInfo })
    {
        ptrWmiInfo.reset();
    }
}

YZGAMINGWMI_API
DWORD
YzGamingWmi_SetPowerMode(BYTE modeValue)
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (!bool{ ptrWmiInfo })
    {
        dwRetVal = ERROR_NOT_READY;
        return dwRetVal;
    }

    HRESULT hr = ptrWmiInfo->SetPowerMode(modeValue);
    if (FAILED(hr))
    {
        dwRetVal = hr;
        return dwRetVal;
    }

    return dwRetVal;
}

YZGAMINGWMI_API
DWORD
YzGamingWmi_GetPowerMode(int* piModeValue)
{
    DWORD dwRetVal = ERROR_SUCCESS;

    if (piModeValue == nullptr)
    {
        dwRetVal = ERROR_INVALID_PARAMETER;
        return dwRetVal;
    }

    if (!bool{ ptrWmiInfo })
    {
        dwRetVal = ERROR_NOT_READY;
        return dwRetVal;
    }

    *piModeValue = ptrWmiInfo->GetPowerMode();
    
    return dwRetVal;
}
