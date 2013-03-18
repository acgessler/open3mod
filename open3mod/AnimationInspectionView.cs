///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [AnimationInspectionView.cs]
// (c) 2012-2013, Alexander C. Gessler
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
        private Timer _timer;
        private double _duration;
        private bool _playing;

        private const int TimerInterval = 30;

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
            _playing = true;
        }


        public bool Empty
        {
            get { return _scene.Raw.AnimationCount == 0; }
        }


        public bool Playing
        {
            get { return _playing; }
            set
            {
                if (value == (_timer != null))
                {
                    return;
                }
                _playing = value;
                if (value)
                {
                    StartPlayingTimer();            
                }
                else
                {
                    StopPlayingTimer();                  
                }
            }
        }


        private void StopPlayingTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }


        private void StartPlayingTimer()
        {
            if (!Playing)
            {
                return;
            }

            _timer = new Timer { Interval = (TimerInterval) };
            _timer.Tick += (o, args) =>
            {
                timeSlideControl.Position = _scene.SceneAnimator.AnimationCursor % _duration;
            };
            _timer.Start();
        }


        private void OnChangeSelectedAnimation(object sender, EventArgs e)
        {
            _scene.SceneAnimator.ActiveAnimation = listBoxAnimations.SelectedIndex - 1;
            if (_scene.SceneAnimator.ActiveAnimation >= 0)
            {
                var anim = _scene.Raw.Animations[_scene.SceneAnimator.ActiveAnimation];
                foreach (var control in panelAnimTools.Controls)
                {
                    ((Control)control).Enabled = true;
                }

                _duration = _scene.SceneAnimator.AnimationDuration;

                timeSlideControl.RangeMin = 0.0;
                timeSlideControl.RangeMax = _duration;
                timeSlideControl.Position = 0.0;

                StartPlayingTimer();
            }
            else
            {
                foreach(var control in panelAnimTools.Controls)
                {
                    ((Control) control).Enabled = false;
                }

                StopPlayingTimer();
            }
        }


        private void OnPlay(object sender, EventArgs e)
        {
            Playing = !Playing;          
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 