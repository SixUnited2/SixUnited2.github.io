using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace yz.gaming.accessoryapp.Api
{
    public class YzCommonApi
    {
        public const int SUCCESS = 0;
        const int BACK_KEY_NUMBER = 2;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Pfn_ProfileDeviceEventHandler(
            ProfileDeviceEventEnmu deviceEvent,
            IntPtr hProfileDevice,
            UIntPtr wParam,
            IntPtr lParam,
            IntPtr lpContext);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Pfn_DeviceUpdateValueCallBack(int value);

        const string DLL_PATH = @"Api/yz.gaming.com.dll";

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_Initialize(
            Pfn_ProfileDeviceEventHandler pfnProfileDeviceEventHandler,
            IntPtr lpContext);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_ReadProfileData(
            IntPtr hProfileDevice,
            IntPtr lpProfileData);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void YZGamingCom_ReleaseProfileDataForRead(
            IntPtr lpProfileData);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_WriteProfileData(
            IntPtr hProfileDevice,
            IntPtr lpProfileData);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void YZGamingCom_Uninitialize();

        #region WIM

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingWmi_Initialize();

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YzGamingWmi_Uninitialize();

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YzGamingWmi_SetPowerMode(byte modeValue);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YzGamingWmi_GetPowerMode(ref int piModeValue);
        #endregion

        #region FwUpdate
        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int YZGamingUpdate_Initialize(
            [MarshalAs(UnmanagedType.LPStr)] String filePath,
            IntPtr caller,
            Pfn_DeviceUpdateValueCallBack pfnUpdateProgress,
            Pfn_DeviceUpdateValueCallBack pfnUpdateStatus);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void YZGamingUpdate_Uninitialize();

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingUpdate_GetDeviceVersionInUpdatePackage();

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingUpdate_Start();

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingUpdate_Stop();
        #endregion

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct YZDpadStruct
        {
            public LightTypeEnum LightType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] LightRGB;        //0:red, 1:green, 2:blue
            public byte LightIntensity;    //0:关闭, 1~5档

            public byte MotorIntensity;    //0:关闭, 1:弱, 2:强
            public byte ControllerMode;    //0:手柄, 1:键鼠
            public byte NsPosition;        //0:XBOX按键布局, 1:NS按键布局

            public byte TurboA;            //0:关闭, 1:开启
            public byte TurboB;            //0:关闭, 1:开启
            public byte TurboX;            //0:关闭, 1:开启
            public byte TurboY;            //0:关闭, 1:开启
            public byte TurboL1;           //0:关闭, 1:开启
            public byte TurboR1;           //0:关闭, 1:开启
            public byte TurboOpen;         //0:关闭, 1:开启

            public byte LStickHeadZoomValue;       //0~20（中心死区%）
            public byte RStickHeadZoomValue;       //0~20（中心死区%）
            public byte LStickSensitivity;         //0~5
            public byte RStickSensitivity;         //0~5
            public byte LTriggerSensitivity;       //0~5
            public byte RTriggerSensitivity;       //0~5
            public byte StickCalibration;          //0:退出摇杆校准, 1:进入摇杆校准
            public byte StickCalibrationSucess;    //0:摇杆校准失败, 1:摇杆校准成功
            public byte TriggerCalibration;        //0:退出扳机校准, 1:进入扳机校准
            public byte TriggerCalibrationSucess;  //0:扳机校准失败, 1:扳机校准成功

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)KeyCodeEnum.MAX_MAPPING_KEYS)]
            public byte[] MkModeKeyData;
            public uint FwVersion;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BACK_KEY_NUMBER, ArraySubType = UnmanagedType.Struct)]
            public BKMACRO[] BkMacro; //M1 + M2
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BKMACROSTEP
        {
            /// <summary>
            /// 映射键码
            /// </summary>
            public byte Key;
            /// <summary>
            /// 0: 键鼠, 1: 手柄
            /// </summary>
            public byte Type;
            /// <summary>
            /// 映射值持续时间，单位为ms
            /// </summary>
            public ushort Time;
            /// <summary>
            /// 与下一步映射值触发时的相隔时间,单位为ms
            /// </summary>
            public ushort Interval;
            /// <summary>
            /// 下一步
            /// </summary>
            public IntPtr Next;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BKMACRO
        {
            /// <summary>
            /// 宏的循环执行标志
            /// </summary>
            public byte CycleFlag;
            /// <summary>
            /// 宏步骤执行完后到重新执行前的相隔时间，单位为ms
            /// </summary>
            public ushort CycleInterval;
            public BKMACROSTEP Step;
        }   

        public enum LightTypeEnum : uint
        { 
            MonoBright,
            MonoBreath,
            RGBBreath,
            RGBCycle,
            RGBWave
        }

        public enum KeyCodeEnum : byte
        {
            [Description("BK")]
            Back = 0x00,
            [Description("L2")]
            L2,
            [Description("L1")]
            L1,
            [Description("D_Up")]
            L3D_UP,
            [Description("D_Down")]
            L3D_DOWN,
            [Description("D_Left")]
            L3D_LEFT,
            [Description("D_Right")]
            L3D_RIGHT,
            [Description("L3")]
            L3,
            [Description("Up")]
            DPAD_UP,
            [Description("Down")]
            DPAD_DOWN,
            [Description("Left")]
            DPAD_LEFT,
            [Description("Right")]
            DPAD_RIGHT,
            [Description("Start")]
            Start,
            [Description("R2")]
            R2,
            [Description("R1")]
            R1,
            [Description("A")]
            A,
            [Description("B")]
            B,
            [Description("X")]
            X,
            [Description("Y")]
            Y,
            [Description("R3")]
            R3,
            [Description("NC")]
            MAX_MAPPING_KEYS,
            [Description("Quick")]
            Quick,
            [Description("M1")]
            M1,
            [Description("M2")]
            M2
        }

        public enum MacroMappingEnum : byte
        {
            [Description("NC")]
            K_NC_Pos = 0xFF,
            [Description("Up")]
            K_UP_Pos = 0,
            [Description("Down")]
            K_DOWN_Pos = 1,
            [Description("Left")]
            K_LEFT_Pos = 2,
            [Description("Right")]
            K_RIGHT_Pos = 3,
            [Description("Start")]
            K_START_Pos = 4,
            [Description("Back")]
            K_BACK_Pos = 5,
            [Description("L3")]
            K_L3_Pos = 6,
            [Description("R3")]
            K_R3_Pos = 7,
            [Description("A")]
            K_A_Pos = 8,
            [Description("B")]
            K_B_Pos = 9,
            [Description("X")]
            K_X_Pos = 10,
            [Description("Y")]
            K_Y_Pos = 11,
            [Description("L1")]
            K_L1_Pos = 12,
            [Description("R1")]
            K_R1_Pos = 13,
            [Description("Home")]
            K_HOME_Pos = 14,
            [Description("L2")]
            K_L2_Pos = 15,
            [Description("R2")]
            K_R2_Pos = 16
        }

        /// <summary>
        /// The profile device events.
        /// </summary>
        public enum ProfileDeviceEventEnmu
        {
            DeviceAdded,
            DeviceRemoved,
            KeyPressed,
            TriggerPressed,
            ThumbPressed,
            ModeChanged,
            VersionArrived
        }

        public enum  TriggerKeyEnmu { Left, Right }

        public enum ThumbEnmu { LX, LY, RX, RY }

        public enum ThumbKeyEnmu { Left, Right }

        public enum ThumbDirectionEnmu { UP, DOWN, LEFT, RIGHT }

        public enum KeyEventTypeEnmu { UP, DOWN }

        public enum KeyPressTypeEnmu { ShortPress, LongPress, Ignore, AppClick }
    }
}
