///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [InspectionView.cs]
// (c) 2012-2013, Alexander C. Gessler
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace open3mod
{
    public partial class InspectionView : UserControl
    {
        public Scene Scene { get; private set; }

        public HierarchyInspectionView Hierarchy { get; private set; }
        public TextureInspectionView Textures { get; private set; }
        public MaterialInspectionView Materials { get; private set; }
        public AnimationInspectionView Animations { get; private set; }

        public InspectionView()
        {
            InitializeComponent();
            Enabled = false;
        }


        public void OpenMaterialsTab()
        {
            tabControlInfoViewPicker.SelectedTab = tabPageMaterials;
        }


        /// <summary>
        /// Binds a scene to the InspectionView.
        /// </summary>
        /// <param name="scene">May be null, in this case the inspector remains disabled</param>
        public void SetSceneSource(Scene scene)
        {
            if (Scene == scene)
            {
                return;
            }

            Clear();
            Scene = scene;

            if(scene == null)
            {
                Enabled = false;
                return;
            }

            Enabled = true;
           
            Hierarchy = new HierarchyInspectionView(Scene, tabPageTree);
            Textures = new TextureInspectionView(Scene, textureFlowPanel);
            if(Textures.Empty)
            {
                // disable the texture tab altogether if there are no textures
                // this would need to be changed if there was a way to add 
                // new texture slots later on.
                tabControlInfoViewPicker.TabPages.Remove(tabPageTextures);
            }

            Animations = new AnimationInspectionView(Scene, tabPageAnimations);
            if (Animations.Empty)
            {
                // same for animations
                tabControlInfoViewPicker.TabPages.Remove(tabPageAnimations);
            }

            //
            Materials = new MaterialInspectionView(Scene, ParentForm as MainWindow, materialFlowPanel);
        }


        /// <summary>
        /// Clear the contents of all inspection tabs
        /// </summary>
        private void Clear()
        {
            //treeViewNodeGraph.Nodes.Clear();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 