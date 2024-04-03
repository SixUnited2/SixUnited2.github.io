using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace yz.gaming.accessoryapp.Utils
{
    public static class ComImpl
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void Pfn_ProfileDeviceEventHandler(int deviceEvent, IntPtr hProfileDevice, IntPtr lpContext);

        const string DLL_PATH = @"Dll/yz.gaming.com.dll";

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_Initialize(
            Pfn_ProfileDeviceEventHandler pfnProfileDeviceEventHandler,
            IntPtr lpContext);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_ReadProfileData(
            IntPtr hProfileDevice,
            IntPtr lpProfileData);

        [DllImport(DLL_PATH, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int YZGamingCom_WriteProfileData(
            IntPtr hProfileDevice,
            IntPtr lpProfileData);
    }
}
