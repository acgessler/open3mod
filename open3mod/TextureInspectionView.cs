///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureInspectionView.cs]
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

            var have = new HashSet<string>();
            foreach (var mat in scene.Raw.Materials)
            {
                var textures = mat.GetAllTextures();
                foreach (var tex in textures)
                {
                    if (have.Contains(tex.FilePath))
                    {
                        continue;
                    }

                    have.Add(tex.FilePath);
                    AddTextureEntry(tex.FilePath);                   
                }
            }

            var countdown = have.Count;
            Scene.TextureSet.AddCallback((name, tex) =>
                {
                    // we need to handle this case because texture callbacks may occur late
                    if (_flow.IsDisposed)
                    {
                        return false;
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
                    return --countdown > 0;
                });

        }

        public bool Empty
        {
            get { return _entries.Count == 0; }
        }

        public TextureThumbnailControl SelectedEntry
        {
            get { return _selectedEntry; }
        }

        public Scene Scene
        {
            get { return _scene; }
        }


        private void AddTextureEntry(string filePath)
        {
            var control = new TextureThumbnailControl(this, Scene, filePath);
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
            if(control == null)
            {
                // this can happen if textures have been replaced -
                // when the replacement textures finish loading,
                // we don't have entries for them. This is handled
                // by the corresponding thumbnails themselves.
                return;
            }
            control.SetTexture(tex);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 