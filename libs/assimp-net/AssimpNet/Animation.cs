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
    /// An animation consists of keyframe data for a number of nodes. For
    /// each node affected by the animation, a separate series of data is given.
    /// </summary>
    public sealed class Animation : IMarshalable<Animation, AiAnimation> {
        private String m_name;
        private double m_duration;
        private double m_ticksPerSecond;
        private List<NodeAnimationChannel> m_nodeChannels;
        private List<MeshAnimationChannel> m_meshChannels;

        /// <summary>
        /// Gets or sets the name of the animation. If the modeling package the
        /// data was exported from only supports a single animation channel, this
        /// name is usually empty.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animation in number of ticks.
        /// </summary>
        public double DurationInTicks {
            get {
                return m_duration;
            }
            set {
                m_duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of ticks per second. It may be zero
        /// if it is not specified in the imported file.
        /// </summary>
        public double TicksPerSecond {
            get {
                return m_ticksPerSecond;
            }
            set {
                m_ticksPerSecond = value;
            }
        }

        /// <summary>
        /// Gets if the animation has node animation channels.
        /// </summary>
        public bool HasNodeAnimations {
            get {
                return m_nodeChannels.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of node animation channels where each channel
        /// affects a single node.
        /// </summary>
        public int NodeAnimationChannelCount {
            get {
                return m_nodeChannels.Count;
            }
        }

        /// <summary>
        /// Gets the node animation channels.
        /// </summary>
        public List<NodeAnimationChannel> NodeAnimationChannels {
            get {
                return m_nodeChannels;
            }
        }

        /// <summary>
        /// Gets if the animation has mesh animations.
        /// </summary>
        public bool HasMeshAnimations {
            get {
                return m_meshChannels.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of mesh animation channels.
        /// </summary>
        public int MeshAnimationChannelCount {
            get {
                return m_meshChannels.Count;
            }
        }

        /// <summary>
        /// Gets the mesh animation channels.
        /// </summary>
        public List<MeshAnimationChannel> MeshAnimationChannels {
            get {
                return m_meshChannels;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Animation"/> class.
        /// </summary>
        public Animation() {
            m_name = String.Empty;
            m_duration = 0;
            m_ticksPerSecond = 0;
            m_nodeChannels = new List<NodeAnimationChannel>();
            m_meshChannels = new List<MeshAnimationChannel>();
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Animation, AiAnimation>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Animation, AiAnimation>.ToNative(IntPtr thisPtr, out AiAnimation nativeValue) {
            nativeValue.Name = new AiString(m_name);
            nativeValue.Duration = m_duration;
            nativeValue.TicksPerSecond = m_ticksPerSecond;
            nativeValue.NumChannels = (uint) NodeAnimationChannelCount;
            nativeValue.NumMeshChannels = (uint) MeshAnimationChannelCount;
            nativeValue.Channels = IntPtr.Zero;
            nativeValue.MeshChannels = IntPtr.Zero;

            if(nativeValue.NumChannels > 0)
                nativeValue.Channels = MemoryHelper.ToNativeArray<NodeAnimationChannel, AiNodeAnim>(m_nodeChannels.ToArray(), true);

            if(nativeValue.NumMeshChannels > 0)
                nativeValue.MeshChannels = MemoryHelper.ToNativeArray<MeshAnimationChannel, AiMeshAnim>(m_meshChannels.ToArray(), true);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Animation, AiAnimation>.FromNative(ref AiAnimation nativeValue) {
            m_nodeChannels.Clear();
            m_meshChannels.Clear();

            m_name = nativeValue.Name.GetString();
            m_duration = nativeValue.Duration;
            m_ticksPerSecond = nativeValue.TicksPerSecond;

            if(nativeValue.NumChannels > 0 && nativeValue.Channels != IntPtr.Zero)
                m_nodeChannels.AddRange(MemoryHelper.FromNativeArray<NodeAnimationChannel, AiNodeAnim>(nativeValue.Channels, (int) nativeValue.NumChannels, true));

            if(nativeValue.NumMeshChannels > 0 && nativeValue.MeshChannels != IntPtr.Zero)
                m_meshChannels.AddRange(MemoryHelper.FromNativeArray<MeshAnimationChannel, AiMeshAnim>(nativeValue.MeshChannels, (int) nativeValue.NumMeshChannels, true));
        }


        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Animation, AiAnimation}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiAnimation aiAnimation = MemoryHelper.Read<AiAnimation>(nativeValue);

            if(aiAnimation.NumChannels > 0 && aiAnimation.Channels != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiNodeAnim>(aiAnimation.Channels, (int) aiAnimation.NumChannels, NodeAnimationChannel.FreeNative, true);

            if(aiAnimation.NumMeshChannels > 0 && aiAnimation.MeshChannels != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMeshAnim>(aiAnimation.MeshChannels, (int) aiAnimation.NumMeshChannels, MeshAnimationChannel.FreeNative, true);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
