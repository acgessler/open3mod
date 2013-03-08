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
            
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            Dock = DockStyle.Fill;
            InitializeComponent();

            tabPageAnimations.Controls.Add(this);

            listBoxAnimations.Items.Add("None (Bind Pose)");

            if (scene.Raw.Animations != null)
            {
                foreach (var anim in scene.Raw.Animations)
                {
                    listBoxAnimations.Items.Add(anim.Name);
                }                
            }
            listBoxAnimations.SelectedIndex = 0;
        }

        public bool Empty
        {
            get { return _scene.Raw.AnimationCount == 0; }
        }


        private void OnChangeSelectedAnimation(object sender, EventArgs e)
        {
            _scene.SceneAnimator.ActiveAnimation = listBoxAnimations.SelectedIndex - 1;
        }
    }
}
