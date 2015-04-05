///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [OverlaySkeleton.cs]
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
    public static class OverlaySkeleton
    {
        public static void DrawSkeletonBone(Node node, float invGlobalScale, bool highlight)
        {
            var target = new Vector3(node.Transform.A4, node.Transform.B4, node.Transform.C4);
            if (!(target.LengthSquared > 1e-6f))
            {
                return;
            }

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.DepthTest);

            GL.Color4(highlight ? new Color4(0.0f, 1.0f, 0.5f, 1.0f) : new Color4(0.0f, 0.5f, 1.0f, 1.0f));

            var right = new Vector3(1, 0, 0);
            var targetNorm = target;
            targetNorm.Normalize();

            Vector3 up;
            Vector3.Cross(ref targetNorm, ref right, out up);
            Vector3.Cross(ref up, ref targetNorm, out right);

            up *= invGlobalScale;
            right *= invGlobalScale;

            const float jointWidth = 0.03f;

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex3(-jointWidth * up + -jointWidth * right);
            GL.Vertex3(-jointWidth * up + jointWidth * right);
            GL.Vertex3(jointWidth * up + jointWidth * right);
            GL.Vertex3(jointWidth * up + -jointWidth * right);
            GL.End();

            GL.Begin(BeginMode.Lines);
            GL.Vertex3(-jointWidth * up + -jointWidth * right);
            GL.Vertex3(target);
            GL.Vertex3(-jointWidth * up + jointWidth * right);
            GL.Vertex3(target);
            GL.Vertex3(jointWidth * up + jointWidth * right);
            GL.Vertex3(target);
            GL.Vertex3(jointWidth * up + -jointWidth * right);
            GL.Vertex3(target);

            GL.Color4(highlight ? new Color4(1.0f, 0.0f, 0.0f, 1.0f) : new Color4(1.0f, 1.0f, 0.0f, 1.0f));

            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(target);
            GL.End();

            GL.Disable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.DepthTest);
        }

    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 