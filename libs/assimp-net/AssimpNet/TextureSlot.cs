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

namespace Assimp {

    /// <summary>
    /// Describes all the values pertaining to a particular texture slot in a material.
    /// </summary>
    [Serializable]
    public struct TextureSlot {
        private String m_filePath;
        private TextureType m_type;
        private uint m_index;
        private TextureMapping m_mapping;
        private uint m_uvIndex;
        private float m_blendFactor;
        private TextureOperation m_texOp;
        private TextureWrapMode m_wrapModeU;
        private TextureWrapMode m_wrapModeV;
        private uint m_flags;

        /// <summary>
        /// Gets the texture file path.
        /// </summary>
        public String FilePath {
            get {
                return m_filePath;
            }
        }

        /// <summary>
        /// Gets the texture type semantic.
        /// </summary>
        public TextureType TextureType {
            get {
                return m_type;
            }
        }

        /// <summary>
        /// Gets the texture index in the material.
        /// </summary>
        public uint TextureIndex {
            get {
                return m_index;
            }
        }

        /// <summary>
        /// Gets the texture mapping.
        /// </summary>
        public TextureMapping Mapping {
            get {
                return m_mapping;
            }
        }

        /// <summary>
        /// Gets the UV channel index that corresponds to this texture from the mesh.
        /// </summary>
        public uint UVIndex {
            get {
                return m_uvIndex;
            }
        }

        /// <summary>
        /// Gets the blend factor.
        /// </summary>
        public float BlendFactor {
            get {
                return m_blendFactor;
            }
        }

        /// <summary>
        /// Gets the texture operation.
        /// </summary>
        public TextureOperation Operation {
            get {
                return m_texOp;
            }
        }

        /// <summary>
        /// Gets the texture wrap mode for the U coordinate.
        /// </summary>
        public TextureWrapMode WrapModeU {
            get {
                return m_wrapModeU;
            }
        }

        /// <summary>
        /// Gets the texture wrap mode for the V coordinate.
        /// </summary>
        public TextureWrapMode WrapModeV {
            get {
                return m_wrapModeV;
            }
        }

        /// <summary>
        /// Gets misc flags.
        /// </summary>
        public uint Flags {
            get {
                return m_flags;
            }
        }

        /// <summary>
        /// Constructs a new TextureSlot.
        /// </summary>
        /// <param name="filePath">Texture filepath</param>
        /// <param name="typeSemantic">Texture type semantic</param>
        /// <param name="texIndex">Texture index in the material</param>
        /// <param name="mapping">Texture mapping</param>
        /// <param name="uvIndex">UV channel in mesh that corresponds to this texture</param>
        /// <param name="blendFactor">Blend factor</param>
        /// <param name="texOp">Texture operation</param>
        /// <param name="wrapModeU">Texture wrap mode for U coordinate</param>
        /// <param name="wrapModeV">Texture wrap mode for V coordinate</param>
        /// <param name="flags">Misc flags</param>
        public TextureSlot(String filePath, TextureType typeSemantic, uint texIndex, TextureMapping mapping, uint uvIndex, float blendFactor,
            TextureOperation texOp, TextureWrapMode wrapModeU, TextureWrapMode wrapModeV, uint flags) {
                m_filePath = (filePath == null) ? String.Empty : filePath;
                m_type = typeSemantic;
                m_index = texIndex;
                m_mapping = mapping;
                m_uvIndex = uvIndex;
                m_blendFactor = blendFactor;
                m_texOp = texOp;
                m_wrapModeU = wrapModeU;
                m_wrapModeV = wrapModeV;
                m_flags = flags;
        }
    }
}
