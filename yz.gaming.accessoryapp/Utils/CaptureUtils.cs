using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace yz.gaming.accessoryapp.Utils
{
    public class CaptureUtils 
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private const byte VK_LWIN = 0x5B;
        private const byte VK_SNAPSHOT = 0x2C;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private static CaptureUtils instance = null;

        public static CaptureUtils Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CaptureUtils();
                }

                return instance;
            }
        }

        public void Capture()
        {
            CaptureWindow(IntPtr.Zero, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");

            //IntPtr hwnd = GetFullscreenWindowOnPrimary();
            //if (hwnd != IntPtr.Zero)
            //{
            //    ActivateAndWait(hwnd);
            //    CaptureWindow(hwnd, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.png");
            //}
        }

        private static void SimulateWinPrintScreenKeyPress()
        {
            keybd_event(VK_LWIN, 0, 0, IntPtr.Zero);
            keybd_event(VK_SNAPSHOT, 0, 0, IntPtr.Zero);
            keybd_event(VK_SNAPSHOT, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        private void ActivateAndWait(IntPtr hwnd)
        {
            SetForegroundWindow(hwnd);
            Thread.Sleep(500);
        }

        private IntPtr GetFullscreenWindowOnPrimary()
        {
            IntPtr hwnd = GetForegroundWindow();

            while (hwnd != IntPtr.Zero)
            {
                if (IsZoomed(hwnd))
                {
                    int length = GetWindowTextLength(hwnd);
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hwnd, sb, sb.Capacity);
                    string windowTitle = sb.ToString();

                    if (IsWindowVisible(hwnd) && windowTitle != "Program Manager")
                    {
                        return hwnd;
                    }
                }

                hwnd = GetWindow(hwnd, 2);
            }

            return IntPtr.Zero;
        }

        private void CaptureWindow(IntPtr hWnd, string filename)
        {
            SimulateWinPrintScreenKeyPress();

            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    Thread.Sleep(1000);
                    var bitmapSource = Clipboard.GetImage();

                    if (bitmapSource != null)
                    {
                        System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        bitmapSource.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
                        bitmap.UnlockBits(data);

                        //var path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        var path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Screenshots";

                        bitmap.Save($"{path}\\{filename}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            }));
            thread.TrySetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
