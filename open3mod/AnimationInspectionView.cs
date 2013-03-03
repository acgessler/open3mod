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
        private readonly Scene _scene;

        public AnimationInspectionView(Scene scene, TabPage tabPageAnimations)
        {
            _scene = scene;
            InitializeComponent();
            tabPageAnimations.Controls.Add(this);

            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }

        public bool Empty
        {
            get { return _scene.Raw.AnimationCount == 0; }
        }
    }
}
