using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private TextureThumbnailControl _selectedEntry;

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

                    if (_flow.IsHandleCreated)
                    {
                        _flow.BeginInvoke(new SetLabelTextDelegate(SetTextureToLoadedStatus),
                                          new object[] {name, tex}
                            );
                    }
                    else
                    {
                        SetTextureToLoadedStatus(name, tex);
                    }
                });

        }

        public TextureThumbnailControl SelectedEntry
        {
            get { return _selectedEntry; }
        }

        private void AddTextureEntry(string filePath)
        {
            var control = new TextureThumbnailControl(this, _scene, filePath);
            control.Click += (sender, args) =>
            {
                var v = sender as TextureThumbnailControl;
                if (v != null)
                {
                    SelectEntry(v);
                }
            };

            _entries.Add(control);
            _flow.Controls.Add(control);
        }

        private void SelectEntry(TextureThumbnailControl thumb)
        {
            if(thumb == SelectedEntry)
            {
                return;
            }

            thumb.IsSelected = true;

            if (_selectedEntry != null)
            {
                _selectedEntry.IsSelected = false;
            }
            _selectedEntry = thumb;
        }

      

        private void SetTextureToLoadedStatus(string name, Texture tex)
        {
            var control = _entries.Find(con => con.FilePath == name);
            Debug.Assert(control != null);
            control.SetTexture(tex);
        }
    }
}
