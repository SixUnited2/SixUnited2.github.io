using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace yz.gaming.accessoryapp.Utils
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct YZDpadStruct1
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] lightRGB;        //0:red, 1:green, 2:blue
        public byte lightIntensity;    //0:关闭, 1~5档

        public byte motorIntensity;    //0:关闭, 1:弱, 2:强
        public byte controllerMode;    //0:手柄, 1:键鼠
        public byte nsPosition;        //0:XBOX按键布局, 1:NS按键布局

        public byte turboOpen;         //0:关闭, 1:开启
        public byte turboA;            //0:关闭, 1:开启
        public byte turboB;            //0:关闭, 1:开启
        public byte turboX;            //0:关闭, 1:开启
        public byte turboY;            //0:关闭, 1:开启
        public byte turboL1;           //0:关闭, 1:开启
        public byte turboL2;           //0:关闭, 1:开启

        public byte lStickHeadZoomValue;       //0~20（中心死区%）
        public byte rStickHeadZoomValue;       //0~20（中心死区%）
        public byte lStickSensitivity;         //0~5
        public byte rStickSensitivity;         //0~5
        public byte lTriggerSensitivity;       //0~5
        public byte rTriggerSensitivity;       //0~5
        public byte stickCalibration;          //0:退出摇杆校准, 1:进入摇杆校准
        public byte stickCalibrationSucess;    //0:摇杆校准失败, 1:摇杆校准成功
        public byte triggerCalibration;        //0:退出扳机校准, 1:进入扳机校准
        public byte triggerCalibrationSucess;  //0:扳机校准失败, 1:扳机校准成功

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] mkModeKeyData;
    }
}
