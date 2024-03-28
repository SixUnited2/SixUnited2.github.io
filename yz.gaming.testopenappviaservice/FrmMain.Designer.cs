
namespace yz.gaming.testopenappviaservice
{
    partial class FrmMain
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRunApp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRunApp
            // 
            this.btnRunApp.Location = new System.Drawing.Point(13, 13);
            this.btnRunApp.Name = "btnRunApp";
            this.btnRunApp.Size = new System.Drawing.Size(75, 23);
            this.btnRunApp.TabIndex = 0;
            this.btnRunApp.Text = "RunApp";
            this.btnRunApp.UseVisualStyleBackColor = true;
            this.btnRunApp.Click += new System.EventHandler(this.btnRunApp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 163);
            this.Controls.Add(this.btnRunApp);
            this.Name = "Form1";
            this.Text = "Open app via service";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRunApp;
    }
}

