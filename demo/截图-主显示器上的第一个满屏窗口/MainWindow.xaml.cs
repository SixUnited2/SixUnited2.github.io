using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ScreenshotApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static void SimulateWinPrintScreenKeyPress()
        {
            keybd_event(VK_LWIN, 0, 0, IntPtr.Zero);
            keybd_event(VK_SNAPSHOT, 0, 0, IntPtr.Zero);
            keybd_event(VK_SNAPSHOT, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = GetFullscreenWindowOnPrimary();
            if (hwnd != IntPtr.Zero)
            {
                ActivateAndWait(hwnd);
                CaptureWindow(hwnd, "screen.png");
            }
            else
            {
                Log("未找到全屏窗口!");
            }
        }

        private void ActivateAndWait(IntPtr hwnd)
        {
            SetForegroundWindow(hwnd);
            System.Threading.Thread.Sleep(500);
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
        }

        // P/Invoke declarations

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
    }
}
