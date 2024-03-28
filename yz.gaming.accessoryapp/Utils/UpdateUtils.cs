using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;

namespace yz.gaming.accessoryapp.Utils
{
    public class UpdateUtils
    {
        private JsonSerializerOptions _options;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private VersionItem _versionItem;
        public string Url => _versionItem == null ? string.Empty : _versionItem.Url;
        public string UpdateLogs
        {
            get 
            {
                string log = string.Empty;

                if (_versionItem != null)
                {
                    log = _versionItem.Logs;

                    if (_versionItem.languages != null)
                    {
                        var lang = _versionItem.languages.Where(p => p.Language.Equals(LanguangeManager.Instance.Languange)).FirstOrDefault();
                        if (lang != null)
                        {
                            log = lang.Log;
                        }
                    }
                }

                return log;
            }
        } 
        public Version Version { get; set; }
        public bool Updateing { get; set; }

        private static UpdateUtils _instance = null;

        public static UpdateUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UpdateUtils();

                }

                return _instance;
            }
        }

        public UpdateUtils()
        {
            _options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task CheckForUpdatesAsync()
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };

            try
            {
                //var response = await httpClient.GetAsync("https://gitlab.com/yzapp/taishan/-/raw/main/update/update.json?inline=false");
                //var response = await httpClient.GetAsync("https://gitlab.com/sixunited1/GameAssistant/-/raw/tanjun/back_key/update/update.json?ref_type=heads&inline=false");
                var response = await httpClient.GetAsync("https://sixunited2.github.io/update/update.json");
                byte[] responseData = await response.Content.ReadAsByteArrayAsync();

                // 解码响应正文
                var json = Encoding.UTF8.GetString(responseData);
                _versionItem = JsonSerializer.Deserialize<VersionItem>(json, _options);

                if (_versionItem != null && !string.IsNullOrEmpty(_versionItem.Version))
                {
                    Version = new Version(_versionItem.Version);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.Error("无法获取更新信息，请检查您的网络连接。");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            catch (JsonException ex)
            {
                _logger.Error("更新信息格式不正确。");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            catch (Exception ex)
            {
                _logger.Error("获取新版本信息失败。");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        public async Task ApplyUpdateAsync(Action<double> progressNotify, Action<bool, string> messageNotify)
        {
            Updateing = true;

            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "update.zip");

                // 如果缓存文件存在且其哈希与预期哈希匹配，则跳过下载。
                if (!(File.Exists(tempFile) && ValidateSHA256(tempFile, _versionItem.Sha256)))
                {
                    using var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(_versionItem.Url, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Error("Get new version info fail。");
                        messageNotify(false, "UpdateFail");
                        return;
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
                        progressNotify((totalBytesRead / totalBytes) * 100);
                    }

                    fileStream.Close();
                    progressNotify(100);
                }

                // 验证下载文件的完整性
                if (!ValidateSHA256(tempFile, _versionItem.Sha256))
                {
                    _logger.Error("Update zip package damaged。");
                    messageNotify(false, "UpdateFail");
                    return;
                }

                messageNotify(false, "Unziping");
                // 解压更新包
                ZipFile.ExtractToDirectory(tempFile, AppDomain.CurrentDomain.BaseDirectory, true);

                // 检查更新包内的setup.exe文件并执行
                var setupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setup.exe");
                if (!File.Exists(setupPath))
                {
                    _logger.Error("lost setup.exe。");
                    messageNotify(false, "UpdateFail");
                    return;
                }

                WinServiceUtils.StopService("GameAssistantService");
                Process.Start(setupPath);
                messageNotify(true, "Installing");
            }
            catch (Exception ex)
            {
                messageNotify(false, "UpdateFail");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            Updateing = false;
        }

        private bool ValidateSHA256(string filePath, string expectedHash)
        {
            using var stream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();

            var fileHash = sha256.ComputeHash(stream);
            var fileHashString = BitConverter.ToString(fileHash).Replace("-", "").ToLower();

            return fileHashString == expectedHash.ToLower();
        }
    }
}
