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
using DevIL.Unmanaged;

namespace DevIL {

    /// <summary>
    /// Represents a "root" image that is currently valid in DevIL. The root image may be the first image in an image array (e.g. animation), 
    /// and each image may contain a number of subimages - mipmaps or faces if the image is a cubemap (positiveX, positiveY, etc). Each surface can
    /// be loaded individually, or the entire image chain can be loaded into a ManagedImage.
    /// </summary>
    public sealed class Image : IDisposable, IEquatable<Image> {
        private bool m_isDisposed = false;
        private ImageID m_id;

        private static Image s_default = new Image(new ImageID(0));

        internal ImageID ImageID {
            get {
                return m_id;
            }
        }

        internal bool IsValid {
            get {
                return m_id >= 0 && IL.IsInitialized; //Just in case someone tries to use it after the last importer is disposed
            }
        }

        public static Image DefaultImage {
            get {
                return s_default;
            }
        }

        #region ImageInfo Properties

        public DataFormat Format {
            get {
                if(!IsValid)
                    return DataFormat.RGBA;

                Bind();
                return (DataFormat) IL.ilGetInteger(ILDefines.IL_IMAGE_FORMAT);
            }
        }

        public CompressedDataFormat DxtcFormat {
            get {
                if(!IsValid)
                    return CompressedDataFormat.None;

                Bind();
                return (CompressedDataFormat) IL.ilGetInteger(ILDefines.IL_DXTC_DATA_FORMAT);
            }
        }

        public DataType DataType {
            get {
                if(!IsValid)
                    return DevIL.DataType.UnsignedByte;

                Bind();
                return (DevIL.DataType) IL.ilGetInteger(ILDefines.IL_IMAGE_TYPE);
            }
        }

        public PaletteType PaletteType {
            get {
                if(!IsValid)
                    return DevIL.PaletteType.None;

                Bind();
                return (DevIL.PaletteType) IL.ilGetInteger(ILDefines.IL_PALETTE_TYPE);
            }
        }

        public DataFormat PaletteBaseType {
            get {
                if(!IsValid)
                    return DataFormat.RGBA;

                Bind();
                return (DataFormat) IL.ilGetInteger(ILDefines.IL_PALETTE_BASE_TYPE);
            }
        }

        public OriginLocation Origin {
            get {
                if(!IsValid)
                    return OriginLocation.UpperLeft;

                Bind();
                return (OriginLocation) IL.ilGetInteger(ILDefines.IL_IMAGE_ORIGIN);
            }
        }

        public int Width {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageWidth);
            }
        }

        public int Height {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageHeight);
            }
        }

        public int Depth {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageDepth);
            }
        }

        public int BytesPerPixel {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageBytesPerPixel);
            }
        }

        public int BitsPerPixel {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageBitsPerPixel);
            }
        }

        public int ChannelCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageChannels);
            }
        }

        public int PaletteBytesPerPixel {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImagePaletteBytesPerPixel);
            }
        }

        public int PaletteColumnCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImagePaletteColumnCount);
            }
        }

        public int FaceCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageFaceCount) + 1;
            }
        }

        public int ImageArrayCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageArrayCount) + 1;
            }
        }

        public int MipMapCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageMipMapCount) + 1;
            }
        }

        public int LayerCount {
            get {
                if(!IsValid)
                    return 0;

                Bind();
                return IL.GetInteger(ILIntegerMode.ImageLayerCount) + 1;
            }
        }

        public bool HasDXTCData {
            get {
                if(!IsValid)
                    return false;
                return this.DxtcFormat != CompressedDataFormat.None;
            }
        }

        public bool HasPaletteData {
            get {
                if(!IsValid)
                    return false;
                return this.PaletteType != DevIL.PaletteType.None;
            }
        }

        public bool IsCubeMap {
            get {
                if(!IsValid)
                    return false;
                CubeMapFace face = (CubeMapFace) IL.ilGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS);
                return (face != CubeMapFace.None) && (face != CubeMapFace.SphereMap);
            }
        }

        public bool IsSphereMap {
            get {
                if(!IsValid)
                    return false;
                CubeMapFace face = (CubeMapFace) IL.ilGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS);
                return face == CubeMapFace.SphereMap;
            }
        }

        #endregion

        internal Image(ImageID id) {
            m_id = id;
        }

        ~Image() {
            Dispose(false);
        }

        public void Bind() {
            IL.BindImage(m_id);
        }

        public bool ConvertToDxtc(CompressedDataFormat compressedFormat) {
            if(!CheckValid(this))
                return false;

            Bind();
            return IL.ImageToDxtcData(compressedFormat);
        }

        public bool CopyFrom(Image srcImage) {
            if(!CheckValid(this) || !CheckValid(srcImage))
                return false;

            Bind();
            return IL.CopyImage(srcImage.ImageID);
        }

        public bool CopyTo(Image destImage) {
            if(!CheckValid(this) || !CheckValid(destImage))
                return false;

            destImage.Bind();
            return IL.CopyImage(this.ImageID);
        }

        public Image Clone() {
            ImageID newID = IL.GenerateImage();
            Image clone = new Image(newID);
            IL.BindImage(newID);
            IL.CopyImage(m_id);
            return clone;
        }

        public void Resize(int width, int height, int depth, SamplingFilter filter, bool regenerateMipMaps) {

            width = Math.Max(1, width);
            height = Math.Max(1, height);
            depth = Math.Max(1, depth);

            Bind();

            SamplingFilter oldFilter = ILU.GetSamplingFilter();
            ILU.SetSamplingFilter(filter);
            ILU.Scale(width, height, depth);

            if(regenerateMipMaps) {
                Bind();
                ILU.BuildMipMaps();
            }

            ILU.SetSamplingFilter(oldFilter);
        }

        public void ResizeToNearestPowerOfTwo(SamplingFilter filter, bool regenerateMipMaps) {
            int width = Width;
            int height = Height;
            int depth = Depth;

            width = MemoryHelper.RoundToNearestPowerOfTwo(width);
            height = MemoryHelper.RoundToNearestPowerOfTwo(height);
            depth = MemoryHelper.RoundToNearestPowerOfTwo(depth);

            Bind();

            SamplingFilter oldFilter = ILU.GetSamplingFilter();
            ILU.SetSamplingFilter(filter);
            ILU.Scale(width, height, depth);

            if(regenerateMipMaps) {
                Bind();
                ILU.BuildMipMaps();
            }

            ILU.SetSamplingFilter(oldFilter);
        }

        public ImageInfo GetImageInfo() {
            ImageInfo info = new ImageInfo();
            if(CheckValid(this)) {
                Bind();
                info = IL.GetImageInfo();
            }
            return info;
        }

        public ImageData GetImageData(int imageIndex, int faceIndex, int layerIndex, int mipmapIndex) {
            if(!IsValid || imageIndex < 0 || faceIndex < 0 || layerIndex < 0 || mipmapIndex < 0)
                return null;

            Subimage subimage = new Subimage(m_id, imageIndex, faceIndex, layerIndex, mipmapIndex);
            return ImageData.Load(subimage);
        }

        public ImageData GetImageData(CubeMapFace cubeMapFace, int mipmapIndex) {
            if(!IsValid || mipmapIndex < 0)
                return null;

            int faceCount = FaceCount;
            for(int i = 0; i < faceCount; i++) {
                Bind();
                IL.ActiveFace(i);
                CubeMapFace face = (CubeMapFace) IL.ilGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS);

                if(face == cubeMapFace) {
                    return ImageData.Load(new Subimage(m_id, 0, i, 0, mipmapIndex));
                }
            }

            return null;
        }

        public ImageData GetImageData(int mipmapIndex) {
            if(!IsValid || mipmapIndex < 0)
                return null;

            Subimage subimage = new Subimage(m_id, 0, 0, 0, mipmapIndex);
            return ImageData.Load(subimage);
        }

        public ManagedImage ToManaged() {
            if(IsValid) {
                return new ManagedImage(this);
            }
            return null;
        }

        public static bool CheckValid(Image image) {
            if(image != null && image.IsValid)
                return true;

            return false;
        }

        public bool Equals(Image other) {
            if(other.ImageID == ImageID)
                return true;

            return false;
        }

        public override bool Equals(object obj) {
            Image image = obj as Image;
            if(image == null) {
                return false;
            } else {
                return image.ImageID == ImageID;
            }
        }

        public override int GetHashCode() {
            return m_id.GetHashCode();
        }

        public override string ToString() {
            return m_id.ToString();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (m_isDisposed) {
                if (m_id > 0) {
                    IL.DeleteImage(m_id);
                }
                m_isDisposed = true;
            }
        }
    }
}
