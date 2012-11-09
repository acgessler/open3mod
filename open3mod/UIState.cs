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
        /// <summary>
        /// Enum of all supported camera modes.
        /// </summary>
        public enum CameraMode
        {
            Fps = 0,
            Orbit,
            _Max
        }

        /// <summary>
        /// Index all 3D views - there can be up to four 3D views at this time,
        /// but the rest of the codebase always works with _Max so it can be
        /// nicely adjusted simply by adding more indexes.
        /// </summary>
        public enum ViewIndex
        {
            Index0 = 0,
            Index1,
            Index2,
            Index3,
            _Max
        }

        /// <summary>
        /// Supported arrangements of 3D views. Right now only the number of
        /// 3d windows.
        /// </summary>
        public enum ViewMode
        {
            Single,
            Two,
            Four
        }

        public bool RenderWireframe;
        public bool RenderTextured = true;
        public bool RenderLit = true;

        public bool ShowFps = true;

        public CameraMode CamMode = CameraMode.Orbit;
        public ViewIndex ActiveViewIndex = 0;

        public ViewMode ActiveViewMode = ViewMode.Single;


        /// <summary>
        /// Camera controllers maintain state even when they are not active, 
        /// therefore we need to keep a camera controller for every
        /// view index X camera mode.
        /// </summary>
        public ICameraController[,] CameraImpls = new ICameraController[(int)CameraMode._Max,(int)ViewIndex._Max];


        /// <summary>
        /// Current active scene
        /// </summary>
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
