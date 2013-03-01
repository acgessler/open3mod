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

        public InspectionView()
        {
            InitializeComponent();
        }


        public void SetSceneSource(Scene scene)
        {
            if (Scene == scene)
            {
                return;
            }

            Clear();
            Scene = scene;

            Hierarchy = new HierarchyInspectionView(Scene, treeViewNodeGraph);
            Textures = new TextureInspectionView(Scene, textureFlowPanel);
            if(Textures.Empty)
            {
                // disable the texture tab altogether if there are no textures
                // this would need to be changed if there was a way to add 
                // new texture slots later on.
                tabControlInfoViewPicker.TabPages.Remove(tabPageTextures);
            }
            Materials = new MaterialInspectionView(Scene, null);

            UpdateStatistics();
        }


        /// <summary>
        /// Clear the contents of all inspection tabs
        /// </summary>
        private void Clear()
        {
            treeViewNodeGraph.Nodes.Clear();
        }


        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            Hierarchy.UpdateFilters();
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            labelNodeStats.Text = string.Format("Showing {0} of {1} nodes ({2} meshes, {3} instances)", 
                Hierarchy.CountVisible, 
                Hierarchy.CountNodes, 
                Hierarchy.CountVisibleMeshes, 
                Hierarchy.CountVisibleInstancedMeshes);
        }

    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 