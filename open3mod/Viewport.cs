///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Viewport.cs]
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
    /// <summary>
    /// Encapsulates the state of a 3D viewport. Each tab maintains a list of active viewports.
    /// retained and thus all state preserved.
    /// </summary>
    public class Viewport
    {
        private Vector4 _bounds;
        private CameraMode _camMode;

        /// <summary>
        /// Camera controllers maintain state even when they are not active, 
        /// therefore we need to keep a camera controller for every
        /// view index X camera mode.
        /// </summary>
        private readonly ICameraController[] _cameraImpls = new ICameraController[(int)CameraMode._Max];


        public Viewport(Vector4 bounds, CameraMode camMode)
        {
            _bounds = bounds;
            _camMode = camMode;
        }


        public Vector4 Bounds
        {
            get { return _bounds; }
            set
            {
                Debug.Assert(value.X < value.Z);
                Debug.Assert(value.Y < value.W);
                _bounds = value;
                // TODO verify we don't overlap any other viewports
            }
        }

        public CameraMode CameraMode
        {
            get { return _camMode; }
        }


        public double RelativeWidth
        {
            get { return _bounds.Z - _bounds.X; }
        }


        public double RelativeHeight
        {
            get { return _bounds.W - _bounds.Y; }
        }


        public ICameraController ActiveCameraControllerForView()
        {
            var camMode = _camMode;
            if (_cameraImpls[(int)camMode] == null)
            {
                switch (camMode)
                {
                    case CameraMode.Fps:
                        _cameraImpls[(int)camMode] = new FpsCameraController();
                        break;
                    case CameraMode.X:
                    case CameraMode.Y:
                    case CameraMode.Z:
                    case CameraMode.Orbit:
                        var orbit = new OrbitCameraController(camMode);
                        _cameraImpls[(int)CameraMode.X] = orbit;
                        _cameraImpls[(int)CameraMode.Y] = orbit;
                        _cameraImpls[(int)CameraMode.Z] = orbit;
                        _cameraImpls[(int)CameraMode.Orbit] = orbit;
                        break;
                    case CameraMode.Pick:
                        _cameraImpls[(int)camMode] = new PickingCameraController();
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            return _cameraImpls[(int)camMode];
        }


        public void ChangeCameraModeForView(CameraMode cameraMode)
        {
            if(_camMode == cameraMode)
            {
                return;
            }

            var oldCam = _cameraImpls[(int) _camMode];

            // when changing to the picking camera mode, preserve the old view matrix
            if (cameraMode == CameraMode.Pick)
            {
                Debug.Assert(oldCam != null);
                if (_cameraImpls[(int)cameraMode] == null)
                {
                    _cameraImpls[(int)cameraMode] = new PickingCameraController();
                }
                var picker = (PickingCameraController)_cameraImpls[(int)cameraMode];
                picker.SetView(oldCam.GetView());
            }

            _camMode = cameraMode;

            // special handling to switch the orbit camera controller between the x,y,z and full orbit modes
            if (cameraMode == CameraMode.Z || cameraMode == CameraMode.Y || cameraMode == CameraMode.X || cameraMode == CameraMode.Orbit)
            {
                if (_cameraImpls[(int)CameraMode.Orbit] == null)
                {
                    return;
                }

                var orbit = _cameraImpls[(int)CameraMode.Orbit] as OrbitCameraController;
                Debug.Assert(orbit != null);

                orbit.SetOrbitOrConstrainedMode(cameraMode);
            } 
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 