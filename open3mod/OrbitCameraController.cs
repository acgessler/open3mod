///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [OrbitCameraController.cs]
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
        private readonly Vector3 _right;
        private readonly Vector3 _up;
        private CameraMode _mode;


        private const float ZoomSpeed = 1.00105f;
        private const float MinimumCameraDistance = 0.1f;

        /// <summary>
        /// Rotation speed, in degrees per pixels
        /// </summary>
        private const float RotationSpeed = 0.5f;
        private const float InitialCameraDistance = 3.0f;


        public OrbitCameraController(CameraMode camMode)
        {
            _mode = camMode;

            _view = Matrix4.Identity;
            _viewWithOffset = Matrix4.Identity;

            _cameraDistance = InitialCameraDistance;

            _right = Vector3.UnitX;
            _up = Vector3.UnitY;

            UpdateViewMatrix();            
        }



        public Matrix4 GetView()
        {
            return _viewWithOffset;
        }


        public void MouseMove(int x, int y)
        {
            if (x != 0)
            {
                _view *= Matrix4.CreateFromAxisAngle(_up, (float)(x * RotationSpeed * Math.PI / 180.0));
            }

            if (y != 0)
            {
  
                _view *= Matrix4.CreateFromAxisAngle(_right, (float) (y*RotationSpeed*Math.PI/180.0));
            }
              
            UpdateViewMatrix();
        }


        public void Scroll(int z)
        {
            _cameraDistance *= (float)Math.Pow(ZoomSpeed, -z);
            _cameraDistance = Math.Max(_cameraDistance, MinimumCameraDistance);
            UpdateViewMatrix();
        }


        public void MovementKey(float x, float y, float z)
        {
            // XXX switch to FPS camera at current position?
        }


        public CameraMode GetCameraMode()
        {
            return _mode;
        }


        private void UpdateViewMatrix()
        {
            _viewWithOffset = Matrix4.LookAt(_view.Column2.Xyz * _cameraDistance, Vector3.Zero, _view.Column1.Xyz);
        }


        /// <summary>
        /// Switches the camera controller between the X,Z,Y and Orbit modes.
        /// </summary>
        /// <param name="cameraMode"></param>
        public void SetOrbitOrConstrainedMode(CameraMode cameraMode)
        {
            if(_mode == cameraMode)
            {
                return;
            }
            _mode = cameraMode;
    
            switch(_mode)
            {
                case CameraMode.X:
                    break;
                case CameraMode.Y:
                    break;
                case CameraMode.Z:
                    break;
                case CameraMode.Orbit:
                    break;               
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 