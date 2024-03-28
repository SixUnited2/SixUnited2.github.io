
namespace yz.gaming.accessoryservice
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.GameAssistantServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.GameAssistantServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // GameAssistantServiceProcessInstaller
            // 
            this.GameAssistantServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.GameAssistantServiceProcessInstaller.Password = null;
            this.GameAssistantServiceProcessInstaller.Username = null;
            // 
            // GameAssistantServiceInstaller
            // 
            this.GameAssistantServiceInstaller.Description = "Service of GameAssistant app";
            this.GameAssistantServiceInstaller.DisplayName = "GameAssistantService";
            this.GameAssistantServiceInstaller.ServiceName = "GameAssistantService";
            this.GameAssistantServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.GameAssistantServiceProcessInstaller,
            this.GameAssistantServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller GameAssistantServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller GameAssistantServiceInstaller;
    }
}