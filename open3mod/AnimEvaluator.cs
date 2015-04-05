///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [AnimEvaluator.cs]
// (c) 2012-2015, Open3Mod Contributors
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

using System.Linq;
using Assimp;
using OpenTK;
using Quaternion = Assimp.Quaternion;

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
        private readonly double _ticksPerSecond;
        private readonly Matrix4[] _currentTransforms;
        private readonly T3[] _lastPositions;
        private double _lastTime;


        private struct T3
        {
            public int Item1;
            public int Item2;
            public int Item3;
        }


        public AnimEvaluator(Animation animation, double ticksPerSecond)
        {
            _animation = animation;
            _ticksPerSecond = ticksPerSecond;

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


        /// <summary>
        /// Evaluate animation channels at a given time
        /// </summary>
        /// <param name="pTime">Time, in seconds. If the time exceeds the animation's
        /// duration value, it will be looped.</param>
        /// <param name="isInEndPosition">A problem with automatic looping (and the
        /// way how assimp handles animation duration) is that evaluating the
        /// animation at a time that is exactly the animation duration might wrap
        /// over to the first key frame (i.e. due to numerical inaccuracies). Setting
        /// this parameter to true causes the pTime value to be ignored and the
        /// last set of key frames to be taken.</param>
        public void Evaluate(double pTime, bool isInEndPosition)
        {
            // extract ticks per second. Assume default value if not given
            // every following time calculation happens in ticks
            pTime *= _ticksPerSecond;

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

                var presentPosition = new Vector3D(0, 0, 0);
                var presentRotation = new Quaternion(1, 0, 0, 0);
                var presentScaling = new Vector3D(1, 1, 1);

                if (isInEndPosition)
                {
                    if (channel.PositionKeyCount > 0)
                    {
                        presentPosition = channel.PositionKeys.Last().Value;
                        _lastPositions[a].Item1 = channel.PositionKeyCount - 1;
                    }
                    if (channel.RotationKeyCount > 0)
                    {
                        presentRotation = channel.RotationKeys.Last().Value;
                        _lastPositions[a].Item2 = channel.RotationKeyCount - 1;
                    }
                    if (channel.ScalingKeyCount > 0)
                    {
                        presentScaling = channel.ScalingKeys.Last().Value;
                        _lastPositions[a].Item3 = channel.ScalingKeyCount - 1;
                    }
                    BuildTransform(ref presentRotation, ref presentScaling, ref presentPosition, out _currentTransforms[a]);
                    continue;
                }

                // ******** Position *****
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
                        presentRotation = Quaternion.Slerp(key.Value, nextKey.Value, factor);
                    }
                    else
                    {
                        presentRotation = key.Value;
                    }

                    _lastPositions[a].Item2 = frame;
                }

                // ******** Scaling **********                
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

                BuildTransform(ref presentRotation, ref presentScaling, ref presentPosition, out _currentTransforms[a]);
            }

            _lastTime = time;
        }


        /// <summary>
        /// Build a transformation matrix from rotation, scaling and translation components.
        /// The transformation order is scaling, rotation, translation (left to right).
        /// </summary>
        /// <param name="presentRotation"></param>
        /// <param name="presentScaling"></param>
        /// <param name="presentPosition"></param>
        /// <param name="outMatrix"></param>
        private static void BuildTransform(ref Quaternion presentRotation, ref Vector3D presentScaling, 
            ref Vector3D presentPosition, out Matrix4 outMatrix)
        {
            // build a transformation matrix from it
            var mat = new Matrix4x4(presentRotation.GetMatrix());
            mat.A1 *= presentScaling.X;
            mat.B1 *= presentScaling.X;
            mat.C1 *= presentScaling.X;
            mat.A2 *= presentScaling.Y;
            mat.B2 *= presentScaling.Y;
            mat.C2 *= presentScaling.Y;
            mat.A3 *= presentScaling.Z;
            mat.B3 *= presentScaling.Z;
            mat.C3 *= presentScaling.Z;
            mat.A4 = presentPosition.X;
            mat.B4 = presentPosition.Y;
            mat.C4 = presentPosition.Z;

            outMatrix = AssimpToOpenTk.FromMatrix(ref mat);
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 