
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
            this.serviceProcessInstallerHandle.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerHandle.Password = null;
            this.serviceProcessInstallerHandle.Username = null;
            this.serviceProcessInstallerHandle.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_AfterInstall);
            this.serviceInstallerHandle.Description = "Logger Engine is Service Registered PipeLine Listerner Capable of Logging the data passing via the pipeLine";
            this.serviceInstallerHandle.DisplayName = "LoggerEngine";
            this.serviceInstallerHandle.ServiceName = "LoggerEngine";
            this.serviceInstallerHandle.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstaller_AfterInstall);
            this.serviceInstallerHandle.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerHandle,
            this.serviceInstallerHandle});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerHandle;
        private System.ServiceProcess.ServiceInstaller serviceInstallerHandle;
    }
}