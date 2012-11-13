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

        private static readonly Vector3 StartPosition = new Vector3(0.0f,2.0f,10.0f);
        private const float MovementBaseSpeed = 1.0f;
        private const float BaseZoomSpeed = 0.005f;
        private const float RotationSpeed = 0.5f;


        public FpsCameraController()
        {
            _view = Matrix4.Identity;
            _orientation = Matrix4.Identity;

            _translation = StartPosition;

            UpdateViewMatrix();
        }


        public Matrix4 GetView()
        {
            return _view;
        }

        public void MovementKey(float x, float y, float z)
        {
            var v = new Vector3(x, y, z) * MovementBaseSpeed;
            Vector3.Transform(ref v, ref _orientation, out v);
            _translation += v;
        }

        public void MouseMove(int x, int y)
        {
            if (y != 0)
            {
                _orientation *= Matrix4.CreateFromAxisAngle(Vector3.Cross(new Vector3(_orientation.Column0), Vector3.UnitY),
                    (float)(y * RotationSpeed * Math.PI / 180.0));
            }

            if (x != 0)
            {
                _orientation *= Matrix4.CreateFromAxisAngle(
                    Vector3.UnitY, (float)(-x * RotationSpeed * Math.PI / 180.0));
            }

            UpdateViewMatrix();
        }

        public void Scroll(int z)
        {
            _translation += new Vector3(_orientation.Column2) *z*BaseZoomSpeed;
        }


        private void UpdateViewMatrix()
        {
            _view = _orientation;
            _view.Transpose();

            _view.M41 = -_translation.X;
            _view.M42 = -_translation.Y;
            _view.M43 = -_translation.Z;
        }
    }
}
