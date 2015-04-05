///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [DonationDialog.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace open3mod
{
    public partial class DonationDialog : Form
    {
        public DonationDialog()
        {
            InitializeComponent();
            labelCount.Text = "In total, you have opened " + CoreSettings.CoreSettings.Default.CountFilesOpened + " 3D models";
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
            Process.Start("http://www.open3mod.com/donate.htm?ref=inapp");
            Close();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 