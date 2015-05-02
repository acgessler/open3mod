using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace open3mod
{
    public partial class RenameDialog : Form
    {
        private readonly HashSet<string> _blacklist;
        private readonly HashSet<string> _greylist;
        public string OldName { get; private set; }
        public string NewName { get; private set; }

        public RenameDialog(string oldName, HashSet<string> blacklist, HashSet<string> greylist)
        {
            _blacklist = blacklist;
            _greylist = greylist;
            OldName = oldName;
            InitializeComponent();
            textBoxNewName.Text = OldName;
            if (OldName == "")
            {
                OnValidate(null, null);
            }
            Text = string.Format("{0} {1}", Text, oldName);
        }

       
        private void OnCancel(object sender, EventArgs e)
        {
            NewName = OldName;
            Close();
        }

        private void OnOk(object sender, EventArgs e)
        {
            Close();
        }

        private void OnValidate(object sender, EventArgs e)
        {
            buttonOk.Enabled = true;
            
            if (textBoxNewName.Text.Trim() == "")
            {
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = "Name can not be empty.";
                buttonOk.Enabled = false;
            }
            else if (textBoxNewName.Text == OldName)
            {
                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "Unchanged";
                NewName = OldName;
            }
            else if (_blacklist.Contains(textBoxNewName.Text))
            {
                labelStatus.ForeColor = Color.Red;
                labelStatus.Text = "Name is already used and cannot be re-used.";
                buttonOk.Enabled = false;
            }
            else if (_greylist.Contains(textBoxNewName.Text))
            {
                labelStatus.ForeColor = Color.Orange;
                labelStatus.Text = "Name is already used. It can be used again,\nbut should be avoided.";
            }
            else if (textBoxNewName.Text.Contains(" ") || textBoxNewName.Text.Contains("\t"))
            {
                labelStatus.ForeColor = Color.Orange;
                labelStatus.Text = "Avoid whitespace in names";
            }
            else
            {
                labelStatus.ForeColor = Color.Green;
                labelStatus.Text = "OK";
                NewName = textBoxNewName.Text;
            }
        }
    }
}
