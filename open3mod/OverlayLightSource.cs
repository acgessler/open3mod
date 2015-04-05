///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [OverlayLightSource.cs]
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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    public static class OverlayLightSource
    {
        public static void DrawLightSource(Vector3 dir)
        {
            

            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.CullFace);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            GL.Begin(BeginMode.Lines);

            GL.Color4(new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            GL.Vertex3(dir * 2.0f);

            GL.Color4(new Color4(1.0f, 0.0f, 0.0f, 1.0f));
            GL.Vertex3(dir * 2.1f);

            GL.End();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 