
namespace SRPCLoggerEngine
{
    partial class ProjectInstaller
    {

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.serviceProcessInstallerHandle = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerHandle = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerHandle
            // 
            this.serviceProcessInstallerHandle.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstallerHandle.Password = null;
            this.serviceProcessInstallerHandle.Username = null;
            this.serviceProcessInstallerHandle.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_AfterInstall);
            // 
            // serviceInstallerHandle
            // 
            this.serviceInstallerHandle.Description = "My First Service demo";
            this.serviceInstallerHandle.DisplayName = "LoggerEngine";
            this.serviceInstallerHandle.ServiceName = "LoggerEngine";
            this.serviceInstallerHandle.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller_AfterInstall);
            this.serviceInstallerHandle.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerHandle,
            this.serviceInstallerHandle});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerHandle;
        private System.ServiceProcess.ServiceInstaller serviceInstallerHandle;
    }
}