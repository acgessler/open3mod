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
    public class Texture
    {
        private readonly string _file;
        private Image _image;
        private int _gl;

        private readonly object _lock = new object();

        /// <summary>
        /// Possible states of a Texture object during its lifetime
        /// </summary>
        public enum LoadState
        {
            LoadingPending,
            LoadingFailed,
            WinFormsImageCreated,
            GlTextureCreated,
        }


        public LoadState State { get; private set; }


        public Texture(string file)
        {
            _file = file;
            LoadAsync();
        }

        /// <summary>
        /// Get the Image associated with the texture or null if the
        /// image has not yet been loaded or if an error occurred
        /// during loading.
        /// </summary>
        public Image Image { get { return _image; }}


        /// <summary>
        /// Get the OpenGl texture object associated with the Texture
        /// or 0 if the texture has not yet been created or if an
        /// error occurred during loading.
        /// </summary>
        public int GlTexture
        {
            get { 
                if (_gl == 0)
                {
                    CreateGlTexture();
                }
                return _gl; 
            }
        }


        private void LoadAsync()
        {
            lock (_lock) {
                State = LoadState.LoadingPending;
                TextureQueue.Enqueue(_file, (file, image) =>
                {
                    Debug.Assert(_file.Equals(file));
                    SetImage(image);
                });
            }
        }

        private void SetImage(Image image)
        {
            Debug.Assert(State == LoadState.LoadingPending);

            lock (_lock)
            {
                _image = image;
                // set proper state
                State = _image == null ? LoadState.LoadingFailed : LoadState.WinFormsImageCreated;
            }
        }

        private void CreateGlTexture()
        {
            Debug.Assert(State == LoadState.WinFormsImageCreated);
            lock (_lock) {
                // http://www.opentk.com/node/259
                var textureBitmap = new Bitmap(_image);

                System.Drawing.Imaging.BitmapData textureData =
                    textureBitmap.LockBits(
                        new System.Drawing.Rectangle(0, 0, textureBitmap.Width, textureBitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb
                        );

                GL.GenTextures(1, out _gl);
                GL.BindTexture(TextureTarget.Texture2D, _gl);

                GL.TexParameter(TextureTarget.Texture2D,
                                TextureParameterName.TextureMinFilter,
                                (int) TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D,
                                TextureParameterName.TextureMagFilter,
                                (int) TextureMagFilter.Linear);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Four,
                              textureBitmap.Width,
                              textureBitmap.Height,
                              0,
                              PixelFormat.Bgr,
                              PixelType.UnsignedByte,
                              textureData.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);

                textureBitmap.UnlockBits(textureData);

                // set final state
                State = LoadState.GlTextureCreated;
            }
        }
    }
}
