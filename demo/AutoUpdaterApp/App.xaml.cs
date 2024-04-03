using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdaterApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // 在应用程序启动时执行的方法
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            // 创建主窗口实例
            var mainWindow = new MainWindow();

            try
            {
                // 启动时检查更新
                var updateInfo = await mainWindow.CheckForUpdatesAsync();
                // 获取当前应用的版本号
                var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // 检查是否有新版本可用
                if (new Version(updateInfo.Version) > currentVersion)
                {
                    // 弹出提示框询问用户是否需要进行更新
                    var result = MessageBox.Show("有新的更新可用。您现在要更新吗?", "有可用的更新", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    // 如果用户选择了“是”，则执行更新
                    if (result == MessageBoxResult.Yes)
                    {
                        var updateApplied = await mainWindow.ApplyUpdateAsync(updateInfo);
                        // 如果更新成功，关闭应用程序
                        if (updateApplied)
                        {
                            Application.Current.Shutdown();
                            return;  // 更新后，不继续执行启动流程，很重要
                        }
                    }
                }
            }
            catch (Exception ex) // 捕获过程中的任何异常
            {
                // 弹出提示框显示错误信息
                MessageBox.Show($"更新过程中出错: {ex.Message}", "更新错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 只显示一个窗口
            if (Application.Current.MainWindow == null)
                mainWindow.Show();
        }
    }


}

