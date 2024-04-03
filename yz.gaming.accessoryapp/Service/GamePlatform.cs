using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using yz.gaming.accessoryapp.Languange;
using yz.gaming.accessoryapp.Model;
using yz.gaming.accessoryapp.Utils;

namespace yz.gaming.accessoryapp.Service
{
    public class GamePlatform
    {
        const string STEAM = "STEAM";
        const string EPIC = "EPIC";
        const string EA = "EA";
        const string WEGAME = "WEGAME";
        const string XBOX = "XBOX";
        const string ROCKSTAR = "ROCKSTAR";
        const string GOG = "GOG";
        const string UBI = "UBI";
        const string BLIZZARD = "BLIZZARD";

        public event Action<PlatformEnum> OnPlatformAdded;
        public event Action<PlatformEnum> OnPlatformRemoved;
        public event Action OnMiniWindows;
        public event Action OnMaxWindows;

        static GamePlatform _gamePlatform;
        static object locker = new object();

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public static GamePlatform Instance
        {
            get
            {
                lock (locker)
                {
                    if (_gamePlatform == null)
                    {
                        _gamePlatform = new GamePlatform();
                    }
                }

                return _gamePlatform;
            }
        }

        //public bool IsPlatformRunning { get; set; }
        //public bool IsPlatformStarting { get; set; }
        public List<PlatformModel> HomePlatforms { get; set; }

        private Dictionary<PlatformEnum, PlatformModel> _platformList;

