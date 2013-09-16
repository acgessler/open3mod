///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [MaterialPreviewRenderer.cs]
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using TextureWrapMode = Assimp.TextureWrapMode;

namespace open3mod
{
    /// <summary>
    /// Renders off-screen preview images for materials, such as those being
    /// shown in the materials tab. One MaterialPreviewRenderer is responsible
    /// only for rendering one preview image for one material (it is "one use only").
    /// 
    /// Rendering preview images is interleaved with the actual visible scene drawing
    /// and can thus not happen at any time. The MaterialPreviewRenderer offers the
    /// PreviewAvailable event to let users know when the preview image is available.
    /// </summary>
    public class MaterialPreviewRenderer
    {
        private readonly Scene _scene;
        private readonly Material _material;
        private readonly uint _width;
        private readonly uint _height;

        public enum CompletionState
        {
            Pending, Failed, Done
        }


        public delegate void OnPreviewAvailableDelegate(MaterialPreviewRenderer me);

        /// <summary>
        /// This event is fired when the preview image is available or if generating
        /// the image failed for some reason. It is only fired once for a
        /// MaterialPreviewRenderer instance.
        /// </summary>
        public event OnPreviewAvailableDelegate PreviewAvailable;


        private CompletionState _state;
        private Image _previewImage;

        // shared geometry data to draw a sphere
        private static SphereGeometry.Vertex[] _sphereVertices;
        private static ushort[] _sphereElements;


        private const float SphereRadius = 0.9f;
        private const int SphereSegments = 50;


        /// <summary>
        /// Constructs a MaterialPreviewRenderer to obtain a preview image
        /// for one given material.
        /// </summary>
        /// <param name="window">Window instance that hosts the primary Gl context</param>
        /// <param name="scene">Scene instance that the material belongs to</param>
        /// <param name="material">Material to render a preview image for</param>
        /// <param name="width">Requested width of the preview image, in pixels</param>
        /// <param name="height">Requested height of the preview image, in pixels</param>
        public MaterialPreviewRenderer(MainWindow window, Scene scene, Material material, uint width, uint height)
        {
            Debug.Assert(window != null);
            Debug.Assert(material != null);
            Debug.Assert(scene != null);
            Debug.Assert(width >= 1);
            Debug.Assert(height >= 1);

            _scene = scene;
            _material = material;
            _width = width;
            _height = height;
    
            _state = CompletionState.Pending;

            window.Renderer.GlExtraDrawJob += (sender) =>
            {
                _state = !RenderPreview() ? CompletionState.Failed : CompletionState.Done;
                OnPreviewAvailable();
            };
        }


        /// <summary>
        /// Obtains the rendered preview image as a System.Drawing.Image.
        /// The value is non-null iff State == CompletionState.Done
        /// </summary>
        public Image PreviewImage
        {
            get { return _previewImage; }
        }


        /// <summary>
        /// Gives the current completion state of the preview rendering job.
        /// </summary>
        public CompletionState State
        {
            get { return _state; }
        }


        /// <summary>
        /// Renders the preview. This performs Gl commands, so it must be called
        /// from a context where this is allowed.
        /// 
        /// Upon success, true is returned and _previewImage gets assigned
        /// the generated preview image.
        /// </summary>
        /// <returns>true in case the preview has been successfully generated</returns>
        private bool RenderPreview()
        {
            // based on http://www.opentk.com/doc/graphics/frame-buffer-objects
            // use a pow2 FBO even if the hardware supports arbitrary sizes
            var fboWidth = (int)RoundToNextPowerOfTwo(_width);
            var fboHeight = (int)RoundToNextPowerOfTwo(_height);

            int fboHandle = -1;
            int colorTexture = -1;
            int depthRenderbuffer = 01;

            // clear error flags
            GL.GetError();

            // Create Color Texture
            GL.GenTextures(1, out colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            // note! OpenTK.Graphics.OpenGL.TextureWrapMode clashes with Assimp.EmbeddedTextureWrapMode
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp);
            
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 
                fboWidth, 
                fboHeight, 
                0, 
                PixelFormat.Rgba, 
                PixelType.UnsignedByte, 
                IntPtr.Zero);

