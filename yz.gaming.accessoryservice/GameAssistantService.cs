using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace yz.gaming.accessoryservice
{
    public partial class GameAssistantService : ServiceBase
    {
        const string APP_NAME = "GameAssistant";
        const string SERVICE_NAME = "GameAssistantService";

        public GameAssistantService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);

                string path = GetWindowsServiceInstallPath(SERVICE_NAME);
                _ = UserProcess.CurrentUserStartProcessAndBypassUAC($"{path}\\{APP_NAME}.exe", out _);
            });
        }

        protected override void OnStop()
        {
        }

        protected override void OnCustomCommand(int command)
        {
            base.OnCustomCommand(command);

            switch (command)
            {
                case 128:
                    Task.Run(async () =>
                    {
                        await Task.Delay(300);

                        string path = GetWindowsServiceInstallPath(SERVICE_NAME);
                        _ = UserProcess.CurrentUserStartProcessAndBypassUAC($"{path}\\{APP_NAME}.exe", out _);
                    });
                    break;
                default:
                    break;
            }
        }

        #region 取服务安装路径
        /// <summary>
        /// 获取服务应用程序的安装路径(或者当前安装目录)
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public string GetWindowsServiceInstallPath(string ServiceName)
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + ServiceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            //替换掉双引号 
            path = path.Replace("\"", string.Empty);

            FileInfo fi = new FileInfo(path);
            return fi.Directory.FullName;
        }
        #endregion
    }
}
