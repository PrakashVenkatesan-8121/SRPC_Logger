using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace SRPCLoggerEngine
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.serviceInstallerHandle.AfterInstall += Autorun_AfterServiceInstall;
        }

        void Autorun_AfterServiceInstall(object sender, InstallEventArgs e)
        {
            ServiceInstaller serviceInstaller = (ServiceInstaller)sender;
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }
        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
           
        }

        private void serviceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
