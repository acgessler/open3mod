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
    /// Describes vertex-based animations for a single mesh or a group of meshes. Meshes
    /// carry the animation data for each frame. The purpose of this object is to define
    /// keyframes, linking each mesh attachment to a particular point in a time.
    /// </summary>
    public sealed class MeshAnimationChannel {
        private String m_name;
        private MeshKey[] m_meshKeys;

        /// <summary>
        /// Gets the name of the mesh to be animated. Empty strings are not allowed,
        /// animation meshes need to be named (not necessarily uniquely, the name can basically
        /// serve as a wildcard to select a group of meshes with similar animation setup).
        /// </summary>
        public String MeshName {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the number of meshkeys in this animation channel. There will always
        /// be at least one key.
        /// </summary>
        public int MeshKeyCount {
            get {
                return (m_meshKeys == null) ? 0 : m_meshKeys.Length;
            }
        }

        /// <summary>
        /// Checks if this animation channel has mesh keys - this should always be true.
        /// </summary>
        public bool HasMeshKeys {
            get {
                return m_meshKeys != null;
            }
        }

        /// <summary>
        /// Gets the mesh keyframes of the animation. This should not be null.
        /// </summary>
        public MeshKey[] MeshKeys {
            get {
                return m_meshKeys;
            }
        }

        /// <summary>
        /// Construct a new MeshAnimation.
        /// </summary>
        /// <param name="meshAnim">Unmanaged AiMeshAnim struct.</param>
        internal MeshAnimationChannel(ref AiMeshAnim meshAnim) {
            m_name = meshAnim.Name.GetString();
            
            //Load mesh keys
            if(meshAnim.NumKeys > 0 && meshAnim.Keys != IntPtr.Zero) {
                m_meshKeys = MemoryHelper.MarshalArray<MeshKey>(meshAnim.Keys, (int) meshAnim.NumKeys);
            }
        }
    }
}
