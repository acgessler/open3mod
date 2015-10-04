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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public partial class MeshDetailsDialog : Form, IHoverUpdateDialog
    {
        private const string XyzPosition = "XYZ Position";
        private const string Normals = "Normals";
        private const string Tangents = "Tangents";
        private const string BoneWeights = "Bone Weights";
        private const string VertexAnimation = "Vertex Animation";
        private readonly MainWindow _host;
        private readonly Scene _scene;
        private NormalVectorGeneratorDialog _normalsDialog;

        private Mesh _mesh;
        private String _meshName;

        public MeshDetailsDialog(MainWindow host, Scene scene)
        {
            Debug.Assert(host != null);
            _host = host;
            _scene = scene;

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
            Text = string.Format("{0} - Mesh Editor", meshName);

            UpdateFaceItems();
            UpdateVertexItems();

            // Immediate material update to avoid poll delay.
            UpdateMaterialPreview();
        }


        private void UpdateFaceItems()
        {
            listBoxFaceData.Items.Clear();
            if (_mesh.PrimitiveType.HasFlag(PrimitiveType.Triangle))
            {
                listBoxFaceData.Items.Add("Triangles");
            }
            if (_mesh.PrimitiveType.HasFlag(PrimitiveType.Line))
            {
                listBoxFaceData.Items.Add("Single Lines");
            }
            if (_mesh.PrimitiveType.HasFlag(PrimitiveType.Point))
            {
                listBoxFaceData.Items.Add("Single Points");
            }
        }


        private void UpdateVertexItems()
        {
            object selectedItem = listBoxVertexData.SelectedItem;
            listBoxVertexData.Items.Clear();
            listBoxVertexData.Items.Add(XyzPosition);
            if (_mesh.HasNormals)
            {
                listBoxVertexData.Items.Add(Normals);
            }
            if (_mesh.HasTangentBasis)
            {
                listBoxVertexData.Items.Add(Tangents);
            }

            for (var i = 0; i < _mesh.TextureCoordinateChannelCount; ++i)
            {
                listBoxVertexData.Items.Add(UVCoordinates(i));
            }
            for (var i = 0; i < _mesh.VertexColorChannelCount; ++i)
            {
                listBoxVertexData.Items.Add(VertexColors(i));
            }
            if (_mesh.HasBones)
            {
                listBoxVertexData.Items.Add(BoneWeights);
            }
            if (_mesh.HasMeshAnimationAttachments)
            {
                listBoxVertexData.Items.Add(VertexAnimation);
            }

            // Restore previous selected item.
            foreach (var item in listBoxVertexData.Items)
            {
                if (item.Equals(selectedItem))
                {
                    listBoxVertexData.SelectedItem = item;
                }
            }         
        }

        private static string VertexColors(int i)
        {
            return string.Format("Vertex Color Set (#{0})", i);
        }

        private static string UVCoordinates(int i)
        {
            return string.Format("UV Coordinates (#{0})", i);
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
            if (_normalsDialog != null)
            {
                _normalsDialog.Close();
                _normalsDialog.Dispose();
                _normalsDialog = null;
            }
            _normalsDialog = new NormalVectorGeneratorDialog(_scene, _mesh, _meshName);
            _normalsDialog.Show(this);

            // Disable this dialog for the duration of the NormalsVectorGeneratorDialog.
            // The latter replaces the mesh being displayed with a temporary preview
            // mesh, so changes made to the source mesh would not be visible. This is
            // very confusing, so disallow it.
            EnableDialogControls(false);
            _normalsDialog.FormClosed +=
                (o, args) =>
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    _normalsDialog = null;
                    EnableDialogControls(true);
                    // Unless the user canceled the operation, Normals were added.
                    UpdateVertexItems();
                };
        }

        private void EnableDialogControls(bool enabled)
        {
            // Only update children. If the form itself is disabled, it acts like a
            // blocked dialog and can no longer be moved.
            foreach (Control c in Controls)
            {
                c.Enabled = enabled;
            }
        }

        private void OnDeleteSelectedVertexComponent(object sender, EventArgs e)
        {
            string item = (string) listBoxVertexData.SelectedItem;
            if (item == null || item == XyzPosition)
            {
                return;
            }
            switch (item)
            {
                case Normals:
                    DeleteVertexComponent(Normals, m => m.Normals);
                    break;
                case Tangents:
                    DeleteVertexComponent(Tangents, m => m.Tangents);
                    // TODO(acgessler): We should also delete bitangents.
                    break;
                case VertexAnimation:
                    DeleteVertexComponent(VertexAnimation, m => m.MeshAnimationAttachments);
                    break;
                case BoneWeights:
                    DeleteVertexComponent(BoneWeights, m => m.Bones);
                    break;
                default:
                    for (var i = 0; i < _mesh.TextureCoordinateChannelCount; ++i)
                    {
                        if (item != UVCoordinates(i)) continue;
                        DeleteVertexComponent(UVCoordinates(i), m => m.TextureCoordinateChannels, i);
                        break;
                    }
                    for (var i = 0; i < _mesh.VertexColorChannelCount; ++i)
                    {
                        if (item != VertexColors(i)) continue;
                        DeleteVertexComponent(VertexColors(i), m => m.VertexColorChannels, i);
                        break;
                    }
                    break;
            }
        }

        private void DeleteVertexComponent<T>(string name, Expression<Func<Mesh, T>> property) where T : new()
        {
            var expr = (MemberExpression)property.Body;
            var prop = (PropertyInfo)expr.Member;

            T oldValue = (T)prop.GetValue(_mesh, null);
            var mesh = _mesh;
            _scene.UndoStack.PushAndDo(String.Format("Mesh \"{0}\": delete {1}", _meshName, name),
                () => prop.SetValue(mesh, new T(), null),
                () => prop.SetValue(mesh, oldValue, null),
                () =>
                {
                    _scene.RequestRenderRefresh();
                    if (mesh == _mesh) // Only update UI if the dialog instance still displays the mesh.
                    {
                        UpdateVertexItems();
                    }
                });
        }

        // Version for T[] (TextureCoordChannels, VertexColorChannels). Passing in an indexed expression
        // directly yields a LINQ expression tree rooted at a BinaryExpression instead of MemberExpression.
        // TODO(acgessler): Check for a way to express this with less duplication.
        private void DeleteVertexComponent<T>(string name, Expression<Func<Mesh, T[]>> property, int index) where T : new()
        {
            var expr = (MemberExpression)property.Body;
            var prop = (PropertyInfo)expr.Member;
            var indexes = new object[] { index };

            T oldValue = ((T[])prop.GetValue(_mesh, null))[index];
            var mesh = _mesh;
            _scene.UndoStack.PushAndDo(String.Format("Mesh \"{0}\": delete {1}", _meshName, name),
                () => ((T[])prop.GetValue(_mesh, null))[index] = new T(),
                () => ((T[])prop.GetValue(_mesh, null))[index] = oldValue,
                () =>
                {
                    _scene.RequestRenderRefresh();
                    if (mesh == _mesh)
                    {
                        UpdateVertexItems();
                    }
                });
        }

        private void OnSelectedVertexComponentChanged(object sender, EventArgs e)
        {
            buttonDeleteVertexData.Enabled = !listBoxVertexData.SelectedItem.Equals(XyzPosition);
        }

        public bool HoverUpdateEnabled
        {
            // Disallow Hover Update while any child dialogs are open.
            get { return _normalsDialog == null; }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 