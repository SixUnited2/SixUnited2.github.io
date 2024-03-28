using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Threading.Tasks;

namespace yz.gaming.accessoryservice
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            Microsoft.Win32.RegistryKey system;
            Microsoft.Win32.RegistryKey currentControlSet;
            Microsoft.Win32.RegistryKey services;
            Microsoft.Win32.RegistryKey service;
            Microsoft.Win32.RegistryKey config;

            try
            {
                //Let the project installer do its job
                base.Install(stateSaver);

                //Open the HKEY_LOCAL_MACHINE/SYSTEM key
                system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
                //Open CurrentControlSet
                currentControlSet = system.OpenSubKey("CurrentControlSet");
                //Go to the services key
                services = currentControlSet.OpenSubKey("Services");
                //Open the key for your service, and allow writing
                service = services.OpenSubKey(GameAssistantServiceInstaller.ServiceName, true);
                //Add your service's description as a REG_SZ value named "Description"
                service.SetValue("Description", "Service of GameAssistant app");
                service.SetValue("DisplayName", "GameAssistantService");
                //(Optional) Add some custom information your service will use...
                config = service.CreateSubKey("Parameters");
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception was thrown during service installation:/n" + e.ToString());
            }
        }

        public override void Commit(IDictionary savedState)
        {

        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            try
            {
                var path = Process.GetCurrentProcess().MainModule.FileName;
                var serviceName = "GameAssistantService";
                var isAdmin = IsAdministrator();

                base.OnAfterInstall(savedState);

                ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + GameAssistantServiceInstaller.ServiceName + "'");
                ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
                InParam["DesktopInteract"] = true;
                ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", InParam, null);

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C sc start {serviceName}",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = isAdmin ? string.Empty : "runas",
                    WindowStyle = ProcessWindowStyle.Normal
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception was thrown during service commit:/n" + e.ToString());
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            Microsoft.Win32.RegistryKey system;
            Microsoft.Win32.RegistryKey currentControlSet;
            Microsoft.Win32.RegistryKey services;
            Microsoft.Win32.RegistryKey service;

            try
            {
                //Drill down to the service key and open it with write permission
                system = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System");
                currentControlSet = system.OpenSubKey("CurrentControlSet");
                services = currentControlSet.OpenSubKey("Services");
                service = services.OpenSubKey(GameAssistantServiceInstaller.ServiceName, true);
                //Delete any keys you created during installation (or that your service created)
                service.DeleteSubKeyTree("Parameters");
                //...
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception encountered while uninstalling service:/n" + e.ToString());
            }
            finally
            {
                //Let the project installer do its job
                base.Uninstall(savedState);
            }
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
