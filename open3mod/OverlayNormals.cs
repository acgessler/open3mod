///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [OverlayNormals.cs]
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

using Assimp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    public static class OverlayNormals
    {
        public static void DrawNormals(Node node, int meshIndex, Mesh mesh, CpuSkinningEvaluator skinner, float invGlobalScale, Matrix4 transform)
        {
            if (!mesh.HasNormals)
            {
                return;
            }

            // The normal directions are transformed using the transpose(inverse(transform)).
            // This ensures correct direction is used when non-uniform scaling is present.
            Matrix4 normalMatrix = transform;
            normalMatrix.Invert();
            normalMatrix.Transpose();

            // Scale by scene size because the scene will be resized to fit
            // the unit box, but the normals should have a fixed length.
            var scale = invGlobalScale * 0.05f;

            GL.Begin(BeginMode.Lines);

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);

            GL.Color4(new Color4(0.0f, 1.0f, 0.0f, 1.0f));

            for (uint i = 0; i < mesh.VertexCount; ++i)
            {
                Vector3 v;
                if (skinner != null && mesh.HasBones)
                {
                    skinner.GetTransformedVertexPosition(node, mesh, i, out v);
                }
                else
                {
                    v = AssimpToOpenTk.FromVector(mesh.Vertices[(int)i]);
                }
                v = Vector4.Transform(new Vector4(v, 1.0f), transform).Xyz; // Skip dividing by W component. It should always be 1, here.

                Vector3 n;
                if (skinner != null)
                {
                    skinner.GetTransformedVertexNormal(node, mesh, i, out n);
                }
                else
                {
                    n = AssimpToOpenTk.FromVector(mesh.Normals[(int)i]);
                }
                n = Vector4.Transform(new Vector4(n, 0.0f), normalMatrix).Xyz; // Toss the W component. It is non-sensical for normals.
                n.Normalize();

                GL.Vertex3(v);
                GL.Vertex3(v + n * scale);
            }
            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 