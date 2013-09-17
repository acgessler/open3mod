///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [FpsCameraController.cs]
// (c) 2012-2013, Open3Mod Contributors
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    public class FpsCameraController : ICameraController
    {
        private Matrix4 _view;

        private Matrix4 _orientation;
        private Vector3 _translation;

        private static readonly Vector3 StartPosition = new Vector3(0.0f,1.0f,2.5f);
        private bool _dirty = true;
        private bool _updateOrientation = true;
        private const float MovementBaseSpeed = 1.0f;
        private const float BaseZoomSpeed = 0.002f;
        private const float RotationSpeed = 0.5f;

        private float _pitchAngle = 0.0f;
        private float _rollAngle = 0.0f;
        private float _yawAngle = 0.0f;


        public FpsCameraController()
        {
            _view = Matrix4.Identity;
            _translation = StartPosition;

            UpdateViewMatrix();
        }

        public void SetPivot(Vector3 pivot)
        { }

        public Matrix4 GetView()
        {
            if(_dirty)
            {
                UpdateViewMatrix();
            }
            return _view;
        }


        public void Pan(float x, float y)
        {
            

        }


        public void MovementKey(float x, float y, float z)
        {
            var v = new Vector3(x, y, z) * MovementBaseSpeed;
            var o = GetOrientation();

            // TODO: somehow, the matrix is transposed so the normal TransformVector() API does not work.
            // It seems we messed up with OpenTK's matrix conventions.
            _translation += v.X * o.Row0.Xyz + v.Y * o.Row1.Xyz + v.Z * o.Row2.Xyz;
            _dirty = true;
        }


        public CameraMode GetCameraMode()
        {
            return CameraMode.Fps;
        }


        public void MouseMove(int x, int y)
        {
            if (y != 0)
            {
                _pitchAngle += (float)(-y * RotationSpeed * Math.PI / 180.0);
            }

            if (x != 0)
            {
                _yawAngle += (float)(-x * RotationSpeed * Math.PI / 180.0);
            }

            _dirty = true;
            _updateOrientation = true;
        }


        public void Scroll(float z)
        {
            var o = GetOrientation();
            _translation -= o.Row2.Xyz *z*BaseZoomSpeed;
            _dirty = true;
        }


        private void UpdateViewMatrix()
        {
            // Derivation:
            //     view = (orientation*translation)^-1
            // =>  view = translation^-1 * orientation^-1
            // =>  view = translation^-1 * orientation^T      ; orientation is ONB
            // where translation^-1 is simply the negated translation vector.
            _view = GetOrientation();
            _view *= Matrix4.CreateFromAxisAngle(_view.Row0.Xyz, _pitchAngle);
            _view.Transpose();

            _view = Matrix4.CreateTranslation(-_translation) * _view;
            _dirty = false;
        }

        private Matrix4 GetOrientation()
        {
            if (_updateOrientation)
            {
                _updateOrientation = false;
                _orientation = Matrix4.CreateFromAxisAngle(Vector3.UnitY, _yawAngle);
                _orientation *= Matrix4.CreateFromAxisAngle(_orientation.Row0.Xyz, _pitchAngle);   
            }
            return _orientation;
        }

        public void LeapInput(float x, float y, float z, float pitch, float roll, float yaw)
        {
            Scroll(-z);
            //TODO parameters in Settings
            _pitchAngle += pitch * 0.05f;
            _yawAngle += -yaw * 0.05f;
            _updateOrientation = true;

            UpdateViewMatrix();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 