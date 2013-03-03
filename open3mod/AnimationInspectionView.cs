using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public sealed partial class AnimationInspectionView : UserControl
    {
        public AnimationInspectionView(TabPage tabPageAnimations)
        {
            InitializeComponent();
            tabPageAnimations.Controls.Add(this);

            Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }
    }
}
