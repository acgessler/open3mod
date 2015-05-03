///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [NodeItemsDialog.cs]
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
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Assimp;
using OpenTK;

namespace open3mod
{
    public partial class NodeItemsDialog : Form, IHoverUpdateDialog
    {
        private Node _node;
        private readonly Timer _timer;
        private Scene _scene;


        private const int TimerUpdateInterval = 100;

        public NodeItemsDialog()
        {
            InitializeComponent();
            _timer = new Timer {Interval = TimerUpdateInterval};
            _timer.Tick += TimerOnTick;
            if (checkBoxShowAnimated.Checked)
            {
                _timer.Start();
            }

            UpdateCollapseState(true);
        }


        public void SetNode(MainWindow mainWindow, Scene scene, Node node)
        {
            _node = node;
            _scene = scene;

            var matrix4X4 = _node.Transform;
            trafoMatrixViewControlLocal.SetMatrix(ref matrix4X4);

            var mat = Matrix4x4.Identity;
            var cur = node;
            while(cur != null)
            {
                var trafo = cur.Transform;
                trafo.Transpose();
                mat = trafo * mat;
                cur = cur.Parent;
            }
            mat.Transpose();
            trafoMatrixViewControlGlobal.SetMatrix(ref mat);

            Text = node.Name + " - Node Details";

            // populate statistics
            labelMeshesDirect.Text = node.MeshCount.ToString(CultureInfo.InvariantCulture);
            labelChildrenDirect.Text = node.ChildCount.ToString(CultureInfo.InvariantCulture);

            var meshTotal = 0;
            var childTotal = 0;
            CountMeshAndChildrenTotal(node, ref meshTotal, ref childTotal);

            labelMeshesTotal.Text = node.MeshCount.ToString(CultureInfo.InvariantCulture);
            labelChildrenTotal.Text = node.ChildCount.ToString(CultureInfo.InvariantCulture);
        }


        private void CountMeshAndChildrenTotal(Node node, ref int meshTotal, ref int childTotal)
        {
            Debug.Assert(node != null);

            meshTotal += node.MeshCount;
            childTotal += node.ChildCount;
            for (var i = 0; i < node.ChildCount; ++i)
            {
                CountMeshAndChildrenTotal(node.Children[i], ref meshTotal, ref childTotal);
            }
        }


        private void OnChangeAnimationState(object sender, EventArgs e)
        {
            if(checkBoxShowAnimated.Checked)
            {
                _timer.Start();
            }
            else
            {
                _timer.Stop();

                trafoMatrixViewControlGlobal.ResetAnimatedMatrix();
                trafoMatrixViewControlLocal.ResetAnimatedMatrix();
            }
        }


        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if(_scene == null)
            {
                return;
            }

            Debug.Assert(_node != null);
            var anim = _scene.SceneAnimator;
            if (anim.ActiveAnimation == -1)
            {
                trafoMatrixViewControlGlobal.ResetAnimatedMatrix();
                trafoMatrixViewControlLocal.ResetAnimatedMatrix();
            }
            else
            {
                Matrix4 m;
                Matrix4x4 mm;
                anim.GetGlobalTransform(_node, out m);
                OpenTkToAssimp.FromMatrix(ref m, out mm);
                trafoMatrixViewControlGlobal.SetAnimatedMatrix(ref mm);

                anim.GetLocalTransform(_node, out m);
                OpenTkToAssimp.FromMatrix(ref m, out mm);
                trafoMatrixViewControlLocal.SetAnimatedMatrix(ref mm);
            }
        }


        private void OnToggleShowGlobalTrafo(object sender, EventArgs e)
        {
            UpdateCollapseState();
           
        }


        private void UpdateCollapseState(bool initial = false)
        {
            const int collapseOffset = 34;
            if (checkBoxShowGlobalTransformation.Checked)
            {
                checkBoxShowGlobalTransformation.Text = "Show Global Transformation ...";
                Height -= trafoMatrixViewControlGlobal.Height + collapseOffset;

                trafoMatrixViewControlGlobal.Visible = false;
            }
            else
            {
                if (!initial)
                {
                    checkBoxShowGlobalTransformation.Text = "Hide Global Transformation ...";
                    Height += trafoMatrixViewControlGlobal.Height + collapseOffset;
                }
                trafoMatrixViewControlGlobal.Visible = true;
            }
        }

        public bool HoverUpdateEnabled
        {
            // Currently, we do not have any sub dialogs so Hover Update is permanently on.
            get { return true; }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 