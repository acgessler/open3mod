using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Visualizes the translation, scaling and rotation parts of a transformation
    /// matrix.
    /// </summary>
    public partial class TrafoMatrixViewControl : UserControl
    {
        private Quaternion _rot;
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
        }


        /// <summary>
        /// Update the display. This involves decomposing the matrix and is
        /// therefore an expensive operation.
        /// </summary>
        /// <param name="mat"></param>
        public void SetMatrix(ref Matrix4x4 mat)
        {
            // use assimp math data structures because they have Decompose()
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

            // rotation
            _rot = rot;
            SetRotation();
        }


        private void SetRotation()
        {
            switch((RotationMode)comboBoxRotMode.SelectedIndex)
            {
                case RotationMode.EulerXyzDegrees:           
                case RotationMode.EulerXyzRadians:
                    labelRotationW.Visible = false;
                    textBoxRotW.Visible = false;

                    // TODO
                    break;

                case RotationMode.Quaternion:
                    labelRotationW.Visible = true;
                    textBoxRotW.Visible = true;

                    textBoxRotX.Text = _rot.X.ToString(Format);
                    textBoxRotY.Text = _rot.Y.ToString(Format);
                    textBoxRotZ.Text = _rot.Z.ToString(Format);
                    textBoxRotW.Text = _rot.W.ToString(Format);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }


        private void OnUpdateRotation(object sender, EventArgs e)
        {
            CoreSettings.CoreSettings.Default.DefaultRotationMode = comboBoxRotMode.SelectedIndex;
            SetRotation();
        }
    }
}
