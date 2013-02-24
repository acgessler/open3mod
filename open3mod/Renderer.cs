///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [Renderer.cs]
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
            GL.ClearColor(Color.LightGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var ui = Window.UiState.ActiveTab;

            var index = Tab.ViewIndex.Index0;
            foreach (var view in ui.ActiveViews)
            {
                // draw the active viewport last (to make sure its contour line is on top)
                if (view == null || ui.ActiveViewIndex == index)
                {
                    ++index;
                    continue;
                }

                var cam = ui.ActiveCameraControllerForView(index);
                DrawViewport(cam, activeScene, view.Value.X, view.Value.Y, view.Value.Z, view.Value.W, false);
                ++index;
            }

            var activeVp = ui.ActiveViews[(int)ui.ActiveViewIndex];
            Debug.Assert(activeVp != null);
            DrawViewport(ui.ActiveCameraController, activeScene, activeVp.Value.X, activeVp.Value.Y, 
                activeVp.Value.Z, activeVp.Value.W, true);

            if (ui.ActiveViewMode != Tab.ViewMode.Single)
            {
                SetFullViewport();
            }

            if (Window.UiState.ShowFps)
            {
                DrawFps();
            }

            _textOverlay.Draw();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public virtual void Dispose(bool disposing)
        {
            _textOverlay.Dispose(disposing);
        }


        /// <summary>
        /// Respond to window changes. Users normally do not need to call this.
        /// </summary>
        public void Resize()
        {
            _textOverlay.Resize();
        }


        /// <summary>
        /// Draw a scene to a viewport using an ICameraController to specify the camera.
        /// </summary>
        /// <param name="view">Active cam controller for this viewport</param>
        /// <param name="activeScene">Scene to be drawn</param>
        /// <param name="xs">X-axis starting point of the viewport in range [0,1]</param>
        /// <param name="ys">Y-axis starting point of the viewport in range [0,1]</param>
        /// <param name="xe">X-axis end point of the viewport in range [0,1]</param>
        /// <param name="ye">X-axis end point of the viewport in range [0,1]</param>
        /// <param name="active"></param>
        private void DrawViewport(ICameraController view, Scene activeScene, double xs, double ys, double xe, 
            double ye, bool active = false)
        {
            // update viewport 
            var w = (double)RenderResolution.Width;
            var h = (double)RenderResolution.Height;

            var vw = (int) ((xe-xs)*w);
            var vh = (int) ((ye-ys)*h);
            GL.Viewport((int)(xs * w), (int)(ys * h), (int)((xe - xs) * w), (int)((ye - ys) * h));

            DrawViewportColorsPre(active);
            var aspectRatio = (float) ((xe - xs) / (ye - ys));

            // set a proper perspective matrix for rendering
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.5f, 10000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);          

            if (activeScene == null)
            {
                DrawNoSceneSplash();
            }
            else
            {
                DrawScene(activeScene, view);
            }

            DrawViewportColorsPost(active, vw, vh);
        }


        private void SetFullViewport()
        {
            GL.Viewport(0, 0, RenderResolution.Width, RenderResolution.Height);
        }


        private void DrawViewportColorsPre(bool active)
        {
            if (!active)
            {
                return;
            }
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            // paint the active viewport in a slightly different shade of gray,
            // overwriting the initial background color.
            GL.Color4(Color.DarkGray);
            GL.Rect(-1, -1, 1, 1);
        }


        private void DrawViewportColorsPost(bool active, int width, int height)
        {
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            var texW = 1.0/width;
            var texH = 1.0/height;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            var lineWidth = active ? 4 : 3;

            // draw contour line
            GL.LineWidth(lineWidth);
            GL.Color4(active ? Color.GreenYellow : Color.DarkGray);

            var xofs = lineWidth * 0.5 * texW;
            var yofs = lineWidth * 0.5 * texH;

            GL.Begin(BeginMode.LineStrip);
            GL.Vertex2(-1.0 + xofs, -1.0 + yofs);
            GL.Vertex2(1.0 - xofs, -1.0 + yofs);
            GL.Vertex2(1.0 - xofs, 1.0 - yofs);
            GL.Vertex2(-1.0 + xofs, 1.0 - yofs);
            GL.Vertex2(-1.0 + xofs, -1.0 + yofs);
            GL.End();

            GL.LineWidth(1);
            GL.MatrixMode(MatrixMode.Modelview);
        }


        private void DrawScene(Scene scene, ICameraController view)
        {
            Debug.Assert(scene != null);
            scene.Render(Window.UiState, view);
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
            graphics.DrawString("FPS: " + Window.Fps.LastFps.ToString("0.0"), Window.UiState.DefaultFont12,
                                new SolidBrush(Color.Red), 5, 5);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 