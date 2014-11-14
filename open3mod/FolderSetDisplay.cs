using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

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
