using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yz.gaming.SHA256Generator
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "压缩包(*.zip,*.hjz)|*.zip;*.hjz";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var stream = File.OpenRead(dialog.FileName);
                    var sha256 = SHA256.Create();

                    var fileHash = sha256.ComputeHash(stream);
                    var fileHashString = BitConverter.ToString(fileHash).Replace("-", "").ToLower();

                    txtValue.Text = fileHashString;
                }
            }
        }
    }
}
