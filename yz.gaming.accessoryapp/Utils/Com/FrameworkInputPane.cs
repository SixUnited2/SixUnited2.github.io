using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Shapes;

namespace yz.gaming.accessoryapp.Utils.Com
{
    [ComImport, Guid("D5120AA3-46BA-44C5-822D-CA8092C1FC72")]
    public class FrameworkInputPane
    {
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
        Guid("5752238B-24F0-495A-82F1-2FD593056796")]
    public interface IFrameworkInputPane
    {
        [PreserveSig]
        int Advise(
            [MarshalAs(UnmanagedType.IUnknown)] object pWindow,
            [MarshalAs(UnmanagedType.IUnknown)] object pHandler,
            out int pdwCookie
            );

        [PreserveSig]
        int AdviseWithHWND(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.IUnknown)] object pHandler,
            out int pdwCookie
            );

        [PreserveSig]
        int Unadvise(
            int pdwCookie
            );

        [PreserveSig]
        int Location(
            out RECT prcInputPaneScreenLocation
            );
    }

    public struct RECT
    {
        long left;
        long top;
        long right;
        long bottom;
    }
}
