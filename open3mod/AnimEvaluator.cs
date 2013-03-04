using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using OpenTK;
using Quaternion = OpenTK.Quaternion;

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
        private T3[] _lastPositions;
        private double _lastTime;


        private struct T3
        {
            public int Item1;
            public int Item2;
            public int Item3;
        }


        public AnimEvaluator(Animation animation)
        {
            _animation = animation;

            _lastPositions = new T3[_animation.NodeAnimationChannelCount];
            _currentTransforms = new Matrix4[_animation.NodeAnimationChannelCount];
        }


        public Animation Animation
        {
            get { return _animation; }
        }


        /// <summary>
        /// Get per-channel evaluated transformation matrices for the current
        /// time. 
        /// </summary>
        public Matrix4[] CurrentTransforms
        {
            get { return _currentTransforms; }
        }


        public void Evaluate(double pTime)
        {
            // extract ticks per second. Assume default value if not given
            double ticksPerSecond = _animation.TicksPerSecond >= 0.0 ? _animation.TicksPerSecond : 25.0;
            // every following time calculation happens in ticks
            pTime *= ticksPerSecond;

            // map into anim's duration
            double time = 0.0f;
            if (_animation.DurationInTicks > 0.0)
            {
                time = pTime % _animation.DurationInTicks;
            }

            // calculate the transformations for each animation channel
            for (int a = 0; a < _animation.NodeAnimationChannelCount; a++)
            {
                var channel = _animation.NodeAnimationChannels[a];

                // ******** Position *****
                var presentPosition = new Vector3D(0, 0, 0);
                if (channel.PositionKeyCount > 0)
                {
                    // Look for present frame number. Search from last position if time is after the last time, else from beginning
                    // Should be much quicker than always looking from start for the average use case.
                    var frame = (time >= _lastTime) ? _lastPositions[a].Item1 : 0;
                    while (frame < channel.PositionKeyCount - 1)
                    {
                        if (time < channel.PositionKeys[frame + 1].Time)
                        {
                            break;
                        }
                        frame++;
                    }

                    // interpolate between this frame's value and next frame's value
                    var nextFrame = (frame + 1) % channel.PositionKeyCount;
                    var key = channel.PositionKeys[frame];
                    var nextKey = channel.PositionKeys[nextFrame];
                    var diffTime = nextKey.Time - key.Time;
                    if (diffTime < 0.0)
                    {
                        diffTime += _animation.DurationInTicks;
                    }

                    if (diffTime > 0)
                    {
                        var factor = (float)((time - key.Time) / diffTime);
                        presentPosition = key.Value + (nextKey.Value - key.Value) * factor;
                    }
                    else
                    {
                        presentPosition = key.Value;
                    }

                    _lastPositions[a].Item1 = frame;
                }

                // ******** Rotation *********
                var presentRotation = new Assimp.Quaternion(1, 0, 0, 0);
                if (channel.RotationKeyCount > 0)
                {
                    var frame = (time >= _lastTime) ? _lastPositions[a].Item2 : 0;
                    while (frame < channel.RotationKeyCount - 1)
                    {
                        if (time < channel.RotationKeys[frame + 1].Time)
                        {
                            break;
                        }
                        frame++;
                    }

                    // interpolate between this frame's value and next frame's value
                    var nextFrame = (frame + 1) % channel.RotationKeyCount;
                    var key = channel.RotationKeys[frame];
                    var nextKey = channel.RotationKeys[nextFrame];
                    double diffTime = nextKey.Time - key.Time;
                    if (diffTime < 0.0)
                    {
                        diffTime += _animation.DurationInTicks;
                    }
                    if (diffTime > 0)
                    {
                        var factor = (float)((time - key.Time) / diffTime);
                        presentRotation = Assimp.Quaternion.Slerp(key.Value, nextKey.Value, factor);
                    }
                    else
                    {
                        presentRotation = key.Value;
                    }

                    _lastPositions[a].Item2 = frame;
                }

                // ******** Scaling **********
                var presentScaling = new Vector3D(1, 1, 1);
                if (channel.ScalingKeyCount > 0)
                {
                    var frame = (time >= _lastTime) ? _lastPositions[a].Item3 : 0;
                    while (frame < channel.ScalingKeyCount - 1)
                    {
                        if (time < channel.ScalingKeys[frame + 1].Time)
                        {
                            break;
                        }
                        frame++;
                    }

                    // TODO: (thom) interpolation maybe? This time maybe even logarithmic, not linear
                    presentScaling = channel.ScalingKeys[frame].Value;
                    _lastPositions[a].Item3 = frame;
                }

                // build a transformation matrix from it
                var mat = new Matrix4x4(presentRotation.GetMatrix());
                mat.A1 *= presentScaling.X; mat.B1 *= presentScaling.X; mat.C1 *= presentScaling.X;
                mat.A2 *= presentScaling.Y; mat.B2 *= presentScaling.Y; mat.C2 *= presentScaling.Y;
                mat.A3 *= presentScaling.Z; mat.B3 *= presentScaling.Z; mat.C3 *= presentScaling.Z;
                mat.A4 = presentPosition.X; mat.B4 = presentPosition.Y; mat.C4 = presentPosition.Z;

                _currentTransforms[a] = AssimpToOpenTk.FromMatrix(ref mat);
            }

            _lastTime = time;
        }
    }
}
