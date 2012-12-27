using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace open3mod
{
    public class TextureInspectionView
    {
        private readonly Scene _scene;
        private readonly ListView _list;

        public TextureInspectionView(Scene scene, ListView list)
        {
            _scene = scene;
            _list = list;
        }
    }
}
