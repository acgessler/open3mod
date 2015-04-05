///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [AssimpToOpenTk.cs]
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

namespace open3mod
{
    /// <summary>
    /// Static utility functions to convert from assimp math data types to OpenTk data types.
    /// </summary>
    public static class AssimpToOpenTk
    {
        public static Matrix4 FromMatrix(Matrix4x4 mat)
        {
            return FromMatrix(ref mat);
        }

        public static Matrix4 FromMatrix(ref Matrix4x4 mat)
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

/* vi: set shiftwidth=4 tabstop=4: */ 