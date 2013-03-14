/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Describes the animation of a single node. The name specifies the bone/node which is affected by
    /// this animation chanenl. The keyframes are given in three separate seties of values,
    /// one for each position, rotation, and scaling. The transformation matrix is computed from
    /// these values and replaces the node's original transformation matrix at a specific time.
    /// <para>This means all keys are absolute and not relative to the bone default pose.
    /// The order which the transformations are to be applied is scaling, rotation, and translation (SRT).</para>
    /// <para>Keys are in chronological order and duplicate keys do not pass the validation step. There most likely will be no
    /// negative time values, but they are not forbidden.</para>
    /// </summary>
    public sealed class NodeAnimationChannel {
        private String m_nodeName;
        private VectorKey[] m_positionKeys;
        private QuaternionKey[] m_rotationKeys;
        private VectorKey[] m_scalingKeys;
        private AnimationBehaviour m_preState;
        private AnimationBehaviour m_postState;

        /// <summary>
        /// Gets the name of the node affected by this animation. It must <c>exist</c> and it <c>must</c>
        /// be unique.
        /// </summary>
        public String NodeName {
            get {
                return m_nodeName;
            }
        }

        /// <summary>
        /// Gets the number of position keys in the animation channel.
        /// </summary>
        public int PositionKeyCount {
            get {
                return (m_positionKeys == null) ? 0 : m_positionKeys.Length;
            }
        }

        /// <summary>
        /// Checks if this animation channel contains position keys.
        /// </summary>
        public bool HasPositionKeys {
            get {
                return m_positionKeys != null;
            }
        }

        /// <summary>
        /// Gets the position keys of this animation channel. Positions are
        /// specified as a 3D vector. If there are position keys, there will
        /// also be -at least- one scaling and one rotation key.
        /// </summary>
        public VectorKey[] PositionKeys {
            get {
                return m_positionKeys;
            }
        }

        /// <summary>
        /// Gets the number of rotation keys in the animation channel.
        /// </summary>
        public int RotationKeyCount {
            get {
                return (m_rotationKeys == null) ? 0 : m_rotationKeys.Length;
            }
        }

        /// <summary>
        /// Checks if the animation channel contains rotation keys.
        /// </summary>
        public bool HasRotationKeys {
            get {
                return m_rotationKeys != null;
            }
        }

        /// <summary>
        /// Gets the rotation keys of this animation channel. Rotations are
        /// given as quaternions. If this exists, there will be -at least- one
        /// scaling and one position key.
        /// </summary>
        public QuaternionKey[] RotationKeys {
            get {
                return m_rotationKeys;
            }
        }

        /// <summary>
        /// Gets the number of scaling keys in the animation channel.
        /// </summary>
        public int ScalingKeyCount {
            get {
                return (m_scalingKeys == null) ? 0 : m_scalingKeys.Length;
            }
        }

        /// <summary>
        /// Checks if the animation channel contains scaling keys.
        /// </summary>
        public bool HasScalingKeys {
            get {
                return m_scalingKeys != null;
            }
        }

        /// <summary>
        /// Gets the scaling keys of this animation channel. Scalings are
        /// specified in a 3D vector. If there are scaling keys, there will
        /// also be -at least- one position and one rotation key.
        /// </summary>
        public VectorKey[] ScalingKeys {
            get {
                return m_scalingKeys;
            }
        }

        /// <summary>
        /// Gets how the animation behaves before the first key is encountered. By default the original
        /// transformation matrix of the affected node is used.
        /// </summary>
        public AnimationBehaviour PreState {
            get {
                return m_preState;
            }
        }

        /// <summary>
        /// Gets how the animation behaves after the last key was processed. By default the original
        /// transformation matrix of the affected node is taken.
        /// </summary>
        public AnimationBehaviour PostState {
            get {
                return m_postState;
            }
        }

        /// <summary>
        /// Constructs a new NodeAnimation.
        /// </summary>
        /// <param name="nodeAnim">Unmanaged AiNodeAnim struct</param>
        internal NodeAnimationChannel(AiNodeAnim nodeAnim) {
            m_nodeName = nodeAnim.NodeName.GetString();
            m_preState = nodeAnim.Prestate;
            m_postState = nodeAnim.PostState;

            //Load position keys
            if(nodeAnim.NumPositionKeys > 0 && nodeAnim.PositionKeys != IntPtr.Zero) {
                m_positionKeys = MemoryHelper.MarshalArray<VectorKey>(nodeAnim.PositionKeys, (int) nodeAnim.NumPositionKeys);
            }

            //Load rotation keys
            if(nodeAnim.NumRotationKeys > 0 && nodeAnim.RotationKeys != IntPtr.Zero) {
                m_rotationKeys = MemoryHelper.MarshalArray<QuaternionKey>(nodeAnim.RotationKeys, (int) nodeAnim.NumRotationKeys);
            }

            //Load scaling keys
            if(nodeAnim.NumScalingKeys > 0 && nodeAnim.ScalingKeys != IntPtr.Zero) {
                m_scalingKeys = MemoryHelper.MarshalArray<VectorKey>(nodeAnim.ScalingKeys, (int) nodeAnim.NumScalingKeys);
            }
        }
    }
}
