///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MeshDetailsDialog.cs]
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
using System.Diagnostics;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class MeshDetailsDialog : Form
    {     
        private readonly MainWindow _host;
        private readonly Scene _scene;
        private readonly NormalVectorGeneratorDialog _normalsDialog;

        private Mesh _mesh;
        private String _meshName;

        public MeshDetailsDialog(MainWindow host, Scene scene)
        {
            Debug.Assert(host != null);
            _host = host;
            _scene = scene;
            _normalsDialog = new NormalVectorGeneratorDialog(_scene);

            InitializeComponent();
            // TODO(acgessler): Factor out preview generation and getting the checker pattern
            // background into a separate utility.
            pictureBoxMaterial.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxMaterial.BackgroundImage = MaterialThumbnailControl.GetBackgroundImage();
            pictureBoxMaterial.BackgroundImageLayout = ImageLayout.Zoom;

            StartUpdateMaterialPreviewLoop();
        }


        /// <summary>
        /// Set current mesh for which detail information is displayed.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="meshName"></param>
        public void SetMesh(Mesh mesh, string meshName) 
        {       
            Debug.Assert(mesh != null);        
            Debug.Assert(meshName != null);
            if (mesh == _mesh)
            {
                return;
            }

            _mesh = mesh;
            _meshName = meshName;

            labelVertexCount.Text = mesh.VertexCount.ToString();
            labelFaceCount.Text = mesh.FaceCount.ToString();
            labelBoneCount.Text = mesh.BoneCount.ToString();
            Text = string.Format("{0} - Details", meshName);

            checkedListBoxPerFace.CheckOnClick = false;
            checkedListBoxPerFace.SetItemCheckState(0,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Triangle)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerFace.SetItemCheckState(1,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Line)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerFace.SetItemCheckState(2,
                mesh.PrimitiveType.HasFlag(PrimitiveType.Point)
                ? CheckState.Checked
                : CheckState.Unchecked);

            checkedListBoxPerVertex.CheckOnClick = false;
            checkedListBoxPerVertex.SetItemCheckState(0, CheckState.Checked);
            checkedListBoxPerVertex.SetItemCheckState(1, mesh.HasNormals
                ? CheckState.Checked
                : CheckState.Unchecked);
            checkedListBoxPerVertex.SetItemCheckState(2, mesh.HasTangentBasis
                ? CheckState.Checked
                : CheckState.Unchecked);

            Debug.Assert(mesh.TextureCoordinateChannels.Length >= 4);
            for (var i = 0; i < 4; ++i)
            {
                checkedListBoxPerVertex.SetItemCheckState(3 + i, mesh.HasTextureCoords(i)
                    ? CheckState.Checked
                    : CheckState.Unchecked);
            }

            Debug.Assert(mesh.VertexColorChannels.Length >= 4);
            for (var i = 0; i < 4; ++i)
            {
                checkedListBoxPerVertex.SetItemCheckState(7 + i, mesh.HasVertexColors(i)
                    ? CheckState.Checked
                    : CheckState.Unchecked);
            }
            checkedListBoxPerVertex.SetItemCheckState(11, mesh.HasBones
                ? CheckState.Checked
                : CheckState.Unchecked);

            // Immediate material update to avoid poll delay.
            UpdateMaterialPreview();

            // Update the mesh in child dialogs.
            if (_normalsDialog.Visible)
            {
                _normalsDialog.SetMesh(_mesh, _meshName);
            }
        }


        /// <summary>
        /// Locate the tab within which the current mesh is.
        /// </summary>
        /// <returns></returns>
        private Tab GetTabForCurrentMesh()
        {      
            if (_mesh == null)
            {
                return null;
            }
            Debug.Assert(_host != null);
            foreach (var tab in _host.UiState.TabsWithActiveScenes())
            {
                var scene = tab.ActiveScene;
                Debug.Assert(scene != null);

                for (var i = 0; i < scene.Raw.MeshCount; ++i)
                {
                    var m = scene.Raw.Meshes[i];
                    if (m == _mesh)
                    {
                        return tab;

                    }
                }
            }
            return null;
        }

        private void StartUpdateMaterialPreviewLoop()
        {
            // Unholy poll. This is the only case where material previews are
            // needed other than the material panel itself.
            MainWindow.DelayExecution(new TimeSpan(0, 0, 0, 0, 1000),
                () =>
                {
                    UpdateMaterialPreview();
                    StartUpdateMaterialPreviewLoop();
                });
        }

        private void UpdateMaterialPreview()
        {
            var tab = GetTabForCurrentMesh();
            if (tab == null)
            {
                pictureBoxMaterial.Image = null;
                labelMaterialName.Text = "None";
                return;
            }

            var scene = tab.ActiveScene;
            var mat = scene.Raw.Materials[_mesh.MaterialIndex];
            var ui = _host.UiForTab(tab);
            Debug.Assert(ui != null);

            var inspector = ui.GetInspector();
            var thumb = inspector.Materials.GetMaterialControl(mat);
            pictureBoxMaterial.Image = thumb.GetCurrentPreviewImage();
            labelMaterialName.Text = mat.Name.Length > 0 ? mat.Name : "Unnamed Material";
        }


        private void OnJumpToMaterial(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = false;

            var tab = GetTabForCurrentMesh();
            if (tab == null)
            {
                return;
            }
            var scene = tab.ActiveScene;
            var mat = scene.Raw.Materials[_mesh.MaterialIndex];
            var ui = _host.UiForTab(tab);
            Debug.Assert(ui != null);

            var inspector = ui.GetInspector();
            inspector.Materials.SelectEntry(mat);
            var thumb = inspector.Materials.GetMaterialControl(mat);
            inspector.OpenMaterialsTabAndScrollTo(thumb);
        }

        private void OnGenerateNormals(object sender, EventArgs e)
        {
            if (!_normalsDialog.Visible)
            {
                _normalsDialog.SetMesh(_mesh, _meshName);
                _normalsDialog.ShowDialog(this);
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 