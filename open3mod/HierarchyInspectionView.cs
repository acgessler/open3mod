using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace open3mod
{
    /// <summary>
    /// Populates the tree view in the scene inspector that shows the
    /// scene hierarchy and allows selection of arbitrary nodes to 
    /// limit the rendering to them and their children.
    /// </summary>
    public class HierarchyInspectionView
    {
        private readonly Scene _scene;
        private readonly TreeView _tree;

        public HierarchyInspectionView(Scene scene, TreeView tree)
        {
            _scene = scene;
            _tree = tree;
        }
    }
}
