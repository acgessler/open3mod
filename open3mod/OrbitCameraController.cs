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
            _cameraDistance += z;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            _view = Matrix4.LookAt(_offset * _cameraDistance, Vector3.Zero, Vector3.UnitY);
        }
    }
}
