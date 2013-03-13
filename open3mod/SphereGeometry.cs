using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    /// <summary>
    /// Utility class that generates vertices and indices for drawing spheres.
    /// 
    /// This code is almost completely taken from http://www.opentk.com/node/1800
    /// </summary>
    public static class SphereGeometry
    {
        public struct Vertex
        { // mimic InterleavedArrayFormat.T2fN3fV3f
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Position;
        }

        /// <summary>
        /// Draws a sphere using Gl immediate mode.
        /// 
        /// Makes no chances to Gl draw/material/matrix/lighting/.. states.
        /// </summary>
        /// <param name="sphereVertices">Array of vertices previously obtained
        ///    from a call to CalculateVertices()</param>
        /// <param name="sphereElements">Array of indices previous obtained
        ///    from a call to CalculateElements()</param>
        public static void Draw(Vertex[] sphereVertices, ushort[] sphereElements)
        {
            GL.Begin(BeginMode.Triangles);
            foreach (var element in sphereElements)
            {
                var vertex = sphereVertices[element];
                GL.TexCoord2(vertex.TexCoord);
                GL.Normal3(vertex.Normal);
                GL.Vertex3(vertex.Position);
            }
            GL.End();
        }


        public static Vertex[] CalculateVertices(float radius, float height, byte segments, byte rings)
        {
            var data = new Vertex[segments * rings];
            var i = 0;

            for (double y = 0; y < rings; y++)
            {
                var phi = (y / (rings - 1)) * Math.PI; //was /2 
                for (double x = 0; x < segments; x++)
                {
                    var theta = (x / (segments - 1)) * 2 * Math.PI;

                    var v = new Vector3()
                    {
                        X = (float)(radius * Math.Sin(phi) * Math.Cos(theta)),
                        Y = (float)(height * Math.Cos(phi)),
                        Z = (float)(radius * Math.Sin(phi) * Math.Sin(theta)),
                    };
                    var n = Vector3.Normalize(v);
                    var uv = new Vector2()
                    {
                        X = (float)(x / (segments - 1)),
                        Y = (float)(y / (rings - 1))
                    };
                    // Using data[i++] causes i to be incremented multiple times in Mono 2.2 (bug #479506).
                    data[i] = new Vertex() { Position = v, Normal = n, TexCoord = uv };
                    i++;
                }

            }

            return data;
        }


        public static ushort[] CalculateElements(byte segments, byte rings)
        {
            var numVertices = segments * rings;
            var data = new ushort[numVertices * 6];

            ushort i = 0;

            for (byte y = 0; y < rings - 1; y++)
            {
                for (byte x = 0; x < segments - 1; x++)
                {
                    data[i++] = (ushort)((y + 0) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x);
                    data[i++] = (ushort)((y + 1) * segments + x + 1);

                    data[i++] = (ushort)((y + 1) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x + 1);
                    data[i++] = (ushort)((y + 0) * segments + x);
                }
            }
            return data;
        }
    }
}
