///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [MaterialInspectionView.cs]
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Assimp;

namespace open3mod
{
    public class MaterialInspectionView : ThumbnailViewBase<MaterialThumbnailControl>
    {
        private readonly Scene _scene;
        private readonly MainWindow _window;

        private delegate void SetLabelTextDelegate(string name, Texture tex);

        public MaterialInspectionView(Scene scene, MainWindow window, FlowLayoutPanel flow)
            : base(flow)
        {
            _scene = scene;
            _window = window;

            foreach (var mat in scene.Raw.Materials)
            {
                var dependencies = new HashSet<string>();
                var textures = mat.GetAllMaterialTextures();
                foreach (var tex in textures)
                {      
                    dependencies.Add(tex.FilePath);                  
                }

                AddMaterialEntry(mat, dependencies);                
            }         
        }
      

        public Scene Scene
        {
            get { return _scene; }
        }


        public MainWindow Window
        {
            get { return _window; }
        }


        private void AddMaterialEntry(Material material, HashSet<string> dependencies)
        {
            var entry = AddEntry(new MaterialThumbnailControl(this, Scene, material));
            if(dependencies.Count == 0)
            {
                return;
            }
            // listen for changes to any textures that this material depends on
            var changeHandler = new TextureSet.TextureCallback((name, tex) =>
            {
                // we need to handle this case because texture callbacks may occur late
                if (Flow.IsDisposed)
                {
                    return false;
                }

                if (dependencies.Contains(name))
                {
                    entry.UpdatePreview();
                    dependencies.Add(tex.FileName);
                }

                return true;
            });

            Scene.TextureSet.AddCallback(changeHandler);
            Scene.TextureSet.AddReplaceCallback(changeHandler); 
        }


        /// <summary>
        /// Select the entry pertaining to a given material.
        /// </summary>
        /// <param name="thumb">Material to select. This material *must*
        ///    have a corresponding entry.</param>
        public void SelectEntry(Material thumb)
        {
            foreach(var v in Entries)
            {
                if(v.Material == thumb)
                {
                    SelectEntry(v);
                    return;
                }
            }    
            Debug.Assert(false);
        }

        /// <summary>
        /// Searches for the corresponding control of a material
        /// </summary>
        /// <param name="mat">the material to search for</param>
        /// <returns>the MaterialThumbnailControl, if the material exists in this view or else null</returns>
        public MaterialThumbnailControl GetMaterialControl(Material mat)
        {
            foreach (var v in Entries)
            {
                if (v.Material == mat)
                {
                    return v;
                }
            }
            return null;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 