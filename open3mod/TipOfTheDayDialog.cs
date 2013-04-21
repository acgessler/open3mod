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
    public partial class TipOfTheDayDialog : Form
    {
        private static String[] _tips = new[]
            {
@"You can use the force to control 
almost everything.

Just make sure the force is strong
in you. Do not attempt to count
Midi-Chlorians.",

@"This is actually just a dummy entry.

Therefore, use it with caution.
",
            };

        private int _cursor;


        public TipOfTheDayDialog()
        {
            InitializeComponent();
            SetTip(CoreSettings.CoreSettings.Default.NextTip);
        }


        private void SetTip(int nextTip)
        {
            while(nextTip < 0)
            {
                nextTip += _tips.Length;
            }
            _cursor = nextTip % _tips.Length;

            pictureBoxTipPic.Image = ImageFromResource.Get("open3mod.Images.TipOfTheDay.Tip" + _cursor + ".png");
            labelTipText.Text = _tips[_cursor];
        }


        private void OnPrevious(object sender, EventArgs e)
        {
            SetTip(_cursor - 1);   
        }


        private void OnNext(object sender, EventArgs e)
        {
            SetTip(_cursor + 1);
        }


        private void OnClose(object sender, FormClosingEventArgs e)
        {
            CoreSettings.CoreSettings.Default.NextTip = (_cursor + 1)%_tips.Length;
        }


        private void OnChangeStartup(object sender, EventArgs e)
        {
            // for some reason the prop binding does not work.
            CoreSettings.CoreSettings.Default.ShowTipsOnStartup = checkBoxDoNotShowAgain.Checked;
        }
    }
}
