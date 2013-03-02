///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextOverlay.cs]
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
using System.Diagnostics;
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
        private int _textTexture;

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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _textBmp.Width, _textBmp.Height, 0,
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


        ~TextOverlay()
        {
            // bad, OpenTK is not safe to use from within finalizers.
            // Dispose() should be called manually.
            Debug.Assert(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual void Dispose(bool disposing)
        {
            if (_textTexture > 0)
            {
                GL.DeleteTexture(_textTexture);
                _textTexture = 0;
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
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, _textTexture);

            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 0f);
            GL.TexCoord2(1f, 0f); GL.Vertex2(cs.Width, 0f);
            GL.TexCoord2(1f, 1f); GL.Vertex2(cs.Width, cs.Height);
            GL.TexCoord2(0f, 1f); GL.Vertex2(0f, cs.Height);
            GL.End();         

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

/* vi: set shiftwidth=4 tabstop=4: */ 