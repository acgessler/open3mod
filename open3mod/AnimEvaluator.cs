using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;

namespace open3mod
{
    /// <summary>
    /// Utility class to actually evaluate node animations at a given time. 
    /// An AnimEvaluator is bound to a specific Animation track.
    /// 
    /// The class implements the standard interpolation modes as recommended 
    /// by the assimp documentation, that is lerp() for scaling and translation 
    /// and slerp() for rotations.
    /// </summary>
    public class AnimEvaluator
    {
        private readonly Animation _animation;
        private Matrix4[] _currentTransforms;

        public AnimEvaluator(Animation animation)
        {
            _animation = animation;
        }

        public Animation Animation
        {
            get { return _animation; }
        }

        public Matrix4[] CurrentTransforms
        {
            get { return _currentTransforms; }
        }


        public void Update(double animCursor)
        {
            
        }
    }
}