        public GamePlatform()
        {
            HomePlatforms = new List<PlatformModel>();

            _platformList = new Dictionary<PlatformEnum, PlatformModel>()
            {
                {
                    PlatformEnum.STEAM,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.STEAM,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/Steam.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "Steam.exe",
                        Parameter = "-noverifyfiles",
                        Name = STEAM,
                        FindName = "Steam",
                        ProcessesByName = "Steam",
                        Url = @"https://store.steampowered.com"
                    }
                },
                {
                    PlatformEnum.EPIC,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.EPIC,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/Epic.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "Epic.exe",
                        Name = EPIC,
                        FindName = "Epic Games Launcher",
                        ProcessesByName = "EpicGamesLauncher",
                        Url = @"https://store.epicgames.com/"
                    }
                },
                {
                    PlatformEnum.EA,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.EA,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/Origin.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "Origin.exe",
                        Name = EA,
                        FindName = "EA",
                        ProcessesByName = "EA",
                        Url = @"https://www.ea.com"
                    }
                },
                {
                    PlatformEnum.WEGAME,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.WEGAME,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/WeGame.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "WeGame.exe",
                        Name = WEGAME,
                        FindName = "WeGame",
                        ProcessesByName = "WeGame",
                        Url = @"https://www.wegame.com.cn"
                    }
                },
                {
                    PlatformEnum.XBOX,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.XBOX,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/XBox.png",
                        IsInstall = false,
                        Path = "xbox:",
                        Exe = "xbox:",
                        Name = XBOX,
                        PackageName = "Microsoft.XboxApp_8wekyb3d8bbwe",
                        Url = @"https://www.xbox.com"
                    }
                },
                {
                    PlatformEnum.ROCKSTAR,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.ROCKSTAR,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/RockStar.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "RockStar.exe",
                        Name = ROCKSTAR,
                        FindName = "Rockstar Games Launcher",
                        ProcessesByName = "Rockstar Games Launcher",
                        Url = @"https://www.rockstargames.com"
                    }
                },
                {
                    PlatformEnum.GOG,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.GOG,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/GOG.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "GOG.exe",
                        Name = GOG,
                        Url = @"https://embed.gog.com"
                    }
                },
                {
                    PlatformEnum.UBI,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.UBI,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/Ubi.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "Ubi.exe",
                        Name = UBI,
                        FindName = "Ubisoft Connect",
                        ProcessesByName = "Ubisoft Connect",
                        Url = @"https://store.ubi.com"
                    }
                },
                {
                    PlatformEnum.BLIZZARD,
                    new PlatformModel()
                    {
                        Platform = PlatformEnum.BLIZZARD,
                        Icon = "pack://SiteOfOrigin:,,,/Resource/Image/Blizzard.png",
                        IsInstall = false,
                        Path = string.Empty,
                        Exe = "Blizzard.exe",
                        Name = BLIZZARD,
                        Url = @"https://blizzcon.com"
                    }
                }
            };

            GetAppList();
            Task.Run(async () =>
            {
                await GetUwpApp();
            });

            var platforms = Properties.Settings.Default.GamePlatforms.Split(',');
            foreach (var item in platforms)
            {
                if (int.Parse(item) >= 0)
                {
                    HomePlatforms.Add(_platformList[(PlatformEnum)Convert.ToInt32(item)]);
                }
            }
        }

        public Dictionary<string, string> GetAppList()
        {
            var findList = new List<string>();

            foreach (var item in _platformList)
            {
                if (!string.IsNullOrEmpty(item.Value.FindName))
                {
                    findList.Add(item.Value.FindName);
                }
            }

            var _appList = AppListUtils.LoadAppList(findList);

            foreach (var item in _platformList)
            {
                var key = item.Value.FindName;
                if (!string.IsNullOrEmpty(key))
                {
                    item.Value.IsInstall = _appList.ContainsKey(key);
                    item.Value.Path = _appList.ContainsKey(key) ? _appList[key] : string.Empty;
                }
            }

            return _appList;
        }

        public async Task GetUwpApp()
        {
            var support = await Windows.System.Launcher.QueryUriSupportAsync(new Uri("xbox:"), Windows.System.LaunchQuerySupportType.Uri);
            _platformList[PlatformEnum.XBOX].IsInstall = support == Windows.System.LaunchQuerySupportStatus.Available;
        }

        public void SendToHomePage(PlatformEnum platform, out string message)
        {
            message = LanguangeManager.Instance.GetString("SendToHomeSuccess");

            if (_platformList[platform].IsInstall &&
                !HomePlatforms.Contains(_platformList[platform]))
            {
                if (HomePlatforms.Count < 8)
                {
                    HomePlatforms.Add(_platformList[platform]);
                }
                else
                {
                    HomePlatforms[7] = _platformList[platform];
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(i < HomePlatforms.Count ? (int)HomePlatforms[i].Platform : -1);
                    sb.Append(i < 7 ? "," : string.Empty);
                }

                Properties.Settings.Default.GamePlatforms = sb.ToString();
                Properties.Settings.Default.Save();
                OnPlatformAdded?.Invoke(platform);
            }
            else
            {
                message = LanguangeManager.Instance.GetString("PlatformNotInstall");
            }
        }

        public void RemoveFormHomePage(PlatformEnum platform)
        {
            if (HomePlatforms.Contains(_platformList[platform]))
            {
                HomePlatforms.Remove(_platformList[platform]);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(i < HomePlatforms.Count ? (int)HomePlatforms[i].Platform : -1);
                    sb.Append(i < 7 ? "," : string.Empty);
                }

                Properties.Settings.Default.GamePlatforms = sb.ToString();
                Properties.Settings.Default.Save();
                OnPlatformRemoved?.Invoke(platform);
            }
        }

        public PlatformModel GetPlatformModel(PlatformEnum platform)
        {
            return _platformList[platform];
        }

        public void Start(PlatformModel model)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(model.PackageName))
                    {
                        _logger.Trace($"Open app => {model.PackageName}");
                        await Windows.System.Launcher.LaunchUriAsync(new Uri($"{model.Path}"));
                    }
                    else
                    {
                        try
                        {
                            //if (IsPlatformStarting || IsPlatformRunning) return;

                            Process p = new Process();
                            p.StartInfo.FileName = model.Path;
                            p.StartInfo.Arguments = model.Parameter;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.RedirectStandardInput = true;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.RedirectStandardError = true;
                            p.StartInfo.CreateNoWindow = false;
                            p.Exited += GamePlatform_Exited;
                            p.EnableRaisingEvents = true;
                            p.Start();
                            //IsPlatformStarting = true;

                            //await Task.Run(() =>
                            //{
                            //    int loop = 10;
                            //    int tick = 1000;

                            //    while (loop > 0)
                            //    {
                            //        loop--;
                            //        Thread.Sleep(tick);

                            //        if (Process.GetProcessesByName(model.Name).Length > 0)
                            //        {
                            //            //IsPlatformRunning = true;
                            //            OnMiniWindows?.Invoke();
                            //        }
                            //    }
                            //    //IsPlatformStarting = false;
                            //});
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }
            });

            OnMiniWindows?.Invoke();
        }

        private void GamePlatform_Exited(object sender, EventArgs e)
        {
            //var steams = Process.GetProcessesByName("Steam");
            //if (steams.Length == 0)
            //{
                //IsPlatformRunning = false;
                OnMaxWindows?.Invoke();
            //}
        }

        public void MiniWindows()
        {
            OnMiniWindows?.Invoke();
        }

        public void MaxWindows()
        {
            OnMaxWindows?.Invoke();
        }

        public bool OpenDefaultBrowserUrl(string url)
        {
            bool isOpen = true;
            try
            {
                //从注册表中读取默认浏览器可执行文件路径
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                if (key != null)
                {
                    string browserPath = string.Empty;
                    string[] splitArr = new string[] { };
                    string browser = key.GetValue("").ToString();

                    //browser是默认浏览器，不同的浏览器后面的参数不一样。例如："D:\Program Files (x86)\Google\chrome.exe" -- "%1"
                    var lastIndex = browser.IndexOf(".exe", StringComparison.Ordinal);
                    if (lastIndex == -1)
                        lastIndex = browser.IndexOf(".EXE", StringComparison.Ordinal);

                    if (lastIndex != -1)
                    {
                        splitArr = browser.Split("\"");
                    }
                    //大于0 说明 按照 " 切割到数据
                    if (splitArr.Length > 0)
                    {
                        browserPath = splitArr[1];
                    }
                    else if (splitArr.Length == 0 && lastIndex != -1)
                    {
                        browserPath = browser;
                    }

                    var result = Process.Start(browserPath, url);
                }
                else
                {
                    isOpen = false;
                    //OpenIe(url);
                }

            }
            catch
            {
                isOpen = false;
            }

            return isOpen;
        }
    }
}
