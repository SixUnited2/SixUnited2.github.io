using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using yz.gaming.accessoryapp.Api;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using static yz.gaming.accessoryapp.Api.YzCommonApi;

namespace yz.gaming.accessoryapp.Utils
{
    public class FwUpdateUtils
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

        private static FwUpdateUtils _instance = null;

        public static FwUpdateUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FwUpdateUtils();
                }

                return _instance;
            }
        }

        public FwUpdateUtils()
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
                var response = await httpClient.GetAsync("https://gitlab.com/yzapp/taishan/-/raw/main/update/fwupdate.json?inline=false");
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

        public async Task ApplyUpdateAsync(Action<string, double> progressNotify, Action<bool, string> messageNotify)
        {
            Updateing = false;

            try
            {
                string tempFile = Path.Combine(Path.GetTempPath(), "updataPkg.hjz");

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
                    progressNotify("Downloading", (totalBytesRead / totalBytes) * 100);
                }

                fileStream.Close();
                fileStream.Dispose();
                progressNotify("Downloading", 100);

                // 验证下载文件的完整性
                if (!ValidateSHA256(tempFile, _versionItem.Sha256))
                {
                    _logger.Error("Update zip package damaged。");
                    messageNotify(false, "UpdateFail");
                    return;
                }

                progressNotify("Updateing", 0);
                await Task.Delay(500);

                String filePath = tempFile;
                var caller = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(YZDpadStruct)));
                int status = -99;
                bool isCallback = false;

                try
                {
                    var result = YzCommonApi.YZGamingUpdate_Initialize(filePath, caller,
                        new Pfn_DeviceUpdateValueCallBack(value =>
                        {
                            try
                            {
                                progressNotify("Updateing", value);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message);
                                _logger.Error(ex.StackTrace);
                            }

                        }),
                        new Pfn_DeviceUpdateValueCallBack(value =>
                        {
                            try
                            {
                                status = value;
                                isCallback = status == 0;
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex.Message);
                                _logger.Error(ex.StackTrace);
                            }
                        }));

                    if (result == 0)
                    {
                        var version = YzCommonApi.YZGamingUpdate_GetDeviceVersionInUpdatePackage();

                        result = YzCommonApi.YZGamingUpdate_Start();

                        int waitTime = 60000;

                        while (waitTime > 0)
                        {
                            if (isCallback) break;
                            await Task.Delay(10);
                            waitTime -= 10;
                        }

                        await Task.Delay(500);
                        YzCommonApi.YZGamingUpdate_Uninitialize();
                        Updateing = status == 0;
                    }
                }
                catch (Exception ex)
                {
                    messageNotify(false, "UpdateFail");
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);

                    YzCommonApi.YZGamingUpdate_Uninitialize();
                }
                finally
                {
                    Marshal.FreeHGlobal(caller);
                }

                messageNotify(Updateing, Updateing ? "UpdateSuccess" : "UpdateFail");
            }
            catch (Exception ex)
            {
                messageNotify(false, "UpdateFail");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
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
