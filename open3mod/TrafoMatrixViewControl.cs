///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TrafoMatrixViewControl.cs]
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
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Assimp;
using OpenTK;
using Quaternion = Assimp.Quaternion;

namespace open3mod
{
    /// <summary>
    /// Visualizes the translation, scaling and rotation parts of a transformation
    /// matrix.
    /// </summary>
    public partial class TrafoMatrixViewControl : UserControl
    {
        private Vector3D _scale;
        private Quaternion _rot;
        private Vector3D _trans;
        private Matrix4x4 _baseMatrix;

        private Quaternion _rotCurrent;
        private bool _isInDiffView;

        private const string Format = "#0.000";

        private enum RotationMode
        {
            // note: this directly corresponds to the UI combobox, keep in sync!
            EulerXyzDegrees = 0,
            EulerXyzRadians = 1,
            Quaternion = 2
        }


        public TrafoMatrixViewControl()
        {
            InitializeComponent();

            comboBoxRotMode.SelectedIndex = CoreSettings.CoreSettings.Default.DefaultRotationMode < comboBoxRotMode.Items.Count 
                ? CoreSettings.CoreSettings.Default.DefaultRotationMode : 0;

            // workaround based on
            // http://stackoverflow.com/questions/276179/how-to-change-the-font-color-of-a-disabled-textbox
            // WinForms only draws custom FG colors for readonly text boxes if an internal "ColorCustomized"
            // flag is set. To set this flag, we have to assign to the BackColor at least once.
            foreach (var cc in Controls.OfType<Control>())
            {
                cc.BackColor = cc.BackColor;
            }
        }


        /// <summary>
        /// Update the display. This involves decomposing the matrix and is
        /// therefore an expensive operation.
        /// </summary>
        /// <param name="mat"></param>
        public void SetMatrix(ref Matrix4x4 mat)
        {
            UpdateUi(mat, false);
            _baseMatrix = mat;
        }


        /// <summary>
        /// Sets an animated matrix to be displayed instead of the last matrix
        /// set via SetMatrix(). Changed components are highlighted in the UI.
        /// </summary>
        /// <param name="mat"></param>
        public void SetAnimatedMatrix(ref Matrix4x4 mat)
        {
            _isInDiffView = true;
            UpdateUi(mat, true);  
        }


        /// <summary>
        /// Set the display back to the last matrix set via SetMatrix()
        /// </summary>
        public void ResetAnimatedMatrix()
        {
            _isInDiffView = false;
            UpdateUi(_baseMatrix, false);
        }


        private void OnUpdateRotation(object sender, EventArgs e)
        {
            CoreSettings.CoreSettings.Default.DefaultRotationMode = comboBoxRotMode.SelectedIndex;
            SetRotation(_isInDiffView);
        }


