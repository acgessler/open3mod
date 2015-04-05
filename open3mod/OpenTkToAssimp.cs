///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [OpenTkToAssimp.cs]
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

namespace open3mod
{
    /// <summary>
    /// Static utility functions to convert from OpenTk math data types to assimp data types.
    /// </summary>
    public static class OpenTkToAssimp
    {
        public static void FromMatrix(ref Matrix4 mConv, out Matrix4x4 m)
        {
            m.A1 = mConv.M11;
            m.A2 = mConv.M12;
            m.A3 = mConv.M13;
            m.A4 = mConv.M14;

            m.B1 = mConv.M21;
            m.B2 = mConv.M22;
            m.B3 = mConv.M23;
            m.B4 = mConv.M24;

            m.C1 = mConv.M31;
            m.C2 = mConv.M32;
            m.C3 = mConv.M33;
            m.C4 = mConv.M34;

            m.D1 = mConv.M41;
            m.D2 = mConv.M42;
            m.D3 = mConv.M43;
            m.D4 = mConv.M44;
        }


        public static Matrix4x4 FromMatrix(Matrix4 mConv)
        {
            Matrix4x4 m;
            FromMatrix(ref mConv, out m);
            return m;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 