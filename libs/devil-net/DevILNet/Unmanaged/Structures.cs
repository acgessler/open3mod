/*
* Copyright (c) 2012 Nicholas Woodfield
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
using System.Runtime.InteropServices;

namespace DevIL.Unmanaged {
    /// <summary>
    /// Represents a specific image surface - e.g. a mipmap, another image in the image array, a face in a cubemap, etc. All surface
    /// numbers set to zero with a valid ImageID represents the root image.
    /// </summary>
    public struct Subimage {
        private ImageID m_rootImage;
        private int m_imageIndex;
        private int m_faceIndex;
        private int m_layerIndex;
        private int m_mipMapIndex;

        public ImageID RootImage {
            get {
                return m_rootImage;
            }
        }

        public int ImageIndex {
            get {
                return m_imageIndex;
            }
        }

        public int FaceIndex {
            get {
                return m_faceIndex;
            }
        }

        public int LayerIndex {
            get {
                return m_layerIndex;
            }
        }

        public int MipMapIndex {
            get {
                return m_mipMapIndex;
            }
        }

        public Subimage(ImageID rootImage) {
            m_rootImage = rootImage;
            m_imageIndex = 0;
            m_faceIndex = 0;
            m_layerIndex = 0;
            m_mipMapIndex = 0;
        }

        public Subimage(ImageID rootImage, int imageIndex) {
            m_rootImage = rootImage;
            m_imageIndex = imageIndex;
            m_faceIndex = 0;
            m_layerIndex = 0;
            m_mipMapIndex = 0;
        }

        public Subimage(ImageID rootImage, int imageIndex, int faceIndex) {
            m_rootImage = rootImage;
            m_imageIndex = imageIndex;
            m_faceIndex = faceIndex;
            m_layerIndex = 0;
            m_mipMapIndex = 0;
        }

        public Subimage(ImageID rootImage, int imageIndex, int faceIndex, int layerIndex) {
            m_rootImage = rootImage;
            m_imageIndex = imageIndex;
            m_faceIndex = faceIndex;
            m_layerIndex = layerIndex;
            m_mipMapIndex = 0;
        }

        public Subimage(ImageID rootImage, int imageIndex, int faceIndex, int layerIndex, int mipMapIndex) {
            m_rootImage = rootImage;
            m_imageIndex = imageIndex;
            m_faceIndex = faceIndex;
            m_layerIndex = layerIndex;
            m_mipMapIndex = mipMapIndex;
        }

        public bool Activate() {
            if(m_rootImage <= 0)
                return false;

            IL.BindImage(m_rootImage);

            //Don't bother to activate if any subimages are zero, as it corresponds to the root image

            if(m_imageIndex > 0 && !IL.ActiveImage(m_imageIndex))
                return false;

            if(m_faceIndex > 0 && !IL.ActiveFace(m_faceIndex))
                return false;

            if(m_layerIndex > 0 && !IL.ActiveLayer(m_layerIndex))
                return false;

            if(m_mipMapIndex > 0 && !IL.ActiveMipMap(m_mipMapIndex))
                return false;

            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageID : IEquatable<ImageID> {
        private int m_id;

        public int ID {
            get {
                return m_id;
            }
        }

        public ImageID(int id) {
            m_id = id;
        }

        public static implicit operator ImageID(int id) {
            return new ImageID(id);
        }

        public static implicit operator int(ImageID id) {
            return id.m_id;
        }

        public static bool operator <(ImageID a, ImageID b) {
            return (a.m_id < b.m_id);
        }

        public static bool operator >(ImageID a, ImageID b) {
            return (a.m_id > b.m_id);
        }

        public static bool operator ==(ImageID a, ImageID b) {
            return (a.m_id == b.m_id);
        }

        public static bool operator !=(ImageID a, ImageID b) {
            return (a.m_id != b.m_id);
        }

        public bool Equals(ImageID other) {
            return m_id == other.m_id;
        }

        public override bool Equals(object obj) {
            if(obj is ImageID) {
                ImageID other = (ImageID) obj;
                return m_id == other.m_id;
            }
            return false;
        }

        public override int GetHashCode() {
            return m_id.GetHashCode();
        }

        public override string ToString() {
            return String.Format("ImageID: {0}", m_id.ToString());
        }

    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct ImageInfo {
        public DataFormat Format;
        public CompressedDataFormat DxtcFormat;
        public DataType DataType;
        public PaletteType PaletteType;
        public DataFormat PaletteBaseType;
        public CubeMapFace CubeFlags;
        public OriginLocation Origin;
        public int Width;
        public int Height;
        public int Depth;
        public int BitsPerPixel;
        public int BytesPerPixel;
        public int Channels;
        public int Duration;
        public int SizeOfData;
        public int OffsetX;
        public int OffsetY;
        public int PlaneSize;
        public int FaceCount;
        public int ImageCount;
        public int LayerCount;
        public int MipMapCount;
        public int PaletteBytesPerPixel;
        public int PaletteColumnCount;

        public bool HasDXTC {
            get {
                return DxtcFormat != CompressedDataFormat.None;
            }
        }

        public bool HasPalette {
            get {
                return PaletteType != DevIL.PaletteType.None;
            }
        }

        public bool IsCubeMap {
            get {
                return CubeFlags != CubeMapFace.None && CubeFlags != CubeMapFace.SphereMap;
            }
        }

        public bool IsSphereMap {
            get {
                return CubeFlags == CubeMapFace.SphereMap;
            }
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct PointF {
        float X;
        float Y;

        public PointF(float x, float y) {
            X = x;
            Y = y;
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct PointI {
        int X;
        int Y;

        public PointI(int x, int y) {
            X = x;
            Y = y;
        }
    }
}
