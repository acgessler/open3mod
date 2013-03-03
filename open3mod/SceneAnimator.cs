using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Encapsulates the animation state of a Scene. Every Scene has its own
    /// SceneAnimator, which is accessible via the Scene.SceneAnimator property
    /// </summary>
    public class SceneAnimator
    {
        private Scene _scene;
        private Assimp.Scene _raw;
        private int _activeAnim = -1;
        private double _animPlaybackSpeed;
        private double _animCursor;


        internal SceneAnimator(Scene scene)
        {
            _scene = scene;
            _raw = scene.Raw;

            Debug.Assert(_raw != null, "scene must already be loaded");
        }


        /// Zero-based index of the currently active animation or -1 to disable
        /// animations altogether (in which case the initial/non-animated pose
        /// is shown).
        public int ActiveAnimation
        {
            get { return _activeAnim; }

            set
            {
                Debug.Assert(value >= -1 && value < _raw.AnimationCount);
                _activeAnim = value;
            }
        }


        /// <summary>
        /// Animation playback speed factor. A factor of 1 means that the current
        /// animation is played in its original speed.
        /// </summary>
        public double AnimationPlaybackSpeed
        {
            get { return _animPlaybackSpeed; }

            set
            {
                Debug.Assert(value >= 0);
                _animPlaybackSpeed = value;
            }
        }


        /// <summary>
        /// Current animation playback position (cursor), in seconds. Can be 
        /// assigned to, but the value needs to be within [0,AnimationDuration].
        /// </summary>
        public double AnimationCursor
        {
            get { return _animCursor; }

            set
            {
                Debug.Assert(value >= 0 && value <= AnimationDuration);
                _animCursor = value;
            }
        }


        /// <summary>
        /// Getter for the duration of the current animation in seconds.
        /// Returns 0.0 if no animation is currently active.
        /// </summary>
        public double AnimationDuration
        {
            get
            {
                if (ActiveAnimation == -1)
                {
                    return 0.0;
                }
                var anim = _raw.Animations[ActiveAnimation];
                return anim.DurationInTicks / anim.TicksPerSecond;
            }
        }
    }
}