        private void UpdateUi(Matrix4x4 mat, bool diffAgainstBaseMatrix)
        {
            // use assimp math data structures because they have Decompose()
   
            // the decomposition algorithm is not very sophisticated - it basically extracts translation
            // and row scaling factors and then converts the rest to a quaternion. 
            // question: what if the matrix is non-invertible? the algorithm then yields
            // at least one scaling factor as zero, further results are undefined. We
            // therefore catch this case by checking the determinant and inform the user
            // that the results may be wrong.
            checkBoxNonStandard.Checked = Math.Abs(mat.Determinant()) < 1e-5;

            Vector3D scale;
            Quaternion rot;
            Vector3D trans;

            mat.Decompose(out scale, out rot, out trans);

            // translation
            textBoxTransX.Text = trans.X.ToString(Format);
            textBoxTransY.Text = trans.Y.ToString(Format);
            textBoxTransZ.Text = trans.Z.ToString(Format);

            // scaling - simpler view mode for uniform scalings
            var isUniform = Math.Abs(scale.X - scale.Y) < 1e-5 && Math.Abs(scale.X - scale.Z) < 1e-5;
            textBoxScaleX.Text = scale.X.ToString(Format);
            textBoxScaleY.Text = scale.Y.ToString(Format);
            textBoxScaleZ.Text = scale.Z.ToString(Format);

            textBoxScaleY.Visible = !isUniform;
            textBoxScaleZ.Visible = !isUniform;
            labelScalingX.Visible = !isUniform;
            labelScalingY.Visible = !isUniform;
            labelScalingZ.Visible = !isUniform;

            if (diffAgainstBaseMatrix)
            {
                const double epsilon = 1e-5f;
                if (Math.Abs(scale.X-_scale.X) > epsilon)
                {
                    labelScalingX.ForeColor = ColorIsAnimated;
                    textBoxScaleX.ForeColor = ColorIsAnimated;
                }

                if (Math.Abs(scale.Y - _scale.Y) > epsilon)
                {
                    labelScalingY.ForeColor = ColorIsAnimated;
                    textBoxScaleY.ForeColor = ColorIsAnimated;
                }

                if (Math.Abs(scale.Z - _scale.Z) > epsilon)
                {
                    labelScalingZ.ForeColor = ColorIsAnimated;
                    textBoxScaleZ.ForeColor = ColorIsAnimated;
                }

                if (Math.Abs(trans.X - _trans.X) > epsilon)
                {
                    labelTranslationX.ForeColor = ColorIsAnimated;
                    textBoxTransX.ForeColor = ColorIsAnimated;
                }

                if (Math.Abs(trans.Y - _trans.Y) > epsilon)
                {
                    labelTranslationY.ForeColor = ColorIsAnimated;
                    textBoxTransY.ForeColor = ColorIsAnimated;
                }

                if (Math.Abs(trans.Z - _trans.Z) > epsilon)
                {
                    labelTranslationZ.ForeColor = ColorIsAnimated;
                    textBoxTransZ.ForeColor = ColorIsAnimated;
                }
            }
            else
            {
                labelScalingX.ForeColor = ColorNotAnimated;
                textBoxScaleX.ForeColor = ColorNotAnimated;

                labelScalingY.ForeColor = ColorNotAnimated;
                textBoxScaleY.ForeColor = ColorNotAnimated;

                labelScalingZ.ForeColor = ColorNotAnimated;
                textBoxScaleZ.ForeColor = ColorNotAnimated;

                labelTranslationX.ForeColor = ColorNotAnimated;
                textBoxTransX.ForeColor = ColorNotAnimated;

                labelTranslationY.ForeColor = ColorNotAnimated;
                textBoxTransY.ForeColor = ColorNotAnimated;

                labelTranslationZ.ForeColor = ColorNotAnimated;
                textBoxTransZ.ForeColor = ColorNotAnimated;

                _scale = scale;
                _trans = trans;
                _rot = rot;
            }

            // rotation - more complicated because the display mode can be changed 
            _rotCurrent = rot;
            SetRotation(diffAgainstBaseMatrix);
        }


        protected Color ColorNotAnimated
        {
            get { return Color.Black; }
        }

        protected Color ColorIsAnimated
        {
            get { return Color.Red; }
        }


        // quat4 -> (roll, pitch, yaw)
        private static void QuatToEulerXyz(ref Quaternion q1, out Vector3 outVector)
        {
            // http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
            double sqw = q1.W*q1.W;
            double sqx = q1.X*q1.X;
            double sqy = q1.Y*q1.Y;
            double sqz = q1.Z*q1.Z;
	        double unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
	        double test = q1.X*q1.Y + q1.Z*q1.W;
	        if (test > 0.499*unit) { // singularity at north pole
                outVector.Z = (float)(2 * Math.Atan2(q1.X, q1.W));
                outVector.Y = (float)(Math.PI / 2);
                outVector.X = 0;
		        return;
	        }
	        if (test < -0.499*unit) { // singularity at south pole
                outVector.Z = (float)(-2 * Math.Atan2(q1.X, q1.W));
                outVector.Y = (float)(-Math.PI / 2);
                outVector.X = 0;
		        return;
	        }
            outVector.Z = (float)Math.Atan2(2 * q1.Y * q1.W - 2 * q1.X * q1.Z, sqx - sqy - sqz + sqw);
            outVector.Y = (float)Math.Asin(2 * test / unit);
            outVector.X = (float)Math.Atan2(2 * q1.X * q1.W - 2 * q1.Y * q1.Z, -sqx + sqy - sqz + sqw);
        }


