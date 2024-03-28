using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace yz.gaming.testopenappviaservice
{
    public class WinServiceUtils
    {
        public static void StartAppViaService(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName, Environment.MachineName);
            ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, Environment.MachineName, "GameAssistantService");
            scp.Assert();
            sc.Refresh();
            sc.ExecuteCommand(128);
        }

        public static bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services != null && services.ToList().Exists(p => p.ServiceName == serviceName);
        }

        public static bool IsServiceRunning(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName, Environment.MachineName))
            {
                return sc.Status == ServiceControllerStatus.Running;
            }
        }

        public static void StartService(string serviceName)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C sc start {serviceName}",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            });
        }

        public static void InstallService(string serviceName, string path)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C sc create {serviceName} binpath= {path} start= auto && sc start {serviceName}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            });
        }

        public static void StopService(string serviceName)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C sc stop {serviceName} 4:4:4 update",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            });
        }

        public static void UninstallService(string serviceName)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C sc delete {serviceName}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
            });
        }
    }
}
