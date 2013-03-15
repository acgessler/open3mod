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
    /// An animation consists of keyframe data for a number of nodes. For
    /// each node affected by the animation, a separate series of data is given.
    /// </summary>
    public sealed class Animation {
        private String m_name;
        private double m_duration;
        private double m_ticksPerSecond;
        private NodeAnimationChannel[] m_channels;
        private MeshAnimationChannel[] m_meshChannels;

        /// <summary>
        /// Gets the name of the animation. If the modeling package the
        /// data was exported from only supports a single animation channel, this
        /// name is usually empty.
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the duration of the animation in number of ticks.
        /// </summary>
        public double DurationInTicks {
            get {
                return m_duration;
            }
        }

        /// <summary>
        /// Gets the number of ticks per second. It may be zero
        /// if it is not specified in the imported file.
        /// </summary>
        public double TicksPerSecond {
            get {
                return m_ticksPerSecond;
            }
        }

        /// <summary>
        /// Gets the number of node animation channels where each channel
        /// affects a single node.
        /// </summary>
        public int NodeAnimationChannelCount {
            get {
                return (m_channels == null) ? 0 : m_channels.Length;
            }
        }

        /// <summary>
        /// Checks if the animation has node animation channels.
        /// </summary>
        public bool HasNodeAnimations {
            get {
                return m_channels != null;
            }
        }

        /// <summary>
        /// Gets the node animation channels.
        /// </summary>
        public NodeAnimationChannel[] NodeAnimationChannels {
            get {
                return m_channels;
            }
        }

        /// <summary>
        /// Gets the number of mesh animation channels.
        /// </summary>
        public int MeshAnimationChannelCount {
            get {
                return (m_meshChannels == null) ? 0 : m_meshChannels.Length;
            }
        }

        /// <summary>
        /// Checks if the animation has mesh animations.
        /// </summary>
        public bool HasMeshAnimations {
            get {
                return m_meshChannels != null;
            }
        }

        /// <summary>
        /// Gets the mesh animation channels.
        /// </summary>
        public MeshAnimationChannel[] MeshAnimationChannels {
            get {
                return m_meshChannels;
            }
        }

        /// <summary>
        /// Constructs a new Animation.
        /// </summary>
        /// <param name="animation">Unmanaged AiAnimation.</param>
        internal Animation(ref AiAnimation animation) {
            m_name = animation.Name.GetString();
            m_duration = animation.Duration;
            m_ticksPerSecond = animation.TicksPerSecond;

            //Load node animations
            if(animation.NumChannels > 0 && animation.Channels != IntPtr.Zero) {
                AiNodeAnim[] nodeAnims = MemoryHelper.MarshalArray<AiNodeAnim>(animation.Channels, (int) animation.NumChannels, true);
                m_channels = new NodeAnimationChannel[nodeAnims.Length];
                for(int i = 0; i < m_channels.Length; i++) {
                    m_channels[i] = new NodeAnimationChannel(ref nodeAnims[i]);
                }
            }

            //Load mesh animations
            if(animation.NumMeshChannels > 0 && animation.MeshChannels != IntPtr.Zero) {
                AiMeshAnim[] meshAnims = MemoryHelper.MarshalArray<AiMeshAnim>(animation.MeshChannels, (int) animation.NumMeshChannels, true);
                m_meshChannels = new MeshAnimationChannel[meshAnims.Length];
                for(int i = 0; i < m_meshChannels.Length; i++) {
                    m_meshChannels[i] = new MeshAnimationChannel(ref meshAnims[i]);
                }
            }
        }
    }
}
