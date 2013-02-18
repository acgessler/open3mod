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

        private delegate void SetLabelTextDelegate(string name, Texture tex);

        public TextureInspectionView(Scene scene, ListView list)
        {
            _scene = scene;
            _list = list;

            foreach (var mat in scene.Raw.Materials)
            {
                var textures = mat.GetAllTextures();
                foreach (var tex in textures)
                {
                    _list.Items.Add(tex.FilePath,tex.FilePath,-1);
                }
            }

            _scene.TextureSet.AddCallback((name, tex) => _list.BeginInvoke(new SetLabelTextDelegate(SetTextureToLoadedStatus),
                new object[]{
                    name, tex
                }
        ));
        }

        private void SetTextureToLoadedStatus(string name, Texture tex)
        {
            _list.Items[name].Text += " LOADED";
        }
    }
}
