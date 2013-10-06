///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [ExportDialog.cs]
// (c) 2012-2013, Open3Mod Contributors
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public partial class ExportDialog : Form
    {
        private readonly MainWindow _main;
        private readonly Assimp.ExportFormatDescription[] _formats;

        private bool _changedText = false;


        public string SelectedFormatId {
            get {
                return _formats[comboBoxExportFormats.SelectedIndex].FormatId;
            }
        }

        public Assimp.ExportFormatDescription SelectedFormat
        {
            get
            {
                return _formats[comboBoxExportFormats.SelectedIndex];
            }
        }

        public ExportDialog(MainWindow main)
        {
            _main = main;
            InitializeComponent();

            using (var v = new Assimp.AssimpContext())
            {
                _formats = v.GetSupportedExportFormats();
                foreach(var format in _formats) {
                    comboBoxExportFormats.Items.Add(format.Description + "  (" + format.FileExtension + ")");
                }
                comboBoxExportFormats.SelectedIndex = ExportSettings.Default.ExportFormatIndex;
                comboBoxExportFormats.SelectedIndexChanged += (object s, EventArgs e) =>
                {
                    ExportSettings.Default.ExportFormatIndex = comboBoxExportFormats.SelectedIndex;
                    UpdateFileName(true);
                };
            }

            textBoxFileName.KeyPress += (object s, KeyPressEventArgs e) =>
            {
                _changedText = true;
            };

            // Respond to updates in the main window - the export dialog is non-modal and
            // always takes the currently selected file at the time the export button
            // is pressed.
            _main.SelectedTabChanged += (Tab tab) =>
            {
                UpdateFileName();
                UpdateCaption();
            };

            UpdateFileName();
            UpdateCaption();
        }


        private void UpdateCaption()
        {
            string str = "Export ";
            var scene = _main.UiState.ActiveTab.ActiveScene;
            if (scene != null)
            {
                str += Path.GetFileName(scene.File);
                buttonExportRun.Enabled = true;
            }
            else
            {
                buttonExportRun.Enabled = false;
                str += "<no scene currently selected>";
            }

            Text = str;
        }


        private void UpdateFileName(bool extensionOnly = false) 
        {
            string str = textBoxFileName.Text;

            if (!extensionOnly && !_changedText)
            {
                var scene = _main.UiState.ActiveTab.ActiveScene;
                if (scene != null)
                {
                    str = Path.GetFileName(scene.File);
                }
            }
            textBoxFileName.Text = Path.ChangeExtension(str, SelectedFormat.FileExtension);
        }


        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog(this);
            textBoxPath.Text = folderBrowserDialog.SelectedPath;
        }


        private void buttonExport(object sender, EventArgs e)
        {
            var scene = _main.UiState.ActiveTab.ActiveScene;
            if (scene == null)
            {
                MessageBox.Show("No exportable scene selected");
                return;
            }
            
                DoExport(scene, SelectedFormatId);
        }

        private void DoExport(Scene scene, string id) 
        {
            progressBarExport.Style = ProgressBarStyle.Marquee;
            progressBarExport.MarqueeAnimationSpeed = 5;

            var path = textBoxPath.Text.Trim();
            var name = textBoxFileName.Text.Trim();
            var fullPath = (path.Length > 0 ? path + "\\" : "") + name;
            var t = new Thread(() => {
                using (var v = new Assimp.AssimpContext())
                {
                    bool result = v.ExportFile(scene.Raw, fullPath, id);

                    _main.BeginInvoke(new MethodInvoker(() =>
                    {
                        progressBarExport.Style = ProgressBarStyle.Continuous;
                        progressBarExport.MarqueeAnimationSpeed = 0;

                        if (!result) {
                            // TODO: get native error message
                            MessageBox.Show("Failed to export to " + path, "Export error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }));
                }
            });

            t.Start();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 