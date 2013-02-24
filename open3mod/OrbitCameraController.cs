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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace open3mod
{
    public class OrbitCameraController : ICameraController
    {
        private Matrix4 _view;
        private Vector3 _offset;
        private float _cameraDistance;


        private const float ZoomSpeed = 1.00105f;
        private const float MinimumCameraDistance = 0.1f;

        /// <summary>
        /// Rotation speed, in degrees per pixels
        /// </summary>
        private const float RotationSpeed = 0.5f;
        private const float InitialCameraDistance = 10.0f;


        public OrbitCameraController()
        {
            _view = Matrix4.Identity;
            _offset = Vector3.UnitZ;
            _cameraDistance = InitialCameraDistance;

            UpdateViewMatrix();
        }



        public Matrix4 GetView()
        {
            return _view;
        }

        public void MouseMove(int x, int y)
        {          
            if (y != 0)
            {
                _offset = Vector3.TransformNormal(_offset, Matrix4.CreateFromAxisAngle(
                    Vector3.Cross(_offset, Vector3.UnitY), (float)(y * RotationSpeed * Math.PI / 180.0)));
            }

            if (x != 0)
            {
                _offset = Vector3.TransformNormal(_offset, Matrix4.CreateFromAxisAngle(
                    Vector3.UnitY, (float)(-x * RotationSpeed * Math.PI / 180.0)));
            }

            if (_offset.Y > 0.8f)
            {
                _offset.Y = 0.8f;
            }
            if (_offset.Y < -0.8f)
            {
                _offset.Y = -0.8f;
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

        private void UpdateViewMatrix()
        {
            _view = Matrix4.LookAt(_offset * _cameraDistance, Vector3.Zero, Vector3.UnitY);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 