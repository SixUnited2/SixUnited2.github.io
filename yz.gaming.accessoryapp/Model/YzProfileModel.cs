using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using yz.gaming.accessoryapp.Api;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.Model
{
    public class YzProfileModel
    {
        public delegate void ProfileRereshHandler(YzProfileModel model);

        public event ProfileRereshHandler OnProfileReresh;

        /// <summary>
        /// 灯光类型
        /// </summary>
        public LightTypeEnum LightType { get; set; }

        /// <summary>
        /// 灯光颜色
        /// </summary>
        public Color LightColor { get; set; }

        /// <summary>
        /// 灯光亮度
        /// 0:关闭, 1~5档
        /// </summary>
        public byte LightIntensity { get; set; }

        /// <summary>
        /// 震动强度
        /// 0:关闭, 1:弱, 2:强
        /// </summary>
        public byte MotorIntensity { get; set; } = 1;

        /// <summary>
        /// 控制模式
        /// 0:手柄, 1:键鼠
        /// </summary>
        public byte ControllerMode { get; set; }

        /// <summary>
        /// 按键布局
        /// 0:XBOX按键布局, 1:NS按键布局
        /// </summary>
        public byte KeyLayout { get; set; }

        /// <summary>
        /// 连发
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboOpen { get; set; }

        /// <summary>
        /// 连发 A键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboA { get; set; }

        /// <summary>
        /// 连发 B键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboB { get; set; }

        /// <summary>
        /// 连发 X键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboX { get; set; }

        /// <summary>
        /// 连发 Y键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboY { get; set; }

        /// <summary>
        /// 连发 L1键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboL1 { get; set; }

        /// <summary>
        /// 连发 L2键
        /// //0:关闭, 1:开启
        /// </summary>
        public byte TurboR1 { get; set; }

        /// <summary>
        /// 中心死区 左摇杆
        /// 0~20（中心死区%）
        /// </summary>
        public byte LStickHeadZoomValue { get; set; }

        /// <summary>
        /// 中心死区 右摇杆
        /// 0~20（中心死区%）
        /// </summary>
        public byte RStickHeadZoomValue { get; set; }

        /// <summary>
        /// 灵敏度 左摇杆
        /// 0~4
        /// </summary>
        public byte LStickSensitivity { get; set; }

        /// <summary>
        /// 灵敏度 右摇杆
        /// 0~4
        /// </summary>
        public byte RStickSensitivity { get; set; }

        /// <summary>
        /// 灵敏度 左扳机
        /// 0~4
        /// </summary>
        public byte LTriggerSensitivity { get; set; }

        /// <summary>
        /// 灵敏度 右扳机
        /// 0~4
        /// </summary>
        public byte RTriggerSensitivity { get; set; }

        /// <summary>
        /// 摇杆校准
        /// 0:退出摇杆校准, 1:进入摇杆校准
        /// </summary>
        public byte StickCalibration { get; set; }

        /// <summary>
        /// 摇杆校准结果
        /// 0:失败, 1:成功
        /// </summary>
        public byte StickCalibrationSucess { get; set; }

        /// <summary>
        /// 扳机校准
        /// 0:退出扳机校准, 1:进入扳机校准
        /// </summary>
        public byte TriggerCalibration { get; set; }

        /// <summary>
        /// 扳机校准结果
        /// 0:失败, 1:成功
        /// </summary>
        public byte TriggerCalibrationSucess { get; set; }

        public byte[] MkModeKeyData { get; set; }

        public Version Version { get; set; }

        public BackKeyMacro BackKey { get; set; }

        YZDpadStruct _OriData;

        public YzProfileModel()
        {
            _OriData.LightRGB = new byte[] { 0, 0, 0 };
            _OriData.MkModeKeyData = new byte[] {
                (byte)KeyCodeEnum.Back,
                (byte)KeyCodeEnum.L2,
                (byte)KeyCodeEnum.L1,
                (byte)KeyCodeEnum.L3D_UP,
                (byte)KeyCodeEnum.L3D_DOWN,
                (byte)KeyCodeEnum.L3D_LEFT,
                (byte)KeyCodeEnum.L3D_RIGHT,
                (byte)KeyCodeEnum.L3,
                (byte)KeyCodeEnum.DPAD_UP,
                (byte)KeyCodeEnum.DPAD_DOWN,
                (byte)KeyCodeEnum.DPAD_LEFT,
                (byte)KeyCodeEnum.DPAD_RIGHT,
                (byte)KeyCodeEnum.Start,
                (byte)KeyCodeEnum.R2,
                (byte)KeyCodeEnum.R1,
                (byte)KeyCodeEnum.A,
                (byte)KeyCodeEnum.B,
                (byte)KeyCodeEnum.X,
                (byte)KeyCodeEnum.Y,
                (byte)KeyCodeEnum.R3,
                (byte)KeyCodeEnum.Quick, };
            _OriData.BkMacro = new BKMACRO[2]
            {
                new BKMACRO()
                {
                    CycleFlag = 0x00,
                    CycleInterval = 0x00,
                    Step = new BKMACROSTEP()
                    {
                        Key = (byte)KeyCodeEnum.MAX_MAPPING_KEYS,
                        Type = 0,
                        Time = 0x00,
                        Interval = 0x00,
                        Next = IntPtr.Zero
                    }
                },
                new BKMACRO()
                {
                    CycleFlag = 0x00,
                    CycleInterval = 0x00,
                    Step = new BKMACROSTEP()
                    {
                        Key = (byte)KeyCodeEnum.MAX_MAPPING_KEYS,
                        Type = 0,
                        Time = 0x00,
                        Interval = 0x00,
                        Next = IntPtr.Zero
                    }
                },
            };

            BackKey = new BackKeyMacro();
        }

        public void OnProfileChanged(YZDpadStruct dataStruct)
        {
            SetData(dataStruct);
        }

        public void SetControllerMode(byte mode)
        {
            ControllerMode = mode;
            OnProfileReresh?.Invoke(this);
        }

        //public static YzProfileModel FormStruct(YZDpadStruct dataStruct)
        //{
        //    return new YzProfileModel()
        //    {
        //        LightType = dataStruct.LightType,
        //        LightColor = Color.FromRgb(dataStruct.LightRGB[0], dataStruct.LightRGB[1], dataStruct.LightRGB[2]),
        //        LightIntensity = dataStruct.LightIntensity,

        //        MotorIntensity = dataStruct.MotorIntensity,    //0:关闭, 1:弱, 2:强
        //        ControllerMode = dataStruct.ControllerMode,    //0:手柄, 1:键鼠
        //        KeyLayout = dataStruct.NsPosition,             //0:XBOX按键布局, 1:NS按键布局

        //        TurboOpen = dataStruct.TurboOpen,       //0:关闭, 1:开启
        //        TurboA = dataStruct.TurboA,             //0:关闭, 1:开启
        //        TurboB = dataStruct.TurboB,             //0:关闭, 1:开启
        //        TurboX = dataStruct.TurboX,             //0:关闭, 1:开启
        //        TurboY = dataStruct.TurboY,             //0:关闭, 1:开启
        //        TurboL1 = dataStruct.TurboL1,           //0:关闭, 1:开启
        //        TurboR1 = dataStruct.TurboR1,           //0:关闭, 1:开启

        //        LStickHeadZoomValue = dataStruct.LStickHeadZoomValue,           //0~20（中心死区%）
        //        RStickHeadZoomValue = dataStruct.RStickHeadZoomValue,           //0~20（中心死区%）
        //        LStickSensitivity = dataStruct.LStickSensitivity,               //0~5
        //        RStickSensitivity = dataStruct.RStickSensitivity,               //0~5
        //        LTriggerSensitivity = dataStruct.LTriggerSensitivity,           //0~5
        //        RTriggerSensitivity = dataStruct.RTriggerSensitivity,           //0~5
        //        StickCalibration = dataStruct.StickCalibration,                 //0:退出摇杆校准, 1:进入摇杆校准
        //        StickCalibrationSucess = dataStruct.StickCalibrationSucess,     //0:摇杆校准失败, 1:摇杆校准成功
        //        TriggerCalibration = dataStruct.TriggerCalibration,             //0:退出扳机校准, 1:进入扳机校准
        //        TriggerCalibrationSucess = dataStruct.TriggerCalibrationSucess, //0:扳机校准失败, 1:扳机校准成功
        //        MkModeKeyData = dataStruct.MkModeKeyData,

        //        _OriData = dataStruct
        //    };
        //}

        public void SetData(YZDpadStruct dataStruct)
        {
            LightType = dataStruct.LightType;
            LightColor = Color.FromRgb(dataStruct.LightRGB[0], dataStruct.LightRGB[1], dataStruct.LightRGB[2]);
            LightIntensity = dataStruct.LightIntensity;

            MotorIntensity = dataStruct.MotorIntensity;    //0:关闭, 1:弱, 2:强
            ControllerMode = dataStruct.ControllerMode;    //0:手柄, 1:键鼠
            KeyLayout = dataStruct.NsPosition;             //0:XBOX按键布局, 1:NS按键布局

            TurboOpen = dataStruct.TurboOpen;       //0:关闭, 1:开启
            TurboA = dataStruct.TurboA;             //0:关闭, 1:开启
            TurboB = dataStruct.TurboB;             //0:关闭, 1:开启
            TurboX = dataStruct.TurboX;             //0:关闭, 1:开启
            TurboY = dataStruct.TurboY;             //0:关闭, 1:开启
            TurboL1 = dataStruct.TurboL1;           //0:关闭, 1:开启
            TurboR1 = dataStruct.TurboR1;           //0:关闭, 1:开启

            LStickHeadZoomValue = dataStruct.LStickHeadZoomValue;           //0~20（中心死区%）
            RStickHeadZoomValue = dataStruct.RStickHeadZoomValue;           //0~20（中心死区%）
            LStickSensitivity = dataStruct.LStickSensitivity;               //0~5
            RStickSensitivity = dataStruct.RStickSensitivity;               //0~5
            LTriggerSensitivity = dataStruct.LTriggerSensitivity;           //0~5
            RTriggerSensitivity = dataStruct.RTriggerSensitivity;           //0~5
            StickCalibration = dataStruct.StickCalibration;                 //0:退出摇杆校准, 1:进入摇杆校准
            StickCalibrationSucess = dataStruct.StickCalibrationSucess;     //0:摇杆校准失败, 1:摇杆校准成功
            TriggerCalibration = dataStruct.TriggerCalibration;             //0:退出扳机校准, 1:进入扳机校准
            TriggerCalibrationSucess = dataStruct.TriggerCalibrationSucess; //0:扳机校准失败, 1:扳机校准成功
            MkModeKeyData = dataStruct.MkModeKeyData;

            var tmp = BitConverter.GetBytes(dataStruct.FwVersion);
            var minor = Convert.ToInt32(tmp[1].ToString("X2"));
            var major = Convert.ToInt32(tmp[0].ToString("X2"));
            Version = new Version(major, minor);

            if (BackKey == null)
            {
                BackKey = new BackKeyMacro();
            }

            // M1
            BackKey.M2.CycleFlag = dataStruct.BkMacro[0].CycleFlag;
            BackKey.M2.CycleInterval = dataStruct.BkMacro[0].CycleInterval;
            MacroStep M2 = new MacroStep
            {
                Key = dataStruct.BkMacro[0].Step.Key,
                Type = dataStruct.BkMacro[0].Step.Type,
                Time = dataStruct.BkMacro[0].Step.Time,
                Interval = dataStruct.BkMacro[0].Step.Interval
            };
            BackKey.M2.Step = M2;
            IntPtr M2NextPtr = dataStruct.BkMacro[0].Step.Next;
            while (M2NextPtr != IntPtr.Zero)
            {
                BKMACROSTEP step = (BKMACROSTEP)Marshal.PtrToStructure(M2NextPtr, typeof(BKMACROSTEP));
                MacroStep next = new MacroStep
                {
                    Key = step.Key,
                    Type = step.Type,
                    Time = step.Time,
                    Interval = step.Interval
                };
                M2.Next = next;
                M2 = next;
                M2NextPtr = step.Next;
            }

            // M2
            BackKey.M1.CycleFlag = dataStruct.BkMacro[1].CycleFlag;
            BackKey.M1.CycleInterval = dataStruct.BkMacro[1].CycleInterval;
            MacroStep M1 = new MacroStep
            {
                Key = dataStruct.BkMacro[1].Step.Key,
                Type = dataStruct.BkMacro[1].Step.Type,
                Time = dataStruct.BkMacro[1].Step.Time,
                Interval = dataStruct.BkMacro[1].Step.Interval
            };
            BackKey.M1.Step = M1;
            IntPtr M1NextPtr = dataStruct.BkMacro[1].Step.Next;
            while (M1NextPtr != IntPtr.Zero)
            {
                BKMACROSTEP step = (BKMACROSTEP)Marshal.PtrToStructure(M1NextPtr, typeof(BKMACROSTEP));
                MacroStep next = new MacroStep
                {
                    Key = step.Key,
                    Type = step.Type,
                    Time = step.Time,
                    Interval = step.Interval
                };
                M1.Next = next;
                M1 = next;
                M1NextPtr = step.Next;
            }

            _OriData = dataStruct;
        }

        public void Restore()
        {
            LightType = _OriData.LightType;
            LightColor = Color.FromRgb(_OriData.LightRGB[0], _OriData.LightRGB[1], _OriData.LightRGB[2]);
            LightIntensity = _OriData.LightIntensity;

            MotorIntensity = _OriData.MotorIntensity;    //0:关闭, 1:弱, 2:强
            ControllerMode = _OriData.ControllerMode;    //0:手柄, 1:键鼠
            KeyLayout = _OriData.NsPosition;             //0:XBOX按键布局, 1:NS按键布局

            TurboOpen = _OriData.TurboOpen;       //0:关闭, 1:开启
            TurboA = _OriData.TurboA;             //0:关闭, 1:开启
            TurboB = _OriData.TurboB;             //0:关闭, 1:开启
            TurboX = _OriData.TurboX;             //0:关闭, 1:开启
            TurboY = _OriData.TurboY;             //0:关闭, 1:开启
            TurboL1 = _OriData.TurboL1;           //0:关闭, 1:开启
            TurboR1 = _OriData.TurboR1;           //0:关闭, 1:开启

            LStickHeadZoomValue = _OriData.LStickHeadZoomValue;           //0~20（中心死区%）
            RStickHeadZoomValue = _OriData.RStickHeadZoomValue;           //0~20（中心死区%）
            LStickSensitivity = _OriData.LStickSensitivity;               //0~5
            RStickSensitivity = _OriData.RStickSensitivity;               //0~5
            LTriggerSensitivity = _OriData.LTriggerSensitivity;           //0~5
            RTriggerSensitivity = _OriData.RTriggerSensitivity;           //0~5
            StickCalibration = _OriData.StickCalibration;                 //0:退出摇杆校准, 1:进入摇杆校准
            StickCalibrationSucess = _OriData.StickCalibrationSucess;     //0:摇杆校准失败, 1:摇杆校准成功
            TriggerCalibration = _OriData.TriggerCalibration;             //0:退出扳机校准, 1:进入扳机校准
            TriggerCalibrationSucess = _OriData.TriggerCalibrationSucess; //0:扳机校准失败, 1:扳机校准成功
            MkModeKeyData = _OriData.MkModeKeyData;

            BackKey = new BackKeyMacro();

            //// M1
            //BackKey.M1.CycleFlag = _OriData.BkMacro[0].CycleFlag;
            //BackKey.M1.CycleInterval = _OriData.BkMacro[0].CycleInterval;
            //MacroStep M1 = new MacroStep
            //{
            //    Key = _OriData.BkMacro[0].Step.Key,
            //    Type = _OriData.BkMacro[0].Step.Type,
            //    Time = _OriData.BkMacro[0].Step.Time,
            //    Interval = _OriData.BkMacro[0].Step.Interval
            //};
            //BackKey.M1.Step = BackKey.M1.Step;
            //IntPtr M1NextPtr = _OriData.BkMacro[0].Step.Next;
            //while (M1NextPtr != IntPtr.Zero)
            //{
            //    BKMACROSTEP step = (BKMACROSTEP)Marshal.PtrToStructure(_OriData.BkMacro[0].Step.Next, typeof(BKMACROSTEP));
            //    MacroStep next = new MacroStep
            //    {
            //        Key = step.Key,
            //        Type = step.Type,
            //        Time = step.Time,
            //        Interval = step.Interval
            //    };
            //    M1.Next = next;
            //    M1 = next;
            //    M1NextPtr = step.Next;
            //}

            //// M2
            //BackKey.M1.CycleFlag = _OriData.BkMacro[1].CycleFlag;
            //BackKey.M1.CycleInterval = _OriData.BkMacro[1].CycleInterval;
            //MacroStep M2 = new MacroStep
            //{
            //    Key = _OriData.BkMacro[1].Step.Key,
            //    Type = _OriData.BkMacro[1].Step.Type,
            //    Time = _OriData.BkMacro[1].Step.Time,
            //    Interval = _OriData.BkMacro[1].Step.Interval
            //};
            //BackKey.M1.Step = BackKey.M1.Step;
            //IntPtr M2NextPtr = _OriData.BkMacro[1].Step.Next;
            //while (M2NextPtr != IntPtr.Zero)
            //{
            //    BKMACROSTEP step = (BKMACROSTEP)Marshal.PtrToStructure(_OriData.BkMacro[1].Step.Next, typeof(BKMACROSTEP));
            //    MacroStep next = new MacroStep
            //    {
            //        Key = step.Key,
            //        Type = step.Type,
            //        Time = step.Time,
            //        Interval = step.Interval
            //    };
            //    M2.Next = next;
            //    M2 = next;
            //    M2NextPtr = step.Next;
            //}

            OnProfileReresh?.Invoke(this);
        }

        public YZDpadStruct ToStruct()
        {
            return new YZDpadStruct()
            {
                LightType = LightType,
                LightRGB = new byte[] { LightColor.R, LightColor.G, LightColor.B, },
                LightIntensity = LightIntensity,

                MotorIntensity = MotorIntensity,    //0:关闭, 1:弱, 2:强
                ControllerMode = ControllerMode,    //0:手柄, 1:键鼠
                NsPosition = KeyLayout,             //0:XBOX按键布局, 1:NS按键布局

                TurboOpen = TurboOpen,       //0:关闭, 1:开启
                TurboA = TurboA,             //0:关闭, 1:开启
                TurboB = TurboB,             //0:关闭, 1:开启
                TurboX = TurboX,             //0:关闭, 1:开启
                TurboY = TurboY,             //0:关闭, 1:开启
                TurboL1 = TurboL1,           //0:关闭, 1:开启
                TurboR1 = TurboR1,           //0:关闭, 1:开启

                LStickHeadZoomValue = LStickHeadZoomValue,           //0~20（中心死区%）
                RStickHeadZoomValue = RStickHeadZoomValue,           //0~20（中心死区%）
                LStickSensitivity = LStickSensitivity,               //0~5
                RStickSensitivity = RStickSensitivity,               //0~5
                LTriggerSensitivity = LTriggerSensitivity,           //0~5
                RTriggerSensitivity = RTriggerSensitivity,           //0~5
                StickCalibration = StickCalibration,                 //0:退出摇杆校准, 1:进入摇杆校准
                StickCalibrationSucess = StickCalibrationSucess,     //0:摇杆校准失败, 1:摇杆校准成功
                TriggerCalibration = TriggerCalibration,             //0:退出扳机校准, 1:进入扳机校准
                TriggerCalibrationSucess = TriggerCalibrationSucess,  //0:扳机校准失败, 1:扳机校准成功
                MkModeKeyData = new byte[] {
                     (byte)0x00,
                     (byte)KeyCode.KEYB_L_CTRL,
                     (byte)MouseCode.MS_KEY_R,
                     (byte)KeyCode.KEYB_UPARROW,
                     (byte)KeyCode.KEYB_DOWNARROW,
                     (byte)KeyCode.KEYB_LEFTARROW,
                     (byte)KeyCode.KEYB_RIGHTARROW,
                     (byte)0x00,
                     (byte)KeyCode.KEYB_UPARROW,
                     (byte)KeyCode.KEYB_DOWNARROW,
                     (byte)KeyCode.KEYB_LEFTARROW,
                     (byte)KeyCode.KEYB_RIGHTARROW,
                     (byte)0x00,
                     (byte)KeyCode.KEYB_L_ALT,
                     (byte)MouseCode.MS_KEY_L,
                     (byte)KeyCode.KEYB_ENTER,
                     (byte)KeyCode.KEYB_ESC,
                     (byte)KeyCode.KEYB_SPACEBAR,
                     (byte)KeyCode.KEYB_TAB,
                     (byte)MouseCode.MS_KEY_L,
                     (byte)0x00
                },
                BkMacro = new BKMACRO[2]
                {
                    new BKMACRO()
                    {
                        CycleFlag = 0x00,
                        CycleInterval = 0x00,
                        Step = new BKMACROSTEP()
                        {
                            Key = BackKey.M2.Step.Key,
                            Type = BackKey.M2.Step.Type,
                            Time = BackKey.M2.Step.Time,
                            Interval = BackKey.M2.Step.Interval,
                            Next = BackKey.M2.Step.Next == null ? IntPtr.Zero : PopupNext(BackKey.M2.Step.Next)
                        }
                    },
                    new BKMACRO()
                    {
                        CycleFlag = BackKey.M1.CycleFlag,
                        CycleInterval = BackKey.M1.CycleInterval,
                        Step = new BKMACROSTEP()
                        {
                            Key = BackKey.M1.Step.Key,
                            Type = BackKey.M1.Step.Type,
                            Time = BackKey.M1.Step.Time,
                            Interval = BackKey.M1.Step.Interval,
                            Next = BackKey.M1.Step.Next == null ? IntPtr.Zero : PopupNext(BackKey.M1.Step.Next)
                        }
                    }
                }

            //MkModeKeyData = new byte[] {
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.Back : (byte)0x00,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L2 : (byte)KeyCode.KEYB_L_CTRL,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L1 : (byte)MouseCode.MS_KEY_R,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L3D_UP : (byte)KeyCode.KEYB_UPARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L3D_DOWN : (byte)KeyCode.KEYB_DOWNARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L3D_LEFT : (byte)KeyCode.KEYB_LEFTARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L3D_RIGHT : (byte)KeyCode.KEYB_RIGHTARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.L3 : (byte)0x00,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.DPAD_UP : (byte)KeyCode.KEYB_UPARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.DPAD_DOWN : (byte)KeyCode.KEYB_DOWNARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.DPAD_LEFT : (byte)KeyCode.KEYB_LEFTARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.DPAD_RIGHT : (byte)KeyCode.KEYB_RIGHTARROW,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.Start : (byte)0x00,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.R2 : (byte)KeyCode.KEYB_L_ALT,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.R1 : (byte)MouseCode.MS_KEY_L,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.A : (byte)KeyCode.KEYB_ENTER,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.B : (byte)KeyCode.KEYB_ESC,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.X : (byte)KeyCode.KEYB_SPACEBAR,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.Y : (byte)KeyCode.KEYB_TAB,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.R3 : (byte)0x00,
            //    ControllerMode == 0 ? (byte)KeyCodeEnum.Quick : (byte)0x00}
            };
        }

        private IntPtr PopupNext(MacroStep step)
        {
            IntPtr stepPtr = IntPtr.Zero;
            if (step.Next != null)
            {
                stepPtr = PopupNext(step.Next);
            }

            var tmp = new BKMACROSTEP()
            {
                Key = step.Key,
                Type = step.Type,
                Time = step.Time,
                Interval = step.Interval,
                Next = stepPtr
            };

            var tmpPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BKMACROSTEP)));
            Marshal.StructureToPtr(tmp, tmpPtr, true);
            _tmpPtr.Add(tmpPtr);

            return tmpPtr;
        }

        List<IntPtr> _tmpPtr = new List<IntPtr>();

        public void FreeTmpPtr()
        {
            _tmpPtr.ForEach(p =>
            {
                Marshal.FreeHGlobal(p);
            });

            _tmpPtr.Clear();
        }
    }
}
