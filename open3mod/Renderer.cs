using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace open3mod
{
    public class Renderer
    {
        private readonly MainWindow _window;
        private TextOverlay _textOverlay;


        public MainWindow Window { get { return _window; } }
        public TextOverlay TextOverlay { get { return _textOverlay; } }


        public Renderer(MainWindow window)
        {
            _window = window;
            _textOverlay = new TextOverlay(this);
        }


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

            _textOverlay.Draw();

            GL.ClearColor(Color.DarkGray);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }


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
            graphics.DrawString("Drag scene here",new Font(FontFamily.GenericSansSerif,16), new SolidBrush(Color.Black), 200, 200);
        }
    }
}
