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

using DevIL.Unmanaged;

namespace DevIL {
    public class ImageData {
        private ImageInfo m_info;
        private byte[] m_data;
        private byte[] m_compressedData;
        private byte[] m_paletteData;

        public DataFormat Format {
            get {
                return m_info.Format;
            }
        }

        public CompressedDataFormat DxtcFormat {
            get {
                return m_info.DxtcFormat;
            }
        }

        public DataType DataType {
            get {
                return m_info.DataType;
            }
        }

        public PaletteType PaletteType {
            get {
                return m_info.PaletteType;
            }
        }

        public DataFormat PaletteBaseType {
            get {
                return m_info.PaletteBaseType;
            }
        }

        public CubeMapFace CubeFace {
            get {
                return m_info.CubeFlags;
            }
        }

        public OriginLocation Origin {
            get {
                return m_info.Origin;
            }
        }

        public int Width {
            get {
                return m_info.Width;
            }
        }

        public int Height {
            get {
                return m_info.Height;
            }
        }

        public int Depth {
            get {
                return m_info.Depth;
            }
        }

        public int BitsPerPixel {
            get {
                return m_info.BitsPerPixel;
            }
        }

        public int BytesPerPixel {
            get {
                return m_info.BytesPerPixel;
            }
        }

        public int ChannelCount {
            get {
                return m_info.Channels;
            }
        }

        public int Duration {
            get {
                return m_info.Duration;
            }
        }

        public int SizeOfData {
            get {
                return m_info.SizeOfData;
            }
        }

        public int OffsetX {
            get {
                return m_info.OffsetX;
            }
        }

        public int OffsetY {
            get {
                return m_info.OffsetY;
            }
        }

        public int PlaneSize {
            get {
                return m_info.PlaneSize;
            }
        }

        public int PaletteBytesPerPixel {
            get {
                return m_info.PaletteBytesPerPixel;
            }
        }

        public int PaletteColumnCount {
            get {
                return m_info.PaletteColumnCount;
            }
        }

        public bool HasDXTCData {
            get {
                return m_info.HasDXTC && m_compressedData != null;
            }
        }

        public bool HasPaletteData {
            get {
                return m_info.HasPalette && m_paletteData != null;
            }
        }

        public bool IsCubeMap {
            get {
                return m_info.IsCubeMap;
            }
        }

        public bool IsSphereMap {
            get {
                return m_info.IsSphereMap;
            }
        }

        public byte[] Data {
            get {
                return m_data;
            }
        }

        public byte[] CompressedData {
            get {
                return m_compressedData;
            }
        }

        public byte[] PaletteData {
            get {
                return m_paletteData;
            }
        }

        private ImageData() { }

        internal static ImageData Load(Subimage subimage) {
            if(!subimage.Activate())
                return null;

            ImageData imageData = new ImageData();
            imageData.m_info = IL.GetImageInfo();
            imageData.m_data = IL.GetImageData();

            //If no uncompressed data, we're here by accident, abort
            if(imageData.m_data == null)
                return null;

            if(imageData.m_info.HasDXTC) {
                imageData.m_compressedData = IL.GetDxtcData(imageData.DxtcFormat);
            }

            if(imageData.m_info.HasPalette) {
                imageData.m_paletteData = IL.GetPaletteData();
            }

            return imageData;
        }
    }
}
