///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [InspectionView.cs]
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


        public void OpenMaterialsTabAndScrollTo(MaterialThumbnailControl thumb)
        {
            Debug.Assert(tabPageMaterials.Contains(thumb));
            tabPageMaterials.ScrollControlIntoView(thumb);
            tabControlInfoViewPicker.SelectedTab = tabPageMaterials;
            tabPageMaterials.Focus();
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

            Scene = scene;
            if(scene == null)
            {
                Enabled = false;
                return;
            }

            Clear();

            Enabled = true;
            Hierarchy = new HierarchyInspectionView(Scene, tabPageTree);
            Textures = new TextureInspectionView(Scene, textureFlowPanel);
            if(Textures.Empty)
            {
                // Disable the texture tab altogether if there are no textures
                // This would need to be changed if there was a way to add 
                // new texture slots later on.
                tabControlInfoViewPicker.TabPages.Remove(tabPageTextures);
            }

            Animations = new AnimationInspectionView(Scene, tabPageAnimations);
            if (Animations.Empty)
            {
                // Same for animations.
                tabControlInfoViewPicker.TabPages.Remove(tabPageAnimations);
            }

            //
            Materials = new MaterialInspectionView(Scene, ParentForm as MainWindow, materialFlowPanel);
        }

        private void Clear()
        {
            tabPageTree.Controls.Clear();
            textureFlowPanel.Controls.Clear();
            tabPageAnimations.Controls.Clear();
            materialFlowPanel.Controls.Clear();
        }

        /// <summary>
        /// Focus the pages, to be able to scroll with the Mouse Wheel.
        /// </summary>
        private void TabPageMaterialsMouseEnter(object sender, EventArgs e)
        {
            tabPageMaterials.Focus();
        }

        private void TabPageTexturesMouseEnter(object sender, EventArgs e)
        {
            tabPageTextures.Focus();
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 