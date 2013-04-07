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
        }


        public MainWindow Main
        {
            set { _main = value; }
        }


        private void OnOk(object sender, EventArgs e)
        {
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
            foreach(var tab in _main.UiState.Tabs)
            {
                if (tab.ActiveScene == null)
                {
                    continue; ;
                }
                tab.ActiveScene.RequestReuploadTextures();
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
            foreach (var tab in _main.UiState.Tabs)
            {
                if (tab.ActiveScene == null)
                {
                    continue; ;
                }
                tab.ActiveScene.RequestReconfigureTextures();
            }
        }


        private void OnChangeMipSettings(object sender, EventArgs e)
        {
            foreach (var tab in _main.UiState.Tabs)
            {
                if (tab.ActiveScene == null)
                {
                    continue; ;
                }
                tab.ActiveScene.RequestReconfigureTextures();
            }
        }
    }
}
