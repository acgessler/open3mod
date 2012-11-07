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
        /// once per frame and whose implementation reides in Renderer.
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
            if(activeScene == null)
            {
                DrawNoSceneSplash();
            }
            else
            {
                DrawScene(activeScene);
            }

          
            GL.ClearColor(Color.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            int w = Window.Width;
            int h = Window.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(Color.Yellow);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(10, 20);
            GL.Vertex2(100, 20);
            GL.Vertex2(100, 50);
            GL.End();

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


        private void DrawScene(Scene scene)
        {
            Debug.Assert(scene != null);
            
        }

        private void DrawNoSceneSplash()
        {
            var graphics = _textOverlay.GetDrawableGraphicsContext();

            graphics.DrawString("Drag file here", Window.UiState.DefaultFont12, new SolidBrush(Color.Black), 199, 199);
            graphics.DrawString("Drag file here", Window.UiState.DefaultFont16, new SolidBrush(Color.Red), 200, 200);
        }
    }
}
