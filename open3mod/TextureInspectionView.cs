///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureInspectionView.cs]
// (c) 2012-2013, Open3Mod Contributors
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
using System.IO;
using System.Linq;
using System.Text;


using System.Windows.Forms;

namespace open3mod
{
    public class TextureInspectionView : ThumbnailViewBase<TextureThumbnailControl>
    {
        private readonly Scene _scene;
        private TextureDetailsDialog _details;

        private delegate void SetLabelTextDelegate(string name, Texture tex);

        public TextureInspectionView(Scene scene, FlowLayoutPanel flow)
            : base(flow)
        {
            _scene = scene;
           
            var have = new HashSet<string>();
            foreach (var mat in scene.Raw.Materials)
            {
                var textures = mat.GetAllMaterialTextures();
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
                    if (Flow.IsDisposed)
                    {
                        return false;
                    }

                    if (Flow.IsHandleCreated)
                    {
                        Flow.BeginInvoke(new SetLabelTextDelegate(SetTextureToLoadedStatus),
                                          new object[] {name, tex}
                            );
                    }
                    else
                    {
                        SetTextureToLoadedStatus(name, tex);
                    }
                    return --countdown > 0;
                });

            
            flow.AllowDrop = true;
            flow.DragDrop += (sender, args) =>
                {
                    try
                    {
                        var a = (Array) args.Data.GetData(DataFormats.FileDrop, false);

                        if (a != null)
                        {
                            string s = a.GetValue(0).ToString();
                            if (!Directory.Exists(s))
                            {
                                MatchWithFolder(s);
                                return;
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error in DragDrop function: " + ex.Message);
                    }
                };
        }
      

        public Scene Scene
        {
            get { return _scene; }
        }


        private void AddTextureEntry(string filePath)
        {
            var t = new TextureThumbnailControl(this, Scene, filePath);
            AddEntry(t);

            TextureThumbnailControl old = null;
            t.MouseEnter += (sender, args) =>
                {
                    if (_details != null)
                    {
                        old = _details.GetTexture();
                        _details.SetTexture(t);
                    }
                };
            t.MouseLeave += (sender, args) =>
                {
                    if (_details != null && old != null)
                    {
                        _details.SetTexture(old);
                        old = null;
                    }
                };
        }
   

        private void SetTextureToLoadedStatus(string name, Texture tex)
        {
            var control = Entries.Find(con => con.FilePath == name);
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


        public void ShowDetails(TextureThumbnailControl textureThumbnailControl)
        {
            if(_details == null)
            {
                _details = new TextureDetailsDialog();
                _details.Closed += (sender, args) =>
                    {
                        _details = null;
                    };
            }
            _details.SetTexture(textureThumbnailControl);
            _details.Show();
        }


        /// <summary>
        /// Given a folder name, try to resolve all FAILED textures using the textures
        /// in that folder. Matching is only for file names and is case insensitive.
        /// </summary>
        /// <param name="s">Folder name</param>
        public void MatchWithFolder(string s)
        {
            Debug.Assert(Directory.Exists(s));

            // for folders with hundreds of files, a quadratic algorithm could already be problematic,
            // so make it roughly O(n) by generating a hashmap to quickly lookup textures first.
            var lookup = new Dictionary<string, TextureThumbnailControl>();
            foreach(var entry in Entries)
            {
                lookup[(Path.GetFileName(entry.FilePath) ?? "").ToLower()] = entry;
            }

            foreach(var file in Directory.GetFiles(s))
            {
                var key = (Path.GetFileName(file) ?? "").ToLower();
                if (!lookup.ContainsKey(key))
                {
                    continue;
                }
                var match = lookup[key];
                if(match.CanChangeTextureSource())
                {
                    match.ChangeTextureSource(file);
                }
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 