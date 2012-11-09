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
    public partial class LogViewer : Form
    {
        private readonly MainWindow _mainWindow;


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

            FetchLogEntries();
        }


        private void FetchLogEntries()
        {
            var scene = MainWindow.UiState.ActiveScene;
            if(scene == null)
            {
                richTextBox.Text = "No scene loaded";
                return;
            }

            var sb = new StringBuilder();

            sb.Append(RtfHeader);

            foreach(var entry in scene.LogStore.Messages)
            {
                string s;
                switch(entry.Cat)
                {
                    case LogStore.Category.Info:
                        s = @"\pard \cf3 \b \fs18 ";
                        break;
                    case LogStore.Category.Warn:
                        s = @"\pard \cf2 \b \fs18 ";
                        break;
                    case LogStore.Category.Error:
                        s = @"\pard \cf1 \b \fs18 ";
                        break;
                    case LogStore.Category.Debug:
                        s = @"\pard \cf4 \b \fs18 ";
                        break;
                    case LogStore.Category.System:
                        s = @"\pard \cf5 \b \fs18 ";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sb.Append(s);

                foreach(var ch in entry.Message)
                {
                    if(ch == '\n' || ch == '\r')
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
    }
}

