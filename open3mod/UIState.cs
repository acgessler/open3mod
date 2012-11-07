using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Utility to store global UI settings such as rendering options.
    /// A single instance of UiState is typically passed around to
    /// anybody who needs access to it.
    /// </summary>
    public class UiState
    {
        public enum CameraMode
        {
            Fps,
            Orbit
        }

        public bool RenderWireframe;
        public bool RenderTextured = true;
        public bool RenderLit = true;

        public bool ShowFps = true;

        public CameraMode CamMode = CameraMode.Orbit;

        public Scene ActiveScene = new Scene("../../../testdata/scenes/COLLADA.dae");


        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 12px)
        /// </summary>
        public readonly Font DefaultFont12;


        /// <summary>
        /// Font to be used for textual overlays in 3D view (size ~ 16px)
        /// </summary>
        public readonly Font DefaultFont16;




        public UiState()
        {
            DefaultFont12 = new Font(FontFamily.GenericSansSerif, 12);
            DefaultFont16 = new Font(FontFamily.GenericSansSerif, 16);
        }
    }
}
