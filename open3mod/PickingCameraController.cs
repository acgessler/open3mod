///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [PickingCameraController.cs]
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
    public class PickingCameraController : ICameraController
    {
        private Matrix4 _view;
        public PickingCameraController(Matrix4 view)
        {
            _view = view;    
        }


        public PickingCameraController()
        {
            
        }


        public void SetPivot(Vector3 pivot)
        {}

        public void SetView(Matrix4 view)
        {
            _view = view;
        }


        public Matrix4 GetView()
        {            
            return _view;
        }


        public void Pan(float x, float y)
        {
        }


        public void MovementKey(float x, float y, float z)
        {
        }


        public CameraMode GetCameraMode()
        {
            return CameraMode.Pick;
        }


        public void MouseMove(int x, int y)
        {
        }


        public void Scroll(float z)
        {

        }

        public void LeapInput(float x, float y, float z, float pitch, float roll, float yaw)
        {

        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 