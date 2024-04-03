using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Utils;
using yz.gaming.accessoryapp.View;

namespace yz.gaming.accessoryapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new MainContainerView();
            GamePlatform.Instance.OnMiniWindows += OnMiniWindows;
            GamePlatform.Instance.OnMaxWindows += OnMaxWindows;

            this.StateChanged += MainWindow_StateChanged;
            YzGamingService.Instance.MainWindow = this;
            YzGamingService.Instance.QuickMenu = new QuickMenu();

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowState = WindowState.Maximized;
            YzGamingService.Instance.IsMainShown = true;
        }

        public new void Show()
        {
            OnMaxWindows();
            base.Show();
        }

        public new void Hide()
        {
            OnMiniWindows();
            base.Hide();
        }

        public void CheckStatus()
        {
            YzGamingService.Instance.IsMainShown = this.WindowState == WindowState.Maximized;
            System.Diagnostics.Debug.WriteLine($"CheckStatus -> IsMainShown: {YzGamingService.Instance.IsMainShown} MainWindowsStatus:{this.WindowState.ToString()}");
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"MainWindowsStatus:{this.WindowState.ToString()}");

            if (this.WindowState == WindowState.Normal)
            {
                YzGamingService.Instance.HideQuickMenu();

                Task.Run(async () =>
                {
                    await Task.Delay(100);
                    await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.WindowState = WindowState.Maximized;
                        YzGamingService.Instance.IsMainShown = this.WindowState == WindowState.Maximized;
                    }));
                });
            }

            YzGamingService.Instance.IsMainShown = this.WindowState == WindowState.Maximized;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            this.WindowState = WindowState.Maximized;
            this.Topmost = false;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;

            System.Diagnostics.Debug.WriteLine($"OnActivated -> IsMainShown: {YzGamingService.Instance.IsMainShown} MainWindowsStatus:{this.WindowState.ToString()}");

            Task.Run(async () =>
            {
                await Task.Delay(200);
                await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    CheckStatus();
                }));
            });
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            YzGamingService.Instance.IsMainShown = false;

            System.Diagnostics.Debug.WriteLine($"OnDeactivated -> IsMainShown: {YzGamingService.Instance.IsMainShown} MainWindowsStatus:{this.WindowState.ToString()}");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WinApi.WM_USER + 1:
                    Show();
                    handled = true;
                    break;
            }

            return IntPtr.Zero;
        }

        private void OnMiniWindows()
        {
            YzGamingService.Instance.IsMainShown = false;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Topmost = false;
                this.WindowState = WindowState.Minimized;
            }));
        }

        private void OnMaxWindows()
        {
            YzGamingService.Instance.IsMainShown = true;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.WindowState = this.WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
            }));
        }
    }
}
