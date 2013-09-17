///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [LogViewer.cs]
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using open3mod.Properties;

namespace open3mod
{
    public partial class LogViewer : Form
    {
        private readonly MainWindow _mainWindow;
        private LogStore _currentLogStore;

        private const string RtfHeader =
	    @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Consolas;}}" +
        @"{\colortbl;" +
            // color palette:
            @"\red255\green0\blue0;" + 
            @"\red255\green120\blue0;" + 
            @"\red0\green150\blue0;" + 
            @"\red0\green0\blue180;" + 
            @"\red0\green0\blue0;}";


        public MainWindow MainWindow
        {
            get { return _mainWindow; }
        }


        public LogViewer(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();

            _mainWindow.TabChanged += (tab, add) =>
            {
                if(IsDisposed)
                {
                    return;
                }
                PopulateList();
            };

            PopulateList();
        }


        private void PopulateList()
        {
            comboBoxSource.Items.Clear();
 
            int select = -1;
            foreach (var tab in MainWindow.UiState.Tabs)
            {
                if(tab.File == null)
                {
                    continue;
                }
                var index = comboBoxSource.Items.Add(tab.File);
                if (tab == MainWindow.UiState.ActiveTab)
                {
                    select = index;
                }
            }

            if (select != -1)
            {
                comboBoxSource.SelectedItem = comboBoxSource.Items[select];
            }
            else
            {
                comboBoxSource.SelectedItem = comboBoxSource.Items.Count > 0 ? comboBoxSource.Items[0] : null;
            }
            FetchLogEntriesFromScene();
        }


        private void FetchLogEntriesFromScene()
        {
            var sceneName = (string)comboBoxSource.SelectedItem;
            var scene = (from tab in MainWindow.UiState.Tabs where tab.File == sceneName select tab.ActiveScene).FirstOrDefault();

            if (scene == null)
            {
                richTextBox.Text = Resources.LogViewer_FetchLogEntriesFromScene_No_scene_loaded;
                return;
            }

            _currentLogStore = scene.LogStore;
            BuildRtf();  
        }


        private void BuildRtf()
        {
            var sb = new StringBuilder();

            sb.Append(RtfHeader);

            foreach (var entry in _currentLogStore.Messages)
            {
                string s;
                switch (entry.Cat)
                {
                    case LogStore.Category.Info:
                        if(!checkBoxFilterInformation.Checked)
                        {
                            continue;
                        }

                        s = @"\pard \cf3 \b \fs18 ";
                        break;
                    case LogStore.Category.Warn:
                        if (!checkBoxFilterWarning.Checked)
                        {
                            continue;
                        }

                        s = @"\pard \cf2 \b \fs18 ";
                        break;
                    case LogStore.Category.Error:
                        if (!checkBoxFilterError.Checked)
                        {
                            continue;
                        }

                        s = @"\pard \cf1 \b \fs18 ";
                        break;
                    case LogStore.Category.Debug:
                        if (!checkBoxFilterVerbose.Checked)
                        {
                            continue;
                        }

                        s = @"\pard \cf4 \b \fs18 ";
                        break;
                    case LogStore.Category.System:
                        s = @"\pard \cf5 \b \fs18 ";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                s = s + "job: "
                    + entry.ThreadId.ToString(CultureInfo.InvariantCulture).PadLeft(5, ' ')
                    + ",\t time: "
                    + entry.Time.ToString(CultureInfo.InvariantCulture).PadLeft(10, ' ')
                    + ",\t";

                sb.Append(s);
                foreach (var ch in entry.Message)
                {
                    if (ch == '\n' || ch == '\r')
                    {
                        continue;
                    }

                    if (ch == '\\' || ch == '}' || ch == '{')
                    {
                        sb.Append('\\');
                    }

                    sb.Append(ch);
                }

                sb.Append(@"\par ");
            }

            sb.Append('}');

            var rtfCode = sb.ToString();
            richTextBox.Rtf = rtfCode;
        }


        private void OnClearAll(object sender, EventArgs e)
        {          
            _currentLogStore.Drop();
            richTextBox.Text = Resources.LogViewer_OnClearAll_Nothing_to_display;
        }


        private void OnSave(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var stream = new StreamWriter(saveFileDialog.OpenFile()))
            {
                foreach (var entry in _currentLogStore.Messages)
                {
                    stream.Write(LogEntryToPlainText(entry) + "\r\n");
                }
            }               
        }


        private string LogEntryToPlainText(LogStore.Entry entry)
        {
            string s;
            switch (entry.Cat)
            {
                case LogStore.Category.Info:
                    s = "Info:   ";
                    break;
                case LogStore.Category.Warn:
                    s = "Warn:   ";
                    break;
                case LogStore.Category.Error:
                    s = "Error:  ";
                    break;
                case LogStore.Category.Debug:
                    s = "Debug:  ";
                    break;
                case LogStore.Category.System:
                    s = "System: ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return entry.ThreadId.ToString(CultureInfo.InvariantCulture).PadLeft(4) 
                + "|" 
                + entry.Time.ToString(CultureInfo.InvariantCulture).PadLeft(10,'0') 
                + "   " 
                + s 
                + entry.Message;
        }


        private void OnFilterChange(object sender, EventArgs e)
        {
            BuildRtf();
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ChangeLogSource(object sender, EventArgs e)
        {
            FetchLogEntriesFromScene();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 