///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Texture.cs]
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace open3mod
{
    /// <summary>
    /// Represents a texture. The class maintains both OpenGL textures
    /// and WinForms/GDI images to serve both the renderer and the GUI.
    /// 
    /// Textures are typically created from a given source path, but the
    /// actual loading is done asynchronously. Existence of a "Texture"
    /// object does not guarantee a valid "Image" or OpenGl texture
    /// object.
    /// </summary>
    public sealed class Texture : IDisposable
    {
        private readonly string _file;
        private readonly TextureQueue.CompletionCallback _callback;
        private Image _image;
        private int _gl;

        private readonly object _lock = new object();
        private readonly string _baseDir;
        private Assimp.Texture _dataSource;

        /// <summary>
        /// Possible states of a Texture object during its lifetime
        /// </summary>
        public enum TextureState
        {
            LoadingPending,
            LoadingFailed,
            WinFormsImageCreated,
            GlTextureCreated,
        }

        public delegate void CompletionCallback(Texture self);

        public TextureState State { get; private set; }
        public string FileName
        {
            get { return _file; }
        }

        /// <summary>
        /// Start loading a texture from a given file path
        /// </summary>
        /// <param name="file">File to load from</param>
        /// <param name="baseDir">Scene root folder </param>
        /// <param name="callback">Optional callback to be invoked
        ///   when loading to memory is either complete or is definitely
        ///   failed.)</param>
        public Texture(string file, string baseDir, CompletionCallback callback)
        {
            _file = file;
            _baseDir = baseDir;
            _callback = (s, image, status) => callback(this);
            LoadAsync();
        }


        /// <summary>
        /// Start loading a texture from a given embedded texture
        /// </summary>
        /// <param name="dataSource">Source texture from assimp</param>
        /// <param name="refName">Sentinel name of the texture. This is the name
        ///    that is set as FileName.</param>
        /// <param name="callback">Optional callback to be invoked
        ///   when loading to memory is either complete or is definitely
        ///   failed.)</param>
        public Texture(Assimp.Texture dataSource, string refName, CompletionCallback callback)
        {
            _file = refName;
            _dataSource = dataSource;
            _callback = (s, image, status) => callback(this);
            LoadAsync();
        }



        /// <summary>
        /// Get the Image associated with the texture or null if the
        /// image has not yet been loaded or if an error occurred
        /// during loading.
        /// </summary>
        public Image Image { get { return _image; }}


        /// <summary>
        /// Get the Gl texture object associated with the Texture or 0 if the texture has not 
        /// been created yet. Use Upload() to create the Gl texture.
        /// </summary>
        public int GlTexture
        {
            get {       
                return _gl; 
            }
        }


        private void LoadAsync()
        {
            TextureQueue.CompletionCallback callback = (file, image, result) =>
                {
                    Debug.Assert(_file == file);
                    SetImage(image, result);

                    if (_callback != null)
                    {
                        _callback(_file, _image, result);
                    }
                };

            lock (_lock) {
                State = TextureState.LoadingPending;
                if (_dataSource != null)
                {
                    TextureQueue.Enqueue(_dataSource, _file, callback);
                    return;
                }
                
                TextureQueue.Enqueue(_file, _baseDir, callback);
            }
        }


        private void SetImage(Image image, TextureLoader.LoadResult result)
        {
            Debug.Assert(State == TextureState.LoadingPending);

            lock (_lock)
            {
                _image = image;
                // set proper state
                State = result != TextureLoader.LoadResult.Good  ? TextureState.LoadingFailed : TextureState.WinFormsImageCreated;
                Debug.Assert(result != TextureLoader.LoadResult.Good || _image != null);
            }
        }


        /// <summary>
        /// Upload the texture to video RAM.
        /// 
        /// This requires that State == TextureState.WinFormsImageCreated, i.e. the
        /// RAM texture image must be ready for use.
        /// </summary>
        public void Upload()
        {
            Debug.Assert(State == TextureState.WinFormsImageCreated);
            lock (_lock) { // this is a long CS, but at this time we don't expect concurrent action.
                // http://www.opentk.com/node/259
                using(var textureBitmap = new Bitmap(_image))
                {
                    System.Drawing.Imaging.BitmapData textureData =
                        textureBitmap.LockBits(
                            new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format24bppRgb
                            );
                    int tex;
                    GL.GenTextures(1, out tex);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, tex);
                    
                    GL.TexParameter(TextureTarget.Texture2D,
                                    TextureParameterName.TextureMinFilter,
                                    (int)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D,
                                    TextureParameterName.TextureMagFilter,
                                    (int)TextureMagFilter.Linear);

                    // generate MIPs
                    GL.TexParameter(TextureTarget.Texture2D, 
                        TextureParameterName.GenerateMipmap, 1);

                    // set maximum anisotropic filtering
                    float maxAniso;
                    GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
                    GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);

                    // upload
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four,
                                  textureBitmap.Width,
                                  textureBitmap.Height,
                                  0,
                                  PixelFormat.Bgr,
                                  PixelType.UnsignedByte,
                                  textureData.Scan0);

                    textureBitmap.UnlockBits(textureData);

                    // set final state only if the Gl texture object has been filled successfully
                    // TODO handle glError
                    _gl = tex;
                    State = TextureState.GlTextureCreated;
                }                
            }
        }


#if DEBUG
        ~Texture()
        {
            // OpenTk is unsafe from here, explicit Dispose() is required.
            Debug.Assert(false);
        }
#endif

        public void Dispose()
        {
            if (_gl != 0)
            {
                GL.DeleteTexture(_gl);
                _gl = 0;
            }
            GC.SuppressFinalize(this);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 