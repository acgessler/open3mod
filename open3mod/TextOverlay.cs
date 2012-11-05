using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Drawing;
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
    public class TextOverlay
    {
        private readonly Renderer _renderer;
        private Bitmap text_bmp;
        private int text_texture;

        private Graphics _tempContext;

        public TextOverlay(Renderer renderer)
        {
            _renderer = renderer;

            // Create Bitmap and OpenGL texture
            var cs = renderer.Window.ClientSize;
            text_bmp = new Bitmap(cs.Width, cs.Height); // match window size

            text_texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, text_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            // just allocate memory, so we can update efficiently using TexSubImage2D
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, text_bmp.Width, text_bmp.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); 
        }

        /// <summary>
        /// Invoked by the Renderer class when the size of the gl control changes.
        /// This will automatically clear all text.
        /// </summary>
        public void Resize()
        {
            
        }

        /// <summary>
        /// Obtain a drawable context so the caller can draw text and mark the text
        /// content as dirty to enforce automatic updating of the underlying gl
        /// resources.
        /// </summary>
        /// <returns>Context to draw to</returns>
        public Graphics GetDrawableGraphicsContext()
        {
            return _tempContext ?? (_tempContext = Graphics.FromImage(text_bmp));
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

            // Draw a full screen quad with the text texture on top
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            var cs = _renderer.Window.ClientSize;
            GL.Ortho(0, cs.Width, cs.Height, 0, -1, 1);

            GL.BindTexture(TextureTarget.Texture2D, text_texture);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(1f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 1f);
            GL.End();
        }


        /// <summary>
        /// Commit all changes to OpenGl
        /// </summary>
        private void Commit()
        {
            BitmapData data = text_bmp.LockBits(new Rectangle(0, 0, text_bmp.Width, text_bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, text_bmp.Width, text_bmp.Height, 0,
                PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            text_bmp.UnlockBits(data);
        }
    }
}
