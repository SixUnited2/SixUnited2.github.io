using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yz.gaming.testopenappviaservice
{
    public partial class FrmMain : Form
    {
        private const string SERVICE_NAME = "GameAssistantService";
        private const string APP_NAME = "GameAssistant";

        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnRunApp_Click(object sender, EventArgs e)
        {
            WinServiceUtils.StartAppViaService(SERVICE_NAME);
        }
    }
}
