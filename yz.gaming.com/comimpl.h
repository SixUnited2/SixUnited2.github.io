#pragma once

#define BACK_KEY_NUMBER  2

typedef enum : BYTE
{
    Back,
    L2,
    L1,
    L3D_UP,
    L3D_DOWN,
    L3D_LEFT,
    L3D_RIGHT,
    L3,
    DPAD_UP,
    DPAD_DOWN,
    DPAD_LEFT,
    DPAD_RIGHT,
    Start,
    R2,
    R1,
    A,
    B,
    X,
    Y,
    R3,
    MAX_MAPPING_KEYS,
    Quick,
    M1,
    M2,
} KEYCODE;

#pragma pack(push, 1)
typedef struct
{
    enum LIGHTTYPE : DWORD { MonoBright, MonoBreath, RGBBreath, RGBCycle, RGBWave } lightType;
    BYTE lightRGB[3];       //0:red, 1:green, 2:blue
    BYTE lightIntensity;    //0:关闭, 1~5档

    BYTE motorIntensity;    //0:关闭, 1:弱, 2:强
    BYTE controllerMode;    //0:手柄, 1:键鼠
    BYTE nsPosition;        //0:XBOX按键布局, 1:NS按键布局

    BYTE turboA;            //0:关闭, 1:开启
    BYTE turboB;            //0:关闭, 1:开启
    BYTE turboX;            //0:关闭, 1:开启
    BYTE turboY;            //0:关闭, 1:开启
    BYTE turboL1;           //0:关闭, 1:开启
    BYTE turboR1;           //0:关闭, 1:开启
    BYTE turboOpen;         //0:关闭, 1:开启

    BYTE lStickDeadZoneValue;       //0~20（中心死区%）
    BYTE rStickDeadZoneValue;       //0~20（中心死区%）
    BYTE lStickSensitivity;         //0~5
    BYTE rStickSensitivity;         //0~5
    BYTE lTriggerSensitivity;       //0~5
    BYTE rTriggerSensitivity;       //0~5
    BYTE stickCalibration;          //0:退出摇杆校准, 1:进入摇杆校准
    BYTE stickCalibrationSucess;    //0:摇杆校准失败, 1:摇杆校准成功
    BYTE triggerCalibration;        //0:退出扳机校准, 1:进入扳机校准
    BYTE triggerCalibrationSucess;  //0:扳机校准失败, 1:扳机校准成功

    KEYCODE keys[MAX_MAPPING_KEYS];

    DWORD fwVersion;                // main version high | main version low | sub version high | sub version low

    struct BKMACRO
    {
        BYTE   cycleFlag;           //宏的循环执行标志
        USHORT cycleInterval;       //宏步骤执行完后到重新执行前的相隔时间，单位为ms

        struct BKMACROSTEP
        {
            BYTE   key;             //映射键码
            BYTE   type;            //0: 键鼠, 1: 手柄
            USHORT time;            //映射值持续时间，单位为ms
            USHORT interval;        //与下一步映射值触发时的相隔时间,单位为ms
            BKMACROSTEP* next;      //下一步
        } step;
    } bkMacro[BACK_KEY_NUMBER];     //M1 + M2
} PROFILEDATA, *LPPROFILEDATA;
#pragma pack(pop)

typedef void* HPROFILEDEVICE;
typedef HPROFILEDEVICE* PHPROFILEDEVICE;

/// <summary>
/// The profile device events.
/// </summary>
typedef enum 
{
    DeviceAdded,    // wParam: null, lParam: null
    DeviceRemoved,  // wParam: null, lParam: null
    KeyPressed,     // wParam: enum KEYCODE, lParam: 0-released 1-pressed
    TriggerPressed, // wParam: enum Tigger, lParam: BYTE
    ThumbPressed,   // wParam: enum Thumb, lParam: SHORT
    ModeChanged,    // wParam: source enum ControllerMode, lParam: target enum ControllerMode
    VersionArrived  // wParam: Major version, lParam: Minor version
} ProfileDeviceEvent;

typedef enum { Left, Right } Trigger;
typedef enum { LX, LY, RX, RY } Thumb;

/// <summary>
/// The profile device event handler.
/// </summary>
/// <param name="deviceEvent">The profile device events.</param>
/// <param name="hProfileDevice">The profile device handle.</param>
/// <param name="wParam">WPARAM type value which is subject to be changed according to the event.</param>
/// <param name="lParam">LPARAM type value which is subject to be changed according to the event.</param>
/// <param name="lpContext">The context pointer passed by call YZGamingCom_Initialize.</param>
typedef void (CALLBACK* PFN_PROFILEDEVICEEVENTHANDLER)(
    ProfileDeviceEvent  deviceEvent,
    HPROFILEDEVICE      hProfileDevice,
    WPARAM              wParam,
    LPARAM              lParam,
    LPVOID              lpContext
    );

/// <summary>
/// Initialize the profile device communication. The profile device is notified via the callback function.
/// </summary>
/// <param name="pfnProfileDeviceEventHandler">The event handler of the profile device events.</param>
/// <returns></returns>
YZGAMINGCOM_API
DWORD
YZGamingCom_Initialize(
    PFN_PROFILEDEVICEEVENTHANDLER pfnProfileDeviceEventHandler,
    LPVOID                        lpContext
    );

/// <summary>
/// Read the profile data from the profile device.
/// </summary>
/// <param name="hProfileDevice">The profile device handle.</param>
/// <param name="lpProfileData">The profile data pointer. It can't be null.</param>
/// <returns></returns>
YZGAMINGCOM_API
DWORD
YZGamingCom_ReadProfileData(
    HPROFILEDEVICE hProfileDevice,
    LPPROFILEDATA lpProfileData
    );

/// <summary>
/// The profile data retured by calling YZGamingCom_ReadProfileData must be released by
/// calling this function.
/// </summary>
/// <param name="lpProfileData">The profile data pointer returned by calling YZGamingCom_ReadProfileData.</param>
/// <returns></returns>
YZGAMINGCOM_API
VOID
YZGamingCom_ReleaseProfileDataForRead(
    LPPROFILEDATA lpProfileData
    );

/// <summary>
/// Write the profile data to the profile device.
/// </summary>
/// <param name="hProfileDevice">The profile device handle.</param>
/// <param name="lpProfileData">The profile data pointer. It can't be null.</param>
/// <returns></returns>
YZGAMINGCOM_API
DWORD
YZGamingCom_WriteProfileData(
    HPROFILEDEVICE hProfileDevice,
    LPPROFILEDATA lpProfileData
    );

/// <summary>
/// Uninitialize the profile device communication
/// </summary>
YZGAMINGCOM_API
VOID
YZGamingCom_Uninitialize(
    VOID
    );