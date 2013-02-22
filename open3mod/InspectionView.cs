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