        private void SetRotation(bool diffAgainstBaseMatrix)
        {
            switch ((RotationMode)comboBoxRotMode.SelectedIndex)
            {
                case RotationMode.EulerXyzDegrees:
                case RotationMode.EulerXyzRadians:
                    labelRotationW.Visible = false;
                    textBoxRotW.Visible = false;

                    Vector3 v;
                    QuatToEulerXyz(ref _rotCurrent, out v);

                    if ((RotationMode)comboBoxRotMode.SelectedIndex == RotationMode.EulerXyzDegrees)
                    {
                        v *= 180.0f/(float)Math.PI;
                    }
                    
                    textBoxRotX.Text = v.X.ToString(Format);
                    textBoxRotY.Text = v.Y.ToString(Format);
                    textBoxRotZ.Text = v.Z.ToString(Format);

                    if (diffAgainstBaseMatrix)
                    {
                        Vector3 vbase;
                        QuatToEulerXyz(ref _rot, out vbase);

                        if ((RotationMode)comboBoxRotMode.SelectedIndex == RotationMode.EulerXyzDegrees)
                        {
                            vbase *= 180.0f / (float)Math.PI;
                        }

                        const double epsilon = 1e-5f;
                        if (Math.Abs(v.X - vbase.X) > epsilon)
                        {
                            labelRotationX.ForeColor = ColorIsAnimated;
                            textBoxRotX.ForeColor = ColorIsAnimated;
                        }
                        if (Math.Abs(v.Y - vbase.Y) > epsilon)
                        {
                            labelRotationY.ForeColor = ColorIsAnimated;
                            textBoxRotY.ForeColor = ColorIsAnimated;
                        }
                        if (Math.Abs(v.Z - vbase.Z) > epsilon)
                        {
                            labelRotationZ.ForeColor = ColorIsAnimated;
                            textBoxRotZ.ForeColor = ColorIsAnimated;
                        }
                    }
                    break;

                case RotationMode.Quaternion:
                    labelRotationW.Visible = true;
                    textBoxRotW.Visible = true;

                    textBoxRotX.Text = _rotCurrent.X.ToString(Format);
                    textBoxRotY.Text = _rotCurrent.Y.ToString(Format);
                    textBoxRotZ.Text = _rotCurrent.Z.ToString(Format);
                    textBoxRotW.Text = _rotCurrent.W.ToString(Format);

                    if (diffAgainstBaseMatrix)
                    {
                        const double epsilon = 1e-5f;
                        if (Math.Abs(_rotCurrent.X - _rot.X) > epsilon)
                        {
                            labelRotationX.ForeColor = ColorIsAnimated;
                            textBoxRotX.ForeColor = ColorIsAnimated;
                        }
                        if (Math.Abs(_rotCurrent.Y - _rot.Y) > epsilon)
                        {
                            labelRotationY.ForeColor = ColorIsAnimated;
                            textBoxRotY.ForeColor = ColorIsAnimated;
                        }
                        if (Math.Abs(_rotCurrent.Z - _rot.Z) > epsilon)
                        {
                            labelRotationZ.ForeColor = ColorIsAnimated;
                            textBoxRotZ.ForeColor = ColorIsAnimated;
                        }
                        if (Math.Abs(_rotCurrent.W - _rot.W) > epsilon)
                        {
                            labelRotationW.ForeColor = ColorIsAnimated;
                            textBoxRotW.ForeColor = ColorIsAnimated;
                        }
                    }
                    
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            if(!diffAgainstBaseMatrix)
            {
                textBoxRotX.ForeColor = ColorNotAnimated;
                textBoxRotY.ForeColor = ColorNotAnimated;
                textBoxRotZ.ForeColor = ColorNotAnimated;
                textBoxRotW.ForeColor = ColorNotAnimated;

                labelRotationX.ForeColor = ColorNotAnimated;
                labelRotationY.ForeColor = ColorNotAnimated;
                labelRotationZ.ForeColor = ColorNotAnimated;
                labelRotationW.ForeColor = ColorNotAnimated;
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 