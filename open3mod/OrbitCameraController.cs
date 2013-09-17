///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [OrbitCameraController.cs]
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    public class OrbitCameraController : ICameraController
    {
        private Matrix4 _view;
        private Matrix4 _viewWithOffset;
        private float _cameraDistance;
        private float _pitchAngle = 0.8f;
        private float _rollAngle = 0.0f;
        private readonly Vector3 _right;
        private readonly Vector3 _up;
        private readonly Vector3 _front;
        private CameraMode _mode;

        private Vector3 _panVector;

        private bool _dirty = true;

        private const float ZoomSpeed = 1.00105f;
        private const float MinimumCameraDistance = 0.1f;

        /// <summary>
        /// Rotation speed, in degrees per pixels
        /// </summary>
        private const float RotationSpeed = 0.5f;
        private const float PanSpeed = 0.004f;
        private const float InitialCameraDistance = 3.0f;

        private Vector3 _pivot;


        public OrbitCameraController(CameraMode camMode)
        {
            _view = Matrix4.CreateFromAxisAngle(new Vector3(0.0f, 1.0f, 0.0f), 0.9f);

            _viewWithOffset = Matrix4.Identity;

            _cameraDistance = InitialCameraDistance;

            _right = Vector3.UnitX;
            _up = Vector3.UnitY;
            _front = Vector3.UnitZ;

            SetOrbitOrConstrainedMode(camMode, true);           
        }


        public void SetPivot(Vector3 pivot)
        {
            _pivot = pivot;
            _dirty = true;
        }



        public Matrix4 GetView()
        {
            if (_dirty)
            {
                UpdateViewMatrix();
            }
            return _viewWithOffset;
        }


        public void MouseMove(int x, int y)
        {
            if(x == 0 && y == 0)
            {
                return;
            }

            if (x != 0)
            {
                _view *= Matrix4.CreateFromAxisAngle(_up, (float)(x * RotationSpeed * Math.PI / 180.0));
            }

            if (y != 0)
            {
                _pitchAngle += (float)(y * RotationSpeed * Math.PI / 180.0);
            }

            _dirty = true;

            // leave the X,Z,Y constrained camera modes if we were in any of them
            SetOrbitOrConstrainedMode(CameraMode.Orbit);
        }


        public void Scroll(float z)
        {
            _cameraDistance *= (float)Math.Pow(ZoomSpeed, -z);
            _cameraDistance = Math.Max(_cameraDistance, MinimumCameraDistance);
            _dirty = true;
        }


        public void Pan(float x, float y)
        {
            _panVector.X += x * PanSpeed;
            _panVector.Y += -y * PanSpeed;

            _dirty = true;
        }


        public void MovementKey(float x, float y, float z)
        {
            // TODO switch to FPS camera at current position?
        }


        public CameraMode GetCameraMode()
        {
            return _mode;
        }


        private void UpdateViewMatrix()
        {
            var viewWithPitchAndRoll = _view * Matrix4.CreateFromAxisAngle(_right, _pitchAngle) * Matrix4.CreateFromAxisAngle(_front, _rollAngle);
            _viewWithOffset = Matrix4.LookAt(viewWithPitchAndRoll.Column2.Xyz * _cameraDistance + _pivot, _pivot, viewWithPitchAndRoll.Column1.Xyz);
            _viewWithOffset *= Matrix4.CreateTranslation(_panVector);

            _dirty = false;
        }


        /// <summary>
        /// Switches the camera controller between the X,Z,Y and Orbit modes.
        /// </summary>
        /// <param name="cameraMode">One of the X,Z,Y,Orbit modes</param>
        /// <param name="init">Do not use</param>
        public void SetOrbitOrConstrainedMode(CameraMode cameraMode, bool init = false)
        {
            if(_mode == cameraMode && !init)
            {
                return;
            }
            _mode = cameraMode;
    
            switch(_mode)
            {
                case CameraMode.X:
                    _view = new Matrix4(
                        0,0,1,0,
                        0,1,0,0,
                       -1,0,0,0,
                        0,0,0,1
                        ); 
                    break;
                case CameraMode.Y:
                    _view = new Matrix4(
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        1, 0, 0, 0,
                        0, 0, 0, 1
                        ); 
                    break;
                case CameraMode.Z:
                    _view = new Matrix4(
                        1, 0, 0, 0,
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1
                        ); 
                    break;
                case CameraMode.Orbit:
                    // leave _view unchanged 
                    break;               
                default:
                    Debug.Assert(false);
                    break;
            }

            //reset rotionangles if we switched to one of the constrained views
            if (_mode != CameraMode.Orbit)
            {
                _pitchAngle = 0.0f;
                _rollAngle = 0.0f;
            }

            _dirty = true;
        }

        public void LeapInput(float x, float y, float z, float pitch, float roll, float yaw)
        {
            // TODO Parameters in Settings dialog 
            _pitchAngle = pitch * 3.0f;
            _rollAngle = roll * 1.0f;
            Matrix4 yawrotation = Matrix4.CreateFromAxisAngle(_up, (float)(x * 0.125 * Math.PI / 180.0));
            _view *= yawrotation;

            //Zoom with hands movement in a forward direction ( Z axis )
            Scroll(z * 3.0f);

            _dirty = true;

            // leave the X,Z,Y constrained camera modes if we were in any of them
            SetOrbitOrConstrainedMode(CameraMode.Orbit);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 