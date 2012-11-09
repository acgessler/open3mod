using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace open3mod
{
    public class Renderer : IDisposable
    {
        private readonly MainWindow _window;
        private TextOverlay _textOverlay;

        /// <summary>
        /// The gl context which is being rendered to
        /// </summary>
        public GLControl GlControl { get { return _window.GlControl; }} 

        /// <summary>
        /// Host window
        /// </summary>
        public MainWindow Window { get { return _window; } }


        /// <summary>
        /// Utility object in charge of maintaining all text overlays
        /// </summary>
        public TextOverlay TextOverlay { get { return _textOverlay; } }

        /// <summary>
        /// Obtain actual rendering resolution in pixels
        /// </summary>
        public Size RenderResolution { get { return GlControl.ClientSize; } }


        /// <summary>
        /// Construct a renderer given a valid and fully loaded MainWindow
        /// </summary>
        /// <param name="window">Main window, Load event of the GlContext
        ///    needs to be fired already.</param>
        internal Renderer(MainWindow window)
        {
            _window = window;
            _textOverlay = new TextOverlay(this);
        }


        /// <summary>
        /// Perform any non-drawing operations that need to be executed
        /// once per frame and whose implementation resides in Renderer.
        /// </summary>
        public void Update()
        {
            
        }


        /// <summary>
        /// Draw a given scene or alternatively a default screen.
        /// </summary>
        /// <param name="activeScene">Active scene or a null to show the "drag file here" screen</param>
        public void Draw(Scene activeScene = null)
        {
                   
            GL.ClearColor(Color.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            switch(Window.UiState.ActiveViewMode)
            {
                case UiState.ViewMode.Single:
                    DrawViewport(activeScene,0.0,0.0,1.0,1.0);
                    break;
                case UiState.ViewMode.Two:
                    DrawViewport(activeScene, 0.0, 0.0, 0.5, 1.0);
                    DrawViewport(activeScene, 0.5, 0.0, 1.0, 1.0);
                    break;
                case UiState.ViewMode.Four:
                    DrawViewport(activeScene, 0.0, 0.0, 0.5, 0.5);
                    DrawViewport(activeScene, 0.5, 0.0, 1.0, 0.5);
                    DrawViewport(activeScene, 0.0, 0.5, 0.5, 1.0);
                    DrawViewport(activeScene, 0.5, 0.5, 1.0, 1.0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



            if (Window.UiState.ShowFps)
            {
                DrawFps();
            }

            _textOverlay.Draw();
        }


        public void Dispose()
        {
            _textOverlay.Dispose();
        }


        /// <summary>
        /// Respond to window changes. Users normally do not need to call this.
        /// </summary>
        public void Resize()
        {
            
            _textOverlay.Resize();
        }


        private void DrawViewport(Scene activeScene, double xs, double ys, double xe, double ye)
        {
            var w = (double)RenderResolution.Width;
            var h = (double)RenderResolution.Height;
            GL.Viewport((int)(xs * w), (int)(ys * w), (int)((xe-xs) * w), (int)((ye-ys) * w));

            var aspectRatio = (float) ((xe - xs) / (ye - ys));

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 1000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);


            if (activeScene == null)
            {
                DrawNoSceneSplash();
            }
            else
            {
                DrawScene(activeScene);
            }
        }


        private void DrawScene(Scene scene)
        {
            Debug.Assert(scene != null);
            scene.Render(Window.UiState);
        }


        private void DrawNoSceneSplash()
        {
            var graphics = _textOverlay.GetDrawableGraphicsContext();

            graphics.DrawString("Drag file here", Window.UiState.DefaultFont12, new SolidBrush(Color.Black), 199, 199);
            graphics.DrawString("Drag file here", Window.UiState.DefaultFont16, new SolidBrush(Color.Red), 200, 200);
        }


        private double _accTime;
        private void DrawFps()
        {
            // only update every 1/3rd of a second
            _accTime += Window.Fps.LastFrameDelta;
            if (_accTime < 0.3333 && !_textOverlay.WantRedraw)
            {
                return;
            }

            _accTime = 0.0;

            var graphics = _textOverlay.GetDrawableGraphicsContext();
            graphics.DrawString("FPS: " + Window.Fps.LastFps.ToString("0.0"), Window.UiState.DefaultFont12, new SolidBrush(Color.Red), 5,5);
        }


        
    }
}
