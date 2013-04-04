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
using System.Collections.Generic;
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
    public sealed class NodeAnimationChannel : IMarshalable<NodeAnimationChannel, AiNodeAnim> {
        private String m_nodeName;
        private List<VectorKey> m_positionKeys;
        private List<QuaternionKey> m_rotationKeys;
        private List<VectorKey> m_scalingKeys;
        private AnimationBehaviour m_preState;
        private AnimationBehaviour m_postState;

        /// <summary>
        /// Gets or sets the name of the node affected by this animation. It must <c>exist</c> and it <c>must</c>
        /// be unique.
        /// </summary>
        public String NodeName {
            get {
                return m_nodeName;
            }
            set {
                m_nodeName = value;
            }
        }

        /// <summary>
        /// Gets the number of position keys in the animation channel.
        /// </summary>
        public int PositionKeyCount {
            get {
                return m_positionKeys.Count;
            }
        }

        /// <summary>
        /// Gets if this animation channel contains position keys.
        /// </summary>
        public bool HasPositionKeys {
            get {
                return m_positionKeys.Count > 0;
            }
        }

        /// <summary>
        /// Gets the position keys of this animation channel. Positions are
        /// specified as a 3D vector. If there are position keys, there should
        /// also be -at least- one scaling and one rotation key.
        /// </summary>
        public List<VectorKey> PositionKeys {
            get {
                return m_positionKeys;
            }
        }

        /// <summary>
        /// Gets the number of rotation keys in the animation channel.
        /// </summary>
        public int RotationKeyCount {
            get {
                return m_rotationKeys.Count;
            }
        }

        /// <summary>
        /// Gets if the animation channel contains rotation keys.
        /// </summary>
        public bool HasRotationKeys {
            get {
                return m_rotationKeys.Count > 0;
            }
        }

        /// <summary>
        /// Gets the rotation keys of this animation channel. Rotations are
        /// given as quaternions. If this exists, there should be -at least- one
        /// scaling and one position key.
        /// </summary>
        public List<QuaternionKey> RotationKeys {
            get {
                return m_rotationKeys;
            }
        }

        /// <summary>
        /// Gets the number of scaling keys in the animation channel.
        /// </summary>
        public int ScalingKeyCount {
            get {
                return m_scalingKeys.Count;
            }
        }

        /// <summary>
        /// Gets if the animation channel contains scaling keys.
        /// </summary>
        public bool HasScalingKeys {
            get {
                return m_scalingKeys.Count > 0;
            }
        }

        /// <summary>
        /// Gets the scaling keys of this animation channel. Scalings are
        /// specified in a 3D vector. If there are scaling keys, there should
        /// also be -at least- one position and one rotation key.
        /// </summary>
        public List<VectorKey> ScalingKeys {
            get {
                return m_scalingKeys;
            }
        }

        /// <summary>
        /// Gets or sets how the animation behaves before the first key is encountered. By default the original
        /// transformation matrix of the affected node is used.
        /// </summary>
        public AnimationBehaviour PreState {
            get {
                return m_preState;
            }
            set {
                m_preState = value;
            }
        }

        /// <summary>
        /// Gets or sets how the animation behaves after the last key was processed. By default the original
        /// transformation matrix of the affected node is taken.
        /// </summary>
        public AnimationBehaviour PostState {
            get {
                return m_postState;
            }
            set {
                m_postState = value;
            }
        }

        /// <summary>
        /// Constructs a new NodeAnimation.
        /// </summary>
        /// <param name="nodeAnim">Unmanaged AiNodeAnim struct</param>
        internal NodeAnimationChannel(ref AiNodeAnim nodeAnim) {
            m_nodeName = nodeAnim.NodeName.GetString();
            m_preState = nodeAnim.Prestate;
            m_postState = nodeAnim.PostState;

            //Load position keys
            if(nodeAnim.NumPositionKeys > 0 && nodeAnim.PositionKeys != IntPtr.Zero) {
                m_positionKeys.AddRange(MemoryHelper.MarshalArray<VectorKey>(nodeAnim.PositionKeys, (int) nodeAnim.NumPositionKeys));
            }

            //Load rotation keys
            if(nodeAnim.NumRotationKeys > 0 && nodeAnim.RotationKeys != IntPtr.Zero) {
                m_rotationKeys.AddRange(MemoryHelper.MarshalArray<QuaternionKey>(nodeAnim.RotationKeys, (int) nodeAnim.NumRotationKeys));
            }

            //Load scaling keys
            if(nodeAnim.NumScalingKeys > 0 && nodeAnim.ScalingKeys != IntPtr.Zero) {
                m_scalingKeys.AddRange(MemoryHelper.MarshalArray<VectorKey>(nodeAnim.ScalingKeys, (int) nodeAnim.NumScalingKeys));
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="NodeAnimationChannel"/> class.
        /// </summary>
        public NodeAnimationChannel() {
            m_nodeName = String.Empty;
            m_preState = AnimationBehaviour.Default;
            m_postState = AnimationBehaviour.Default;

            m_positionKeys = new List<VectorKey>();
            m_rotationKeys = new List<QuaternionKey>();
            m_scalingKeys = new List<VectorKey>();
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<NodeAnimationChannel, AiNodeAnim>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<NodeAnimationChannel, AiNodeAnim>.ToNative(IntPtr thisPtr, out AiNodeAnim nativeValue) {
            nativeValue.NodeName = new AiString(m_nodeName);
            nativeValue.Prestate = m_preState;
            nativeValue.PostState = m_postState;

            nativeValue.NumPositionKeys = (uint) m_positionKeys.Count;
            nativeValue.PositionKeys = IntPtr.Zero;
            
            if(nativeValue.NumPositionKeys > 0)
                MemoryHelper.ToNativeArray<VectorKey>(m_positionKeys.ToArray());


            nativeValue.NumRotationKeys = (uint) m_rotationKeys.Count;
            nativeValue.RotationKeys = IntPtr.Zero;
            
            if(nativeValue.NumRotationKeys > 0)
                MemoryHelper.ToNativeArray<QuaternionKey>(m_rotationKeys.ToArray());


            nativeValue.NumScalingKeys = (uint) m_scalingKeys.Count;
            nativeValue.ScalingKeys = IntPtr.Zero;
            
            if(nativeValue.NumScalingKeys > 0)
                MemoryHelper.ToNativeArray<VectorKey>(m_scalingKeys.ToArray());
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<NodeAnimationChannel, AiNodeAnim>.FromNative(ref AiNodeAnim nativeValue) {
            m_nodeName = nativeValue.NodeName.GetString();
            m_preState = nativeValue.Prestate;
            m_postState = nativeValue.PostState;

            m_positionKeys.Clear();
            m_rotationKeys.Clear();
            m_scalingKeys.Clear();

            if(nativeValue.NumPositionKeys > 0 && nativeValue.PositionKeys != IntPtr.Zero)
                m_positionKeys.AddRange(MemoryHelper.FromNativeArray<VectorKey>(nativeValue.PositionKeys, (int) nativeValue.NumPositionKeys));

            if(nativeValue.NumRotationKeys > 0 && nativeValue.RotationKeys != IntPtr.Zero)
                m_rotationKeys.AddRange(MemoryHelper.FromNativeArray<QuaternionKey>(nativeValue.RotationKeys, (int) nativeValue.NumRotationKeys));

            if(nativeValue.NumScalingKeys > 0 && nativeValue.ScalingKeys != IntPtr.Zero)
                m_scalingKeys.AddRange(MemoryHelper.FromNativeArray<VectorKey>(nativeValue.ScalingKeys, (int) nativeValue.NumScalingKeys));
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiNodeAnim aiNodeAnim = MemoryHelper.Read<AiNodeAnim>(nativeValue);

            if(aiNodeAnim.NumPositionKeys > 0 && aiNodeAnim.PositionKeys != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiNodeAnim.PositionKeys);

            if(aiNodeAnim.NumRotationKeys > 0 && aiNodeAnim.RotationKeys != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiNodeAnim.RotationKeys);

            if(aiNodeAnim.NumScalingKeys > 0 && aiNodeAnim.ScalingKeys != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiNodeAnim.ScalingKeys);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
