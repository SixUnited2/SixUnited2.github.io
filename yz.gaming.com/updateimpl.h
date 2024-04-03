#pragma once

#define YZGAMINGUPDATE_API YZGAMINGCOM_API

/// <summary>
/// The device update progress and status callback.
/// </summary>
/// <param name="caller">The caller defined context.</param>
/// <param name="value">Progress or status value.
/// For status value:
///             0: 升级成功 ,负数表示升级失败，
///            -1: 升级包与设备不匹配
///            -2：升级包不存在
///            -3: Device version Read Error
///            -4: Device Rom Erase error
///            -5: Device Rom Update error
///            -6: Device Rom Erase error
///            -7: Device Rom Update error
///            -8: Device Rom Write error
///            -9: Device Checksum Read error
///            -10:Checksum error
///            -11:Device Rom Erase error
///            -12:Device Rom Update error
///            -13:Device Rom Write error
///            -14:Device Checksum Read error
///            -15:Checksum error
///            -16:Device Reset failed
///            -17:Accidental device removal
/// </param>
typedef VOID (CALLBACK *PFN_DEVICEUPDATEVALUECB)(
    int    value
    );

/// <summary>
/// Initialize the device update context 
/// </summary>
/// <param name="filePath">The firmware binary file in full path.</param>
/// <param name="caller">The caller defined context.</param>
/// <param name="pfnUpdateProgress">The device update progress callback function.</param>
/// <param name="pfnUpdateStatus">The device update status callback function.</param>
/// <returns>ERROR_SUCCESS: success, Otherwise: fail</returns>
YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Initialize(
    LPCSTR                  filePath,
    LPVOID                  caller,
    PFN_DEVICEUPDATEVALUECB pfnUpdateProgress,
    PFN_DEVICEUPDATEVALUECB pfnUpdateStatus
    );

/// <summary>
/// Uninitialize the device update context. Must stop updating before calling this.
/// </summary>
/// <returns>ERROR_SUCCESS: success, Otherwise: fail</returns>
YZGAMINGUPDATE_API
VOID
YZGamingUpdate_Uninitialize(VOID);

/// <summary>
/// Get device version contained in the update package
/// </summary>
/// <returns>-1: fail, >=0: otherwise</returns>
YZGAMINGUPDATE_API
int
YZGamingUpdate_GetDeviceVersionInUpdatePackage(VOID);

/// <summary>
/// Start device update
/// </summary>
/// <returns>ERROR_SUCCESS: success, Otherwise: fail</returns>
YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Start(VOID);

/// <summary>
/// Stop device update
/// </summary>
/// <returns>ERROR_SUCCESS: success, Otherwise: fail</returns>
YZGAMINGUPDATE_API
DWORD
YZGamingUpdate_Stop(VOID);
