#pragma once

#define YZGAMINGWMI_API YZGAMINGCOM_API

/// <summary>
/// Initialize the WMI object.
/// </summary>
/// <returns></returns>
YZGAMINGWMI_API
DWORD
YZGamingWmi_Initialize(VOID);

/// <summary>
/// Uninitialize the WMI object.
/// </summary>
/// <returns></returns>
YZGAMINGWMI_API
VOID
YzGamingWmi_Uninitialize(VOID);

/// <summary>
/// Set power mode.
/// </summary>
/// <returns></returns>
YZGAMINGWMI_API
DWORD
YzGamingWmi_SetPowerMode(BYTE modeValue);

/// <summary>
/// Get power mode.
/// </summary>
/// <returns></returns>
YZGAMINGWMI_API
DWORD
YzGamingWmi_GetPowerMode(int* piModeValue);


