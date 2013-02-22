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
        private readonly FlowLayoutPanel _flow;
        private readonly List<TextureThumbnailControl> _entries; 

        private delegate void SetLabelTextDelegate(string name, Texture tex);

        public TextureInspectionView(Scene scene, FlowLayoutPanel flow)
        {
            _scene = scene;
            _flow = flow;
            _entries = new List<TextureThumbnailControl>();

            foreach (var mat in scene.Raw.Materials)
            {
                var textures = mat.GetAllTextures();
                foreach (var tex in textures)
                {
                    AddTextureEntry(tex.FilePath);
                }
            }

            _scene.TextureSet.AddCallback((name, tex) =>
                {
                    // we need to handle this case because texture callbacks may occur late
                    if (_flow.IsDisposed)
                    {
                        return;
                    }
                    _flow.BeginInvoke(new SetLabelTextDelegate(SetTextureToLoadedStatus),
                            new object[] {name, tex}
                        );
                });

        }

        private void AddTextureEntry(string filePath)
        {
            var control = new TextureThumbnailControl(this, _scene, filePath);
            _entries.Add(control);
            _flow.Controls.Add(control);
        }

        private void SetTextureToLoadedStatus(string name, Texture tex)
        {
            //var control = _entries.Find(control => control.FilePath == name);
        }
    }
}
