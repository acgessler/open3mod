///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [FolderSetDisplay.cs]
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
using System.Windows.Forms;

namespace open3mod
{
    public partial class FolderSetDisplay : UserControl
    {
        public FolderSetDisplay()
        {
            InitializeComponent();
        }


        public event OnChangeHandler Change;

        public void OnChange()
        {
            OnChangeHandler handler = Change;
            if (handler != null) handler(this);
        }

        public delegate void OnChangeHandler(object sender);


        public String[] Folders { 
            get {
                var folders = new String[listBoxFolders.Items.Count];
                int i = 0;
                foreach (var f in listBoxFolders.Items)
                {
                    folders[i++] = (string)f;
                }
                return folders;
            }

            set {
                listBoxFolders.Items.Clear();
                foreach (var f in value) {
                    listBoxFolders.Items.Add(f);
                }

                OnChange();
            }
        }


        private void OnAddFolder(object sender, EventArgs e)
        {
            var t = textBoxFolder.Text.Trim();
            if (t.Length == 0)
            {
                return;
            }
            listBoxFolders.Items.Insert(0, t);
            listBoxFolders.SelectedItem = listBoxFolders.Items[0];
            OnChange();
        }


        private void OnRemoveFolder(object sender, EventArgs e)
        {
            if (listBoxFolders.SelectedItem == null)
            {
                return;
            }

            listBoxFolders.Items.Remove(listBoxFolders.SelectedItem);
            textBoxFolder.Text = "";
            OnChange();
        }


        private void OnSelectFolder(object sender, EventArgs e)
        {
            if (listBoxFolders.SelectedItem == null)
            {
                return;
            }
            textBoxFolder.Text = (string)listBoxFolders.SelectedItem;
        }

        private void OnBrowse(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }

    
}

/* vi: set shiftwidth=4 tabstop=4: */ 