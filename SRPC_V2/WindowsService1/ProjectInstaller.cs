using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace CentralLogStreamHandler
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // Set the account type
            processInstaller.Account = ServiceAccount.LocalSystem;

            // Set service properties
            serviceInstaller.ServiceName = "CentralLogStreamHandler";
            serviceInstaller.DisplayName = "Central Log Stream Handler";
            serviceInstaller.Description = "Windows Service for centralized, concurrent log stream handling via IPC.";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
