///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [OverlayBoundingBox.cs]
// (c) 2012-2015, Open3Mod Contributors
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
using Assimp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    public static class OverlayBoundingBox
    {
        public static void DrawBoundingBox(Node node, int meshIndex, Mesh mesh, CpuSkinningEvaluator skinner)
        {
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Color4(new Color4(1.0f, 0.0f, 0.0f, 1.0f));

            var min = new Vector3(1e10f, 1e10f, 1e10f);
            var max = new Vector3(-1e10f, -1e10f, -1e10f);
            for (uint i = 0; i < mesh.VertexCount; ++i)
            {
                Vector3 tmp;
                if (skinner != null && mesh.HasBones)
                {
                    skinner.GetTransformedVertexPosition(node, mesh, i, out tmp);
                }
                else
                {
                    tmp = AssimpToOpenTk.FromVector(mesh.Vertices[(int)i]);
                }

                min.X = Math.Min(min.X, tmp.X);
                min.Y = Math.Min(min.Y, tmp.Y);
                min.Z = Math.Min(min.Z, tmp.Z);

                max.X = Math.Max(max.X, tmp.X);
                max.Y = Math.Max(max.Y, tmp.Y);
                max.Z = Math.Max(max.Z, tmp.Z);
            }

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(min);
            GL.Vertex3(new Vector3(min.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(min.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(min.X, min.Y, max.Z));
            GL.End();

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(new Vector3(max.X, min.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, min.Y, max.Z));
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(min);
            GL.Vertex3(new Vector3(max.X, min.Y, min.Z));

            GL.Vertex3(new Vector3(min.X, max.Y, min.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, min.Z));

            GL.Vertex3(new Vector3(min.X, max.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, max.Y, max.Z));

            GL.Vertex3(new Vector3(min.X, min.Y, max.Z));
            GL.Vertex3(new Vector3(max.X, min.Y, max.Z));
            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
        }      
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 