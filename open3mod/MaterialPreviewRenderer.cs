using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK.Graphics.OpenGL;
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


        /// <summary>
        /// Constructs a MaterialPreviewRenderer to obtain a preview image
        /// for one given material.
        /// </summary>
        /// <param name="window">Window instance that hosts the primary Gl context</param>
        /// <param name="material">Material to render a preview image for</param>
        /// <param name="width">Requested width of the preview image, in pixels</param>
        /// <param name="height">Requested height of the preview image, in pixels</param>
        public MaterialPreviewRenderer(MainWindow window, Material material, uint width, uint height)
        {
            Debug.Assert(material != null);
            _state = CompletionState.Pending;

            window.Renderer.GlExtraDrawJob += () =>
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
            const int fboWidth = 512;
            const int fboHeight = 512;

            uint fboHandle;
            uint colorTexture;
            uint depthRenderbuffer;

            // Create Color Texture
            GL.GenTextures(1, out colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
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
                return false;
            }

            // since there's only 1 Color buffer attached this is not explicitly required
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);

            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
            GL.Viewport(0, 0, fboWidth, fboHeight);

            // restores GL.Viewport() parameters
            GL.PopAttrib(); 
            // return to visible framebuffer
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); 
            GL.DrawBuffer(DrawBufferMode.Back);

            return true;
        }


        private void OnPreviewAvailable()
        {
            OnPreviewAvailableDelegate handler = PreviewAvailable;
            if (handler != null) handler(this);
        }
    }
}
