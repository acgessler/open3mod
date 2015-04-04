///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TipOfTheDayDialog.cs]
// (c) 2012-2015, Open3Mod Contributors
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
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace open3mod
{
    public partial class TipOfTheDayDialog : Form
    {
        private static String[] _tips = new[]
            {
@"You can lock on a search by pressing ENTER.

Pressing ENTER again cycles through the search 
results. The current selection is highlighted
in yellow then.",

@"You can permanently hide parts of a scene by
right-clicking on it in the Scene Browser 
and selecting 'Hide'.
",

 @"Double-click on a texture to see it in full size. 
If the texture viewer is already open, hovering 
over a mini texture shows a quick preview of it.
",

  @"In the toolbar you can highlight the joints in 
a scene. This is extremely useful when viewing 
rigged models. When animations are played, 
the visualization reflects the skeletal 
movements.
",

  @"When hovering over a joint in the Scene Browser,
joint highlighting is automatically turned on and 
the selected joint highlighted.
",

   @"Use Bullseye mode to lock the current 3D view.
When you hover with your mouse over parts of the
3D scene, the Scene Browser automatically 
highlights them in the scene hierarchy.
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

/* vi: set shiftwidth=4 tabstop=4: */ 