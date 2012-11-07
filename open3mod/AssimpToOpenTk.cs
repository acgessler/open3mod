using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;
using OpenTK.Graphics;

namespace open3mod
{
    /// <summary>
    /// Static utility functions to convert assimp math structures to their OpenTK counterparts
    /// </summary>
    public static class AssimpToOpenTk
    {
        public static Matrix4 FromMatrix(Matrix4x4 mat)
        {
            var m = new Matrix4
            {
                M11 = mat.A1,
                M12 = mat.A2,
                M13 = mat.A3,
                M14 = mat.A4,
                M21 = mat.B1,
                M22 = mat.B2,
                M23 = mat.B3,
                M24 = mat.B4,
                M31 = mat.C1,
                M32 = mat.C2,
                M33 = mat.C3,
                M34 = mat.C4,
                M41 = mat.D1,
                M42 = mat.D2,
                M43 = mat.D3,
                M44 = mat.D4
            };
            return m;
        }

        public static Vector3 FromVector(Vector3D vec)
        {
            Vector3 v;
            v.X = vec.X;
            v.Y = vec.Y;
            v.Z = vec.Z;
            return v;
        }

        public static Color4 FromColor(Color4D color)
        {
            Color4 c;
            c.R = color.R;
            c.G = color.G;
            c.B = color.B;
            c.A = color.A;
            return c;
        }
    }
}