            // test for GL Error here (might be unsupported format)
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                EnsureTemporaryResourcesReleased(colorTexture, 
                    depthRenderbuffer, 
                    fboHandle);
                return false;
            }

            GL.BindTexture(TextureTarget.Texture2D, 0); // prevent feedback, reading and writing to the same image is a bad idea

            // Create Depth Renderbuffer
            GL.Ext.GenRenderbuffers(1, out depthRenderbuffer);
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthRenderbuffer);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, 
                (RenderbufferStorage)All.DepthComponent32, fboWidth, fboHeight);

            // test for GL Error here (might be unsupported format)
            error = GL.GetError();
            if(error != ErrorCode.NoError)
            {
                EnsureTemporaryResourcesReleased(colorTexture, 
                    depthRenderbuffer, fboHandle);
                return false;
            }

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out fboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, 
                FramebufferAttachment.ColorAttachment0Ext, 
                TextureTarget.Texture2D, 
                colorTexture, 0);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, 
                FramebufferAttachment.DepthAttachmentExt, 
                RenderbufferTarget.RenderbufferExt, 
                depthRenderbuffer);

            // verify the FBO is complete and ready to use
            var status = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                EnsureTemporaryResourcesReleased(colorTexture, 
                    depthRenderbuffer, fboHandle);
                return false;
            }

            // since there's only 1 Color buffer attached this is not explicitly required
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
            GL.Viewport(0, 0, (int)_width, (int)_height);

            Draw();
            CopyToImage();

            // restores GL.Viewport() parameters
            GL.PopAttrib(); 
            // return to visible framebuffer
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); 
            GL.DrawBuffer(DrawBufferMode.Back);

            EnsureTemporaryResourcesReleased(colorTexture, 
                depthRenderbuffer, fboHandle);
            return true;
        }


        private void CopyToImage()
        {
            var bmp = new Bitmap((int)_width, (int)_height);
            var data = bmp.LockBits(new Rectangle(0,0,(int)_width, (int)_height), 
                ImageLockMode.WriteOnly, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.ReadPixels(0,0,(int)_width,(int)_height,PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0 );    
            bmp.UnlockBits(data);

            _previewImage = bmp;
        }


        private static void EnsureTemporaryResourcesReleased(int colorTexture, int depthRenderbuffer, int fboHandle)
        {
            if (colorTexture != -1)
            {
                GL.DeleteTexture(colorTexture);
            }
            if (depthRenderbuffer != -1)
            {
                GL.DeleteRenderbuffers(1, ref depthRenderbuffer);
            } 
            if (fboHandle != -1)
            {
                GL.DeleteFramebuffers(1, ref fboHandle);
            }
        }


        private void Draw()
        {
            GL.ClearColor(Color.Transparent);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (_sphereVertices == null)
            {
                _sphereVertices = SphereGeometry.CalculateVertices(SphereRadius, SphereRadius, SphereSegments, SphereSegments);
            }
            if (_sphereElements == null)
            {
                _sphereElements = SphereGeometry.CalculateElements(SphereSegments, SphereSegments);
            }

            // reset color and alpha blending
            GL.Color4(Color.White);
            GL.Disable(EnableCap.Blend);

            _scene.MaterialMapper.ApplyMaterial(null, _material, true, true);
            // always enable depth writes when rendering material previews
            GL.DepthMask(true);

            GL.MatrixMode(MatrixMode.Modelview);
            var lookat = Matrix4.LookAt(0, 0, -2.5f, 0, 0, 0, 0, 1, 0);
            GL.LoadMatrix(ref lookat);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.0f, 0.01f, 100.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            GL.Enable(EnableCap.DepthTest);

            // set fixed-function lighting parameters
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Light0);
      
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0.5f, -0.6f, -0.8f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 1, 1, 1, 1 });     

            Debug.Assert(_sphereVertices != null);
            Debug.Assert(_sphereElements != null);
            SphereGeometry.Draw(_sphereVertices, _sphereElements);
        }


        private static uint RoundToNextPowerOfTwo(uint s)
        {
            return (uint)Math.Pow(2, Math.Ceiling(Math.Log(s, 2)));
        }


        private void OnPreviewAvailable()
        {
            OnPreviewAvailableDelegate handler = PreviewAvailable;
            if (handler != null) handler(this);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 