///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MeshInfoPopup.cs]
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

using System.Diagnostics;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class MeshInfoPopup : UserControl
    {
        private HierarchyInspectionView _owner;

        public MeshInfoPopup()
        {
            InitializeComponent();
        }


        public HierarchyInspectionView Owner
        {
            set { _owner = value; }
        }


        /// <summary>
        /// Sets the contents of the info pop-up given an assimp mesh.
        /// 
        /// At the time this method is called, the mesh info popup's
        /// location has already been adjusted by the caller.
        /// </summary>
        public void Populate(Mesh mesh)
        {
            Debug.Assert(mesh != null);
            Debug.Assert(_owner != null);

            labelInfo.Text = string.Format("{0} Vertices\n{1} Faces\n", mesh.VertexCount, mesh.FaceCount);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 