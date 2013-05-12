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

#define DETECT_ALPHA_EARLY

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;


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
        private readonly Assimp.EmbeddedTexture _dataSource;
 

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

        /// <summary>
        /// Possibles values for the HasAlpha property
        /// </summary>
        public enum AlphaState
        {
            NotKnownYet,
            Opaque,
            HasAlpha,
        }

        public delegate void CompletionCallback(Texture self);



        private volatile AlphaState _alphaState = AlphaState.NotKnownYet;
        private volatile TextureState _state;
        private volatile bool _reconfigure;


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

            if (CoreSettings.CoreSettings.Default.LoadTextures)
            {
                LoadAsync();
            }
            else
            {
                SetImage(null, TextureLoader.LoadResult.FileNotFound);
            }
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
        public Texture(Assimp.EmbeddedTexture dataSource, string refName, CompletionCallback callback)
        {
            _file = refName;
            _dataSource = dataSource;
            _callback = (s, image, status) => callback(this);

            if (CoreSettings.CoreSettings.Default.LoadTextures)
            {
                LoadAsync();
            }
            else
            {
                SetImage(null, TextureLoader.LoadResult.FileNotFound);
            }
        }



        /// <summary>
        /// Get the Image associated with the texture or null if the
        /// image has not yet been loaded or if an error occurred
        /// during loading.
        /// </summary>
        public Image Image { get
        {
            Debug.Assert(_image == null || State == TextureState.WinFormsImageCreated || State == TextureState.GlTextureCreated);
            return _image;
        }}


        /// <summary>
        /// Returns whether the texture has any non-opaque pixels and thus
        /// needs to be rendered with alpha-blending.
        /// 
        /// This requires that State >= TextureState.GlTextureCreated, i.e. the
        /// Gl texture image must have been Upload()ed
        /// </summary>
        public AlphaState HasAlpha
        {
            get
            {
                if (_alphaState == AlphaState.NotKnownYet)
                {
                    TryDetectAlpha();
                }
                return _alphaState;
            }
        }


        /// <summary>
        /// Current state of the texture object.
        /// </summary>
        public TextureState State
        {
            get { return _state; }
            private set { _state = value; }
        }


        public string FileName
        {
            get { return _file; }
        }


        /// <summary>
        /// Flag that indicates whether ReconfigureUploadedTexture() should be
        /// called on this texture. If BindGlTexture() is called while this
        /// flag is set, it does this automatically. See the docs for BindGlTexture
        /// for more information.
        /// 
        /// This flag can be read and written to from any thread while
        /// ReconfigureUploadedTexture() requires to be called on the Gl thread.
        /// </summary>
        public bool ReconfigureUploadedTextureRequested
        {
            get { return _reconfigure; }
            set { _reconfigure = value; }
        }


        /// <summary>
        /// Upload the texture to video RAM.
        /// 
        /// This requires that State == TextureState.WinFormsImageCreated, i.e. the
        /// RAM texture image must be ready for use and the Gl object may not have 
        /// been created yet.
        /// 
        /// Must be called on a thread that is allowed to use Gl APIs.
        /// </summary>
        public void Upload()
        {
            Debug.Assert(State == TextureState.WinFormsImageCreated);

            // this may be required if ReleaseUpload() has been called before
            if (_gl != 0)
            {
                GL.DeleteTexture(_gl);
                _gl = 0;
            }

            lock (_lock) { // this is a long CS, but at this time we don't expect concurrent action.
                // http://www.opentk.com/node/259
                Bitmap textureBitmap = null;
                var shouldDisposeBitmap = false;
               
                // in order to LockBits(), we need to create a Bitmap. In case the given Image
                // *is* already a Bitmap however, we can directly re-use it.
                try {
                    if (_image is Bitmap)
                    {
                        textureBitmap = (Bitmap)_image;
                    }
                    else
                    {
                        textureBitmap = new Bitmap(_image);
                        shouldDisposeBitmap = true;
                    }

                    GL.GetError();

                    // apply texture resolution bias? (i.e. low quality textures)
                    if(GraphicsSettings.Default.TexQualityBias > 0)
                    {
                        var b = ApplyResolutionBias(textureBitmap, GraphicsSettings.Default.TexQualityBias);
                        if(shouldDisposeBitmap)
                        {
                            textureBitmap.Dispose();
                        }
                        textureBitmap = b;
                        shouldDisposeBitmap = true;
                    }

                    var textureData = textureBitmap.LockBits(
                        new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb
                        );


                    // determine alpha pixels if this has not been done before
                    if (_alphaState == AlphaState.NotKnownYet)
                    {
                        _alphaState = LookForAlphaBits(textureData) ? AlphaState.HasAlpha : AlphaState.Opaque;
                    }

                    int tex;
                    GL.GenTextures(1, out tex);

                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, tex);

                    ConfigureFilters();                    

                    // upload
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four,
                                  textureBitmap.Width,
                                  textureBitmap.Height,
                                  0,
                                  PixelFormat.Bgra,
                                  PixelType.UnsignedByte,
                                  textureData.Scan0);

                    textureBitmap.UnlockBits(textureData);

                    // set final state only if the Gl texture object has been filled successfully
                    if (GL.GetError() == ErrorCode.NoError)
                    {
                        _gl = tex;
                        State = TextureState.GlTextureCreated;
                    }
                }       
                finally {
                    if (shouldDisposeBitmap)
                    {
                        textureBitmap.Dispose();
                    }
                }
            }
        }


        /// <summary>
        /// Binds the Gl texture object for this Texture to the Texture2D stage.
        /// 
        /// This requires that the texture has been uploaded. Do not use
        /// Gl.BindTexture directly. This method calls ReconfigureUploadedTexture
        /// if the ReconfigureUploadedTextureRequested flag is set. It is therefore
        /// only safe to use from within displaylists if this flag is not set (i.e.
        /// ReconfigureUploadedTexture() should be called before the displaylist
        /// is compiled).
        /// 
        /// This method may be called only from a thread that has access to Gl.
        /// </summary>
        public void BindGlTexture()
        {
            Debug.Assert(State == TextureState.GlTextureCreated);
            GL.BindTexture(TextureTarget.Texture2D, _gl);

            if(ReconfigureUploadedTextureRequested)
            {
                ReconfigureUploadedTexture();
            } 
        } 


        private void ConfigureFilters()
        {
            // assuming Gl texture is bound!
            var settings = GraphicsSettings.Default;
            var mips = settings.UseMips;

            switch(settings.TextureFilter)
            {
                    // full aniso
                case 3:             
                    // low aniso
                case 2:           
                    // linear
                case 1:
                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int)(mips ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));

                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Linear);
                    break;

                    // point
                case 0:
                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMinFilter,
                        (int)(mips ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));

                    GL.TexParameter(TextureTarget.Texture2D,
                        TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Nearest);
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }

            // select anisotropic filtering if needed
            if (settings.TextureFilter >= 2)
            {
                float maxAniso;
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 
                    settings.TextureFilter >= 3
                        ? maxAniso
                        : maxAniso * 0.5f);    
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                    0.0f);    
            }
            
            // generate MIPs?
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, mips ? 1 : 0);
            if (!mips)
            {
                return;
            }

            // already uploaded before? need glGenerateMipMap to update
            if(State == TextureState.GlTextureCreated)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }


        /// <summary>
        /// Requests that the Gl object for the texture should be released.
        /// After this method has been called, Upload() can be used to re-create
        /// the object (otherwise calling Upload() again would be illegal). 
        /// 
        /// This method can be called from any thread.
        /// </summary>
        public void ReleaseUpload()
        {
            Debug.Assert(State == TextureState.GlTextureCreated);

            lock (_lock)
            {
                State = TextureState.WinFormsImageCreated;
            }
            // don't Gl.DeleteTexture() at this point - it is not necessarily the
            // Gl thread. Upload() does this.
        }


        /// <summary>
        /// Requests that the texture filtering settings associated with the
        /// Gl object for this texture should be reconfigured based on the
        /// current GraphicsSettings. Reconfiguration happens immediately.
        /// 
        /// Use ReconfigureUploadedTextureRequested to request this method
        /// to be called the next time the texture is bound.
        /// 
        /// This method must therefore be called from a thread that has
        /// access to Gl. The currently bound texture is afterwards restored
        /// to its old value.
        /// </summary>
        public void ReconfigureUploadedTexture()
        {
            Debug.Assert(State == TextureState.GlTextureCreated);

            ReconfigureUploadedTextureRequested = false;

            int old;
            GL.GetInteger(GetPName.TextureBinding2D, out old);
            GL.BindTexture(TextureTarget.Texture2D, _gl);
            ConfigureFilters();

            if(old != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, old);
            }
        }


        /// <summary>
        /// Obtain a downscaled version of a given Bitmap. The downscaling
        /// factor is expressed as an log2 resolution bias.
        /// </summary>
        /// <param name="textureBitmap"></param>
        /// <param name="bias">Value greater 0</param>
        /// <returns></returns>
        private static Bitmap ApplyResolutionBias(Bitmap textureBitmap, int bias)
        {
            Debug.Assert(textureBitmap != null);
            Debug.Assert(bias > 0);

            var width = textureBitmap.Width >> bias;
            var height = textureBitmap.Height >> bias;

            var b = new Bitmap(width, height);
            using (var g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(textureBitmap, 0, 0, width, height);
            }

            return b;
        }


        /// <summary>
        /// Populate the _alphaState field if it is still AlphaState.NotKnownYet
        /// </summary>
        private void TryDetectAlpha()
        {
            Debug.Assert(_alphaState == AlphaState.NotKnownYet);
            if (State != TextureState.WinFormsImageCreated && State != TextureState.GlTextureCreated)
            {
                return;
            }
            lock(_lock)
            {               
                // in order to LockBits(), we need to create a Bitmap. In case the given Image
                // *is* already a Bitmap however, we can directly re-use it.
                try {
                    Bitmap textureBitmap = null;
                    if (_image is Bitmap)
                    {
                        textureBitmap = (Bitmap)_image;
                    }
                    else
                    {
                        // replace the image by the Bitmap - Upload() needs it anyway
                        var old = _image;
                        _image = textureBitmap = new Bitmap(_image);
                        old.Dispose();
                    }

                    var textureData = textureBitmap.LockBits(
                        new Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                            ImageLockMode.ReadOnly,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb
                        );
                
                   _alphaState = LookForAlphaBits(textureData) ? AlphaState.HasAlpha : AlphaState.Opaque;
                   textureBitmap.UnlockBits(textureData);
                }
                catch
                { }
            }
        }


        /// <summary>
        /// Returns whether there are any non-oapque pixels in a given texture slice.
        /// </summary>
        /// <param name="textureData"></param>
        /// <returns></returns>
        private static bool LookForAlphaBits(BitmapData textureData)
        {        
            Debug.Assert(textureData.Stride > 0);

            var countBytes = textureData.Stride * textureData.Height;
            var tempBuffer = new byte[countBytes];
     
            var dataLineLength = textureData.Width * 4;
            var padding = textureData.Stride - dataLineLength;
            Debug.Assert(padding >= 0);

            System.Runtime.InteropServices.Marshal.Copy(textureData.Scan0, tempBuffer, 0, countBytes);

            var n = 3;
            for (var y = 0; y < textureData.Height; ++y, n += padding)
            {
                for (var x = 0; x < textureData.Width; ++x, n += 4)
                {
                    if (tempBuffer[n] < 0xff)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Schedule asynchronous texture loading.
        /// </summary>
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

            lock (_lock)
            {
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
                State = result != TextureLoader.LoadResult.Good ? TextureState.LoadingFailed : TextureState.WinFormsImageCreated;
                Debug.Assert(result != TextureLoader.LoadResult.Good || _image != null);

                
#if DETECT_ALPHA_EARLY
                if (_image != null)
                {
                    TryDetectAlpha();
                }
#endif
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
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 