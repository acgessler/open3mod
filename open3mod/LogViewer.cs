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
	    @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Courier New;}}" +
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

            FetchLogEntriesFromScene();
        }


        private void FetchLogEntriesFromScene()
        {
            var scene = MainWindow.UiState.ActiveTab.ActiveScene;
            if(scene == null)
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
            saveFileDialog.ShowDialog();
            using (var openFile = saveFileDialog.OpenFile())
            {
                using (var stream = new StreamWriter(openFile))
                {
                    foreach (var entry in _currentLogStore.Messages)
                    {                       
                        stream.Write(LogEntryToPlainText(entry) + "\r\n");
                    }
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

            return entry.Time.ToString(CultureInfo.InvariantCulture).PadLeft(10,'0') + "   " + s + entry.Message;
        }


        private void OnFilterChange(object sender, EventArgs e)
        {
            BuildRtf();
        }
    }
}

