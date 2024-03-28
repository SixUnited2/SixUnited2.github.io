using System.ServiceProcess;

namespace yz.gaming.accessoryservice
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
            new GameAssistantService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
