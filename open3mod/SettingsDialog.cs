///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [SettingsDialog.cs]
// (c) 2012-2013, Alexander C. Gessler
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public partial class SettingsDialog : Form
    {
        private GraphicsSettings _gSettings;
        private MainWindow _main;

        public SettingsDialog()
        {
            InitializeComponent();

            _gSettings = GraphicsSettings.Default;

            InitTexResolution();
            InitTexFilter();
            InitMultiSampling();
            InitLightingQuality();
            InitRenderingBackend();
        }


        public MainWindow Main
        {
            set { _main = value; }
        }


        private void OnOk(object sender, EventArgs e)
        {
            _gSettings.Save();
            if (_main == null)
            {
                Close();
                return;
            }
            _main.CloseSettingsDialog();
        }


        private void InitTexResolution()
        {
            var bias = _gSettings.TexQualityBias;
            if (bias == 0)
            {
                comboBoxTexResolution.SelectedIndex = 0;
            }
            else if (bias == 1)
            {
                comboBoxTexResolution.SelectedIndex = 1;
            }
            else if (bias > 1)
            {
                comboBoxTexResolution.SelectedIndex = 2;
            }
        }


        private void OnChangeTextureResolution(object sender, EventArgs e)
        {
            switch(comboBoxTexResolution.SelectedIndex)
            {
                case 0:
                    _gSettings.TexQualityBias = 0;
                    
                    break;
                case 1:
                    _gSettings.TexQualityBias = 1;
                    break;
                case 2:
                    _gSettings.TexQualityBias = 3;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            if(_main == null)
            {
                return;
            }
            foreach (var scene in _main.UiState.ActiveScenes())
            {
                scene.RequestReuploadTextures();
            }
        }


        private void InitTexFilter()
        {
            comboBoxSetTextureFilter.SelectedIndex = _gSettings.TextureFilter;
        }


        private void OnChangeTextureFilter(object sender, EventArgs e)
        {
            Debug.Assert(comboBoxSetTextureFilter.SelectedIndex <= 3);
            _gSettings.TextureFilter = comboBoxSetTextureFilter.SelectedIndex;
           
            if (_main == null)
            {
                return;
            }
            foreach (var scene in _main.UiState.ActiveScenes()) 
            {
                scene.RequestReconfigureTextures();
            }
        }


        private void OnChangeMipSettings(object sender, EventArgs e)
        {
            foreach (var scene in _main.UiState.ActiveScenes())
            {
                scene.RequestReconfigureTextures();
            }
        }


        private void InitMultiSampling()
        {
            comboBoxSetMultiSampling.SelectedIndex = _gSettings.MultiSampling;
        }


        private void OnChangeMultiSamplingMode(object sender, EventArgs e)
        {
            Debug.Assert(comboBoxSetMultiSampling.SelectedIndex <= 3);
            if (_gSettings.MultiSampling != comboBoxSetMultiSampling.SelectedIndex)
            {
                _gSettings.MultiSampling = comboBoxSetMultiSampling.SelectedIndex;
                labelPleaseRestart.Visible = true;
            }
        }


        private void InitLightingQuality()
        {     
            comboBoxSetLightingMode.SelectedIndex = _gSettings.LightingQuality;
        }


        private void InitRenderingBackend()
        {
            comboBoxSetBackend.SelectedIndex = _gSettings.RenderingBackend;
        }


        private void OnChangeRenderingBackend(object sender, EventArgs e)
        {
            Debug.Assert(comboBoxSetBackend.SelectedIndex <= 1);
            _gSettings.RenderingBackend = comboBoxSetBackend.SelectedIndex;

            if (_main == null)
            {
                return;
            }
            foreach (var scene in _main.UiState.ActiveScenes())
            {
                scene.RecreateRenderingBackend();
            }
        }


        private void checkBoxBFCulling_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var scene in _main.UiState.ActiveScenes())
            {
                scene.RequestRenderRefresh();
            }
        }

        private void checkBoxGenerateTangentSpace_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxComputeNormals_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 