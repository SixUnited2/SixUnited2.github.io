using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdaterApp
{
    public partial class MainWindow : Window
    {
        // 主窗口的构造方法
        public MainWindow()
        {
            InitializeComponent();
        }

        // 检查更新按钮的点击事件处理函数
        private async void OnCheckForUpdatesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // 检查是否有可用的更新
                var updateInfo = await CheckForUpdatesAsync();
                var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                // 比较当前版本和最新版本
                if (new Version(updateInfo.Version) > currentVersion)
                {
                    // 应用更新
                    await ApplyUpdateAsync(updateInfo);
                    MessageBox.Show("应用程序已更新。请重新启动。", "已更新", MessageBoxButton.OK);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("您使用的是最新版本。", "没有更新", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新过程中出错: {ex.Message}", "更新错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 异步检查是否有可用的更新
        public async Task<UpdateInfo> CheckForUpdatesAsync()
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };

            try
            {
                // 从远程URL获取更新信息
                var json = await httpClient.GetStringAsync("https://raw.githubusercontent.com/maskonface/updateTestServer/main/AutoUpdaterRelease/update-info.json");
                return JsonSerializer.Deserialize<UpdateInfo>(json);
            }
            catch (HttpRequestException)
            {
                throw new Exception("无法获取更新信息。请检查您的网络连接。");
            }
            catch (JsonException)
            {
                throw new Exception("更新信息格式不正确。");
            }
        }

        // 应用更新
        public async Task<bool> ApplyUpdateAsync(UpdateInfo update)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "update.zip");

            // 如果缓存文件存在且其哈希与预期哈希匹配，则跳过下载。
            if (!(File.Exists(tempFile) && ValidateSHA256(tempFile, update.SHA256)))
            {
                var progressWindow = new UpdateProgressWindow();
                progressWindow.Show();

                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(update.Url, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("下载更新失败。");
                }

                // 将下载的内容写入临时文件
                using var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                using var stream = await response.Content.ReadAsStreamAsync();

                var totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault();
                byte[] buffer = new byte[8192];
                int bytesRead;
                double totalBytesRead = 0;

                // 读取并写入数据，同时更新进度
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    progressWindow.UpdateProgress((totalBytesRead / totalBytes) * 100);
                }

                fileStream.Close();
                progressWindow.Close();
            }

            // 验证下载文件的完整性
            if (!ValidateSHA256(tempFile, update.SHA256))
            {
                throw new Exception("更新包已损坏。");
            }

            // 解压更新包
            ZipFile.ExtractToDirectory(tempFile, AppDomain.CurrentDomain.BaseDirectory, true);

            // 检查更新包内的setup.exe文件并执行
            var setupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.exe");
            if (!File.Exists(setupPath))
            {
                throw new Exception("更新包内缺少setup.exe。");
            }

            Process.Start(setupPath);
            return true;
        }

        // 验证文件的SHA256哈希
        private bool ValidateSHA256(string filePath, string expectedHash)
        {
            using var stream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();

            var fileHash = sha256.ComputeHash(stream);
            var fileHashString = BitConverter.ToString(fileHash).Replace("-", "").ToLower();

            return fileHashString == expectedHash;
        }

        // 更新信息的数据结构
        public class UpdateInfo
        {
            [JsonPropertyName("version")]
            public string Version { get; set; }

            [JsonPropertyName("sha256")]
            public string SHA256 { get; set; }

            [JsonPropertyName("url")]
            public string Url { get; set; }
        }
    }
}
