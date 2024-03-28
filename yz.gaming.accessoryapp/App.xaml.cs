using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.ViewModel;
using yz.gaming.accessoryapp.ViewModel.Main;
using yz.gaming.accessoryapp.ViewModel.HomePage;
using yz.gaming.accessoryapp.ViewModel.ControllerPage;
using yz.gaming.accessoryapp.ViewModel.Setting;
using System.Diagnostics;
using static yz.gaming.accessoryapp.Utils.ComImpl;
using yz.gaming.accessoryapp.Utils;
using System.Runtime.InteropServices;
using yz.gaming.accessoryapp.Service;
using yz.gaming.accessoryapp.Model;
using System.Security.Principal;
using System.Threading;
using System.Windows.Threading;
using NLog;

namespace yz.gaming.accessoryapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string SERVICE_NAME = "GameAssistantService";
        private const string APP_NAME = "GameAssistant";
        private const string ARGUMENT_INSTALL_SERVICE = "installservice";

        private static System.Threading.Mutex mutex; //系统能够识别有名称的互斥，因此可以使用它禁止应用程序启动两次
        Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, APP_NAME);

            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                var process = SystemUtils.Instance.RunningInstance();
                if (process != null)
                {
                    SystemUtils.Instance.ShowProcess(process);
                }
                this.Shutdown();
            }

            if (!IsAdministrator())
            {
                if (WinServiceUtils.IsServiceExisted(SERVICE_NAME))
                {
                    if (WinServiceUtils.IsServiceRunning(SERVICE_NAME))
                    {
                        WinServiceUtils.StartAppViaService(SERVICE_NAME);
                    }
                    else
                    {
                        WinServiceUtils.StartService(SERVICE_NAME);
                    }
                }
                else
                {
                    // Restart and run as admin and install service
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
                    {
                        Verb = "runas",
                        UseShellExecute = true,
                        Arguments = ARGUMENT_INSTALL_SERVICE
                    };
                    Process.Start(startInfo);
                }

                Environment.Exit(0);
            }

            if (e.Args.Length > 0 && e.Args[0].Equals(ARGUMENT_INSTALL_SERVICE))
            {
                var path = $"{AppDomain.CurrentDomain.BaseDirectory}\\{SERVICE_NAME}.exe";
                WinServiceUtils.InstallService(SERVICE_NAME, path);
            }

            Ioc.Default.ConfigureServices(new ServiceCollection()
                .AddSingleton(typeof(MainContainerViewModel))
                .AddSingleton(typeof(HomePageViewModel))
                .AddSingleton(typeof(NewHomePageViewModel))
                .AddSingleton(typeof(GuidePageViewModel))
                .AddSingleton(typeof(DeviceInfoPageViewModel))
                .AddSingleton(typeof(GamePlatformPageViewModel))
                .AddSingleton(typeof(ControllerPageViewModel))
                .AddSingleton(typeof(SettingPageViewModel))
                .AddSingleton(typeof(KeyPageViewModel))
                .AddSingleton(typeof(KeyDescriptionPageViewModel))
                .AddSingleton(typeof(RepetPageViewModel))
                .AddSingleton(typeof(CalibrationAndAdvancedSettingPageViewModel))
                .AddSingleton(typeof(LightEffectPageViewModel))
                .AddSingleton(typeof(ShakePageViewModel))
                .AddSingleton(typeof(GyroPageViewModel))
                .AddSingleton(typeof(FirmwarePageViewModel))
                .AddSingleton(typeof(LanguagePageViewModel))
                .AddSingleton(typeof(SoftwareVersionPageViewModel))
                .AddSingleton(typeof(YzProfileModel))
                .AddSingleton(typeof(QuickMenuViewModel))
                .BuildServiceProvider());

            LanguangeManager.Instance.OnLanguageChanged += OnLanguageChanged;

            if (accessoryapp.Properties.Settings.Default.IsFirstRun)
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "zh-CN")
                {
                    accessoryapp.Properties.Settings.Default.Languange = "zh-Hans";
                }
                else
                {
                    accessoryapp.Properties.Settings.Default.Languange = "en-US";
                }

                accessoryapp.Properties.Settings.Default.IsFirstRun = false;
                accessoryapp.Properties.Settings.Default.Save();
            }

            OnLanguageChanged(accessoryapp.Properties.Settings.Default.Languange);
            //LanguangeManager.Instance.LanguangeDictionary = Resources.MergedDictionaries[0];

            YzProfileModel model = Ioc.Default.GetRequiredService<YzProfileModel>();

            Task.Run(async () =>
            {
                try
                {
                    if (!YzGamingService.Instance.IsInited)
                    {
                        Thread.Sleep(1000);
                        YzGamingService.Instance.OnProfileDataChanged += model.OnProfileChanged;
                        YzGamingService.Instance.Initialize();
                    }

                    await UpdateUtils.Instance.CheckForUpdatesAsync();
                    await FwUpdateUtils.Instance.CheckForUpdatesAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            YzGamingService.Instance.Uninitialize();
        }

        private void OnLanguageChanged(string language)
        {
            var path = $"pack://application:,,,/Languange/{language}/languange.xaml";
            Resources.MergedDictionaries[0].Source = new Uri(path);
            LanguangeManager.Instance.LanguangeDictionary = Resources.MergedDictionaries[0];
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 应用程序启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.DispatcherUnhandledException += App_OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                _logger.Error(e.Exception.Message);
                _logger.Error(e.Exception.StackTrace);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// 非UI线程抛出全局异常事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    _logger.Error(exception.Message);
                    _logger.Error(exception.StackTrace);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
