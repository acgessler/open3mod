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
    }
}
