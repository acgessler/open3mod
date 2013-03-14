/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Describes a right-handed camera in the scene. An important aspect is that
    /// the camera itself is also part of the scenegraph, meaning any values such
    /// as the direction vector are not *absolute*, they can be relative to the coordinate
    /// system defined by the node which corresponds to the camera. This allows for camera
    /// animations.
    /// </summary>
    public sealed class Camera {
        private String m_name;
        private Vector3D m_position;
        private Vector3D m_up;
        private Vector3D m_direction;
        private float m_fieldOfView;
        private float m_clipPlaneNear;
        private float m_clipPlaneFar;
        private float m_aspectRatio;

        /// <summary>
        /// Gets the name of the camera. This corresponds to a node in the
        /// scenegraph with the same name. This node specifies the position of the
        /// camera in the scene hierarchy and can be animated.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the position of the camera relative to the coordinate space defined by
        /// the corresponding node. THe default value is 0|0|0.
        /// </summary>
        public Vector3D Position {
            get {
                return m_position;
            }
        }

        /// <summary>
        /// Gets the 'up' vector of the camera, relative to the coordinate space defined by the
        /// corresponding node. The 'right' vector of the camera is the cross product of the up
        /// and direction vectors. The default value is 0|1|0.
        /// </summary>
        public Vector3D Up {
            get {
                return m_up;
            }
        }

        /// <summary>
        /// Gets the viewing direction of the camera, relative to the coordiante space defined by the corresponding node.
        /// The default value is 0|0|1.
        /// </summary>
        public Vector3D Direction {
            get {
                return m_direction;
            }
        }

        /// <summary>
        /// Gets the half horizontal field of view angle, in radians. The FoV angle is
        /// the angle between the center line of the screen and the left or right border. The default
        /// value is 1/4PI.
        /// </summary>
        public float FieldOfview {
            get {
                return m_fieldOfView;
            }
        }

        /// <summary>
        /// Gets the distance of the near clipping plane from the camera. The value may not
        /// be 0.0f for arithmetic reasons to prevent a division through zero. The default value is 0.1f;
        /// </summary>
        public float ClipPlaneNear {
            get {
                return m_clipPlaneNear;
            }
        }

        /// <summary>
        /// Gets the distance of the far clipping plane from the camera. The far clippling plane must
        /// be further than the near clippling plane. The default value is 1000.0f. The ratio between
        /// the near and far plane should not be too large (between 1000 - 10000 should be ok) to avoid
        /// floating-point inaccuracies which can lead to z-fighting.
        /// </summary>
        public float ClipPlaneFar {
            get {
                return m_clipPlaneFar;
            }
        }

        /// <summary>
        /// Gets the screen aspect ratio. This is the ratio between the width and height of the screen. Typical
        /// values are 4/3, 1/2, or 1/1. This value is 0 if the aspect ratio is not defined in the source file.
        /// The default value is zero.
        /// </summary>
        public float AspectRatio {
            get {
                return m_aspectRatio;
            }
        }

        /// <summary>
        /// Gets a right-handed view matrix.
        /// </summary>
        public Matrix4x4 ViewMatrix {
            get {
                Vector3D zAxis = m_direction;
                zAxis.Normalize();
                Vector3D yAxis = m_up;
                yAxis.Normalize();
                Vector3D xAxis = Vector3D.Cross(m_up, m_direction);
                zAxis.Normalize();

                //Assimp docs *say* they deal with Row major matrices,
                //but aiCamera.h has this calc done with translation in the 4th column
                Matrix4x4 mat;
                mat.A1 = xAxis.X;
                mat.A2 = xAxis.Y;
                mat.A3 = xAxis.Z;
                mat.A4 = 0;

                mat.B1 = yAxis.X;
                mat.B2 = yAxis.Y;
                mat.B3 = yAxis.Z;
                mat.B4 = 0;

                mat.C1 = zAxis.X;
                mat.C2 = zAxis.Y;
                mat.C3 = zAxis.Z;
                mat.C4 = 0;

                mat.D1 = -(Vector3D.Dot(xAxis, m_position));
                mat.D2 = -(Vector3D.Dot(yAxis, m_position));
                mat.D3 = -(Vector3D.Dot(zAxis, m_position));
                mat.D4 = 1.0f;

                return mat;
            }
        }

        /// <summary>
        /// Constructs a new Camera.
        /// </summary>
        /// <param name="camera">Unmanaged aiCamera</param>
        internal Camera(AiCamera camera) {
            m_name = camera.Name.GetString();
            m_position = camera.Position;
            m_direction = camera.LookAt;
            m_up = camera.Up;
            m_fieldOfView = camera.HorizontalFOV;
            m_clipPlaneFar = camera.ClipPlaneFar;
            m_clipPlaneNear = camera.ClipPlaneNear;
            m_aspectRatio = camera.Aspect;
        }
    }
}
