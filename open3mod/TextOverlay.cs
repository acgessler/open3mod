using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace open3mod
{
    /// <summary>
    /// 
    /// Utility class to maintain a full-size overlay for the GLControl hosting the
    /// scene. The overlay can be used to efficiently draw text messages on top
    /// of everything.
    /// 
    /// The concept is easy, anyone who wants to draw a text can obtain a .net
    /// drawing context and draw whatever they like. Text drawers need to take
    /// care not to interfere with each other's regions since they will easily
    /// overwrite other text.
    /// 
    /// Based on http://www.opentk.com/doc/graphics/how-to-render-text-using-opengl
    /// </summary>
    public class TextOverlay : IDisposable
    {
        private readonly Renderer _renderer;
        private Bitmap _textBmp;
        private readonly int _textTexture;

        private Graphics _tempContext;


        public bool WantRedraw
        {
            get { return _tempContext != null; }
        }


        public TextOverlay(Renderer renderer)
        {
            _renderer = renderer;

            // Create Bitmap and OpenGL texture
            var cs = renderer.RenderResolution;
            _textBmp = new Bitmap(cs.Width, cs.Height); // match window size

            _textTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            // just allocate memory, so we can update efficiently using TexSubImage2D
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _textBmp.Width, _textBmp.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); 
        }

        /// <summary>
        /// Invoked by the Renderer class when the size of the Gl control changes.
        /// This will automatically clear all text.
        /// </summary>
        public void Resize()
        {
            var cs = _renderer.RenderResolution;

            _textBmp.Dispose();
            _textBmp = new Bitmap(cs.Width, cs.Height);

            GL.BindTexture(TextureTarget.Texture2D, _textTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, 0, 0, _textBmp.Width, _textBmp.Height,
                PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);

            GetDrawableGraphicsContext();
        }

        /// <summary>
        /// Obtain a drawable context so the caller can draw text and mark the text
        /// content as dirty to enforce automatic updating of the underlying Gl
        /// resources.
        /// </summary>
        /// <returns>Context to draw to</returns>
        public Graphics GetDrawableGraphicsContext()
        {
            if(_tempContext == null)
            {
                _tempContext = Graphics.FromImage(_textBmp);

                _tempContext.Clear(Color.Transparent);
                _tempContext.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }

            return _tempContext;
        }


        public void Dispose()
        {
            if (_textTexture > 0)
            {
                GL.DeleteTexture(_textTexture);
            }
        }


        public void Draw()
        {
            // Update the GL texture if needed. Make sure the .net Graphics context is 
            // not hold longer than absolutely necessary
            if(_tempContext != null)
            {
                Commit();

                _tempContext.Dispose();
                _tempContext = null;
            }
     

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Draw a full screen quad with the text texture on top
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            var cs = _renderer.RenderResolution;
            GL.Ortho(0, cs.Width, cs.Height, 0, -1, 1);
          
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(cs.Width, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(cs.Width, cs.Height);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, cs.Height);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, _textTexture);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);

            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }


        /// <summary>
        /// Commit all changes to OpenGl
        /// </summary>
        private void Commit()
        {
            BitmapData data = _textBmp.LockBits(new Rectangle(0, 0, _textBmp.Width, _textBmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, _textTexture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _textBmp.Width, _textBmp.Height, 
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            _textBmp.UnlockBits(data);
        }        
    }
}
