using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public partial class DonationDialog : Form
    {
        public DonationDialog()
        {
            InitializeComponent();
        }

        private void NotNowAskAgain(object sender, EventArgs e)
        {
            Close();
        }

        private void DontAskAgain(object sender, EventArgs e)
        {
            CoreSettings.CoreSettings.Default.DonationUseCountDown = -1;
            Close();
        }

        private void Donate(object sender, EventArgs e)
        {
            // TODO
            Close();
        }
    }
}
