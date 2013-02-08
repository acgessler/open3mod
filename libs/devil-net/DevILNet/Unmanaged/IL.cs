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
using System.IO;
using System.Runtime.InteropServices;

namespace DevIL.Unmanaged {
    public static class IL {

        private const String ILDLL = "DevIL.dll";
        private static bool _init = false;
        private static Object s_sync = new Object();
        private static int s_ref = 0;

        public static bool IsInitialized {
            get {
                return _init;
            }
        }

        internal static void AddRef() {
            lock(s_sync) {
                if(s_ref == 0) {
                    IL.Initialize();
                    ILU.Initialize();
                }
                s_ref++;
            }
        }

        internal static void Release() {
            lock(s_sync) {
                if(s_ref != 0) {
                    s_ref--;

                    if(s_ref == 0) {
                        IL.Shutdown();
                    }
                }
            }
        }

        #region IL Methods

        public static bool ActiveFace(int faceNum) {
            if(faceNum >= 0) {
                return ilActiveFace((uint) faceNum);
            }
            return false;
        }

        public static bool ActiveImage(int imageNum) {
            if(imageNum >= 0) {
                return ilActiveImage((uint) imageNum);
            }
            return false;
        }

        public static bool ActiveLayer(int layerNum) {
            if(layerNum >= 0) {
                return ilActiveLayer((uint) layerNum);
            }
            return false;
        }

        public static bool ActiveMipMap(int mipMapNum) {
            if(mipMapNum >= 0) {
                return ilActiveMipmap((uint) mipMapNum);
            }
            return false;
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilApplyPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ApplyPalette([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        /* TODO
        ///InProfile: char*
        ///OutProfile: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilApplyProfile", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilApplyProfile(IntPtr InProfile, IntPtr OutProfile);
        */

        public static void BindImage(ImageID imageID) {
            if(imageID.ID >= 0) {
                ilBindImage((uint) imageID.ID);
            }
        }

        public static bool Blit(ImageID srcImageID, int destX, int destY, int destZ, int srcX, int srcY, int srcZ, int width, int height, int depth) {
            if(srcImageID.ID >= 0) {
                return ilBlit((uint) srcImageID.ID, destX, destY, destZ, (uint) srcX, (uint) srcY, (uint) srcZ, (uint) width, (uint) height, (uint) depth);
            }
            return false;
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilClampNTSC", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClampNTSC();

        [DllImportAttribute(ILDLL, EntryPoint = "ilClearColour", CallingConvention = CallingConvention.StdCall)]
        public static extern void ClearColor(float red, float green, float blue, float alpha);

        [DllImportAttribute(ILDLL, EntryPoint = "ilClearImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool ClearImage();

        public static int CloneCurrentImage() {
            return (int) ilCloneCurImage();
        }

        /* TODO: Needs further investigation
        public static byte[] CompressDXT(byte[] data, int width, int height, int depth, CompressedDataFormat dxtFormat) {
            if(data == null || data.Length == 0) {
                return null;
            }

            unsafe {
                fixed(byte* ptr = data) {
                    uint sizeOfData = 0;
                    IntPtr compressedData = ilCompressDXT(new IntPtr(ptr), (uint) width, (uint) height, (uint) depth, (uint) dxtFormat, ref sizeOfData);
                    if(compressedData == IntPtr.Zero) {
                        return null;
                    }

                    byte[] dataToReturn = MemoryHelper.ReadByteBuffer(compressedData, (int) sizeOfData);

                    //Memory leak, DevIL allocates data for us, how do we free it? Function is not like the others where we can create data to
                    //get filled or get the size needed.

                    return dataToReturn;
                }
            }
        }*/

        public static bool ConvertImage(DataFormat destFormat, DataType destType) {
            return ilConvertImage((uint) destFormat, (uint) destType);
        }

        public static bool ConvertPalette(PaletteType palType) {
            return ilConvertPal((uint) palType);
        }

        public static bool CopyImage(ImageID srcImageID) {
            return ilCopyImage((uint) srcImageID.ID);
        }

        /// <summary>
        /// Copies the currently bounded image data to a managed byte array that gets returned. The image copied is specified by the offsets and lengths supplied.
        /// Conversions to the format/data type are handled automatically.
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="zOffset">Z Offset</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        /// <param name="format">Data format to convert to</param>
        /// <param name="dataType">Data type to convert to</param>
        /// <returns>Managed byte array, or null if the operation failed</returns>
        public static byte[] CopyPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType) {
            int dataSize = MemoryHelper.GetDataSize(width, height, depth, format, dataType);
            byte[] data = new byte[dataSize];

            unsafe {
                fixed(byte* ptr = data) {
                    uint size = ilCopyPixels((uint) xOffset, (uint) yOffset, (uint) zOffset, (uint) width, (uint) height, (uint) depth, (uint) format, (uint) dataType, new IntPtr(ptr));
                    
                    //Zero denotes something went wrong
                    if(size == 0) {
                        return null;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// DevIL will copy the currently bounded image data to the specified pointer. The image copied is specified by the offsets and lengths supplied.
        /// Conversions to the format/data type are handled automatically.
        /// </summary>
        /// <param name="xOffset">X Offset</param>
        /// <param name="yOffset">Y Offset</param>
        /// <param name="zOffset">Z Offset</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="depth">Depth</param>
        /// <param name="format">Data format to convert to</param>
        /// <param name="dataType">Data type to convert to</param>
        /// <param name="data">Pointer to memory that the data will be copied to</param>
        /// <returns>True if the operation succeeded or not</returns>
        public static bool CopyPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType, IntPtr data) {
            if(data == IntPtr.Zero)
                return false;

            return ilCopyPixels((uint) xOffset, (uint) yOffset, (uint) zOffset, (uint) width, (uint) height, (uint) depth, (uint) format, (uint) dataType, data) > 0;
        }

        //Looks like it creates an empty image for either next/mip/layer for current image, then creates
        //N "next" images for the subimage
        public static bool CreateSubImage(SubImageType subImageType, int subImageCount) {
            //Returns 0 if something happened.
            if(ilCreateSubImage((uint) subImageType, (uint) subImageCount) != 0) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Initializes the currently bound image to the default image - a 128x128 checkerboard texture.
        /// </summary>
        /// <returns>True if successful</returns>
        [DllImportAttribute(ILDLL, EntryPoint = "ilDefaultImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DefaultImage();

        public static void DeleteImage(ImageID imageID) {
            //Dont delete default, and valid images are non-negative
            if(imageID > 0)
                return;

            ilDeleteImage((uint) imageID.ID);
        }

        public static void DeleteImages(ImageID[] imageIDs) {
            uint[] ids = new uint[imageIDs.Length];
            for(int i = 0; i < imageIDs.Length; i++) {
                ids[i] = (uint) imageIDs[i].ID;
            }

            UIntPtr size = new UIntPtr((uint) ids.Length);

            ilDeleteImages(size, ids);
        }

        public static ImageType DetermineImageType(String fileName) {
            if(String.IsNullOrEmpty(fileName)) {
                return ImageType.Unknown;
            }
            return (ImageType) ilDetermineType(fileName);
        }

        public static ImageType DetermineImageType(byte[] lump) {
            if(lump == null || lump.Length == 0)
                return ImageType.Unknown;

            uint size = (uint) lump.Length;

            unsafe {
                fixed(byte* ptr = lump) {
                    return (ImageType) ilDetermineTypeL(new IntPtr(ptr), size);
                }
            }
        }

        /// <summary>
        /// Determines the image type from the specified file extension.
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns></returns>
        public static ImageType DetermineImageTypeFromExtension(String extension) {
            if(String.IsNullOrEmpty(extension))
                return ImageType.Unknown;

            return (ImageType) ilTypeFromExt(extension);
        }

        /// <summary>
        /// Disables an enable bit.
        /// </summary>
        /// <param name="mode">Enable bit to disable</param>
        /// <returns>True if disabled</returns>
        public static bool Disable(ILEnable mode) {
            return ilDisable((uint) mode);
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilDxtcDataToImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToImage();

        [DllImportAttribute(ILDLL, EntryPoint = "ilDxtcDataToSurface", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool DxtcDataToSurface();

        /// <summary>
        /// Enables an enable bit.
        /// </summary>
        /// <param name="mode">Enable bit to enable</param>
        /// <returns>True if enabled</returns>
        public static bool Enable(ILEnable mode) {
            return ilEnable((uint) mode);
        }

        /// <summary>
        /// Flips the currently bound surface (image, mipmap, etc)'s dxtc data.
        /// </summary>
        [DllImportAttribute(ILDLL, EntryPoint = "ilFlipSurfaceDxtcData", CallingConvention = CallingConvention.StdCall)]
        public static extern void FlipSurfaceDxtcData();

        /// <summary>
        /// Creates an image and returns the image's id.
        /// </summary>
        /// <returns>Generated image id</returns>
        public static ImageID GenerateImage() {
            return new ImageID((int)ilGenImage());
        }

        /// <summary>
        /// Batch generates images and returns an array of the generated image ids.
        /// </summary>
        /// <param name="count">Number of images to generate</param>
        /// <returns>Generated images</returns>
        public static ImageID[] GenerateImages(int count) {
            UIntPtr num = new UIntPtr((uint) count);
            uint[] images = new uint[count];
            ilGenImages(num, images);

            ImageID[] copy = new ImageID[count];
            for(int i = 0; i < count; i++) {
                copy[i] = new ImageID((int)images[i]);
            }

            return copy;
        }

        /* Needs investigation
        public static byte[] GetAlphaData(DataType dataType) {
            //Returns a pointer that gets allocated, we don't have a way to release the memory?
        }*/

        public static bool GetBoolean(ILBooleanMode mode) {
            return (ilGetInteger((uint) mode) == 0) ? false : true;
        }

        public static int GetInteger(ILIntegerMode mode) {
            return ilGetInteger((uint) mode);
        }

        public static byte[] GetDxtcData(CompressedDataFormat dxtcFormat) {
            uint bufferSize = ilGetDXTCData(IntPtr.Zero, 0, (uint) dxtcFormat);
            if(bufferSize == 0) {
                return null;
            }
            byte[] buffer = new byte[bufferSize];

            unsafe {
                fixed(byte* ptr = buffer) {
                    ilGetDXTCData(new IntPtr(ptr), bufferSize, (uint) dxtcFormat);
                }
            }
            return buffer;
        }

        /// <summary>
        /// Gets the last set error.
        /// </summary>
        /// <returns>Error type</returns>
        public static ErrorType GetError() {
            return (ErrorType) ilGetError();
        }

        /// <summary>
        /// Gets the total (uncompressed) data of the currently bound image.
        /// </summary>
        /// <returns>Image data</returns>
        public static byte[] GetImageData() {
            IntPtr ptr = ilGetData();

            if(ptr == IntPtr.Zero) {
                return null;
            }

            int size = ilGetInteger((uint) ILDefines.IL_IMAGE_SIZE_OF_DATA);

            return MemoryHelper.ReadByteBuffer(ptr, size);
        }

        /// <summary>
        /// Gets an unmanaged pointer to the uncompressed data of the currently bound image.
        /// </summary>
        /// <returns>Unmanaged pointer to the image data</returns>
        public static IntPtr GetData() {
            return ilGetData();
        }

        public static byte[] GetPaletteData() {
            PaletteType type = (PaletteType) ilGetInteger((uint) ILDefines.IL_PALETTE_TYPE);
            int palColumnCount = ilGetInteger((uint) ILDefines.IL_PALETTE_NUM_COLS);
            int bpp = MemoryHelper.GetPaletteComponentCount(type);

            int size = bpp * palColumnCount;

            //Getting a pointer directly to the palette data, so dont need to free
            IntPtr ptr = ilGetPalette();

            return MemoryHelper.ReadByteBuffer(ptr, size);
        }

        /// <summary>
        /// Gets the currently set global data format.
        /// </summary>
        /// <returns>Data format</returns>
        public static DataFormat GetDataFormat() {
            return (DataFormat) ilGetInteger((uint) ILDefines.IL_FORMAT_MODE);
        }

        /// <summary>
        /// Gets the currently set global compressed data format.
        /// </summary>
        /// <returns>Compressed data format</returns>
        public static CompressedDataFormat GetDxtcFormat() {
            return (CompressedDataFormat) ilGetInteger((uint) ILDefines.IL_DXTC_FORMAT);
        }

        /// <summary>
        /// Gets the currently set global data type.
        /// </summary>
        /// <returns>Data type</returns>
        public static DataType GetDataType() {
            return (DataType) ilGetInteger((uint) ILDefines.IL_TYPE_MODE);
        }

        /// <summary>
        /// Gets the currently set jpg save format state.
        /// </summary>
        /// <returns></returns>
        public static JpgSaveFormat GetJpgSaveFormat() {
            return (JpgSaveFormat) ilGetInteger((uint) ILDefines.IL_JPG_SAVE_FORMAT);
        }

        /// <summary>
        /// Gets the currently set global origin location.
        /// </summary>
        /// <returns>Image origin</returns>
        public static OriginLocation GetOriginLocation() {
            return (OriginLocation) ilGetInteger((uint) ILDefines.IL_ORIGIN_MODE);
        }

        /// <summary>
        /// Gets the currently set string value for the state.
        /// </summary>
        /// <param name="mode">String state type</param>
        /// <returns>String value</returns>
        public static String GetString(ILStringMode mode) {
            IntPtr ptr = ilGetString((uint) mode);

            if(ptr != IntPtr.Zero) {
                return Marshal.PtrToStringAnsi(ptr);
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets information about the currently bound image.
        /// </summary>
        /// <returns>Image Info</returns>
        public static ImageInfo GetImageInfo() {
            ImageInfo info = new ImageInfo();
            info.Format = (DataFormat) ilGetInteger(ILDefines.IL_IMAGE_FORMAT);
            info.DxtcFormat = (CompressedDataFormat) ilGetInteger(ILDefines.IL_DXTC_DATA_FORMAT);
            info.DataType = (DataType) ilGetInteger(ILDefines.IL_IMAGE_TYPE);
            info.PaletteType = (PaletteType) ilGetInteger(ILDefines.IL_PALETTE_TYPE);
            info.PaletteBaseType = (DataFormat) ilGetInteger(ILDefines.IL_PALETTE_BASE_TYPE);
            info.CubeFlags = (CubeMapFace) ilGetInteger(ILDefines.IL_IMAGE_CUBEFLAGS);
            info.Origin = (OriginLocation) ilGetInteger(ILDefines.IL_IMAGE_ORIGIN);
            info.Width = ilGetInteger(ILDefines.IL_IMAGE_WIDTH);
            info.Height = ilGetInteger(ILDefines.IL_IMAGE_HEIGHT);
            info.Depth = ilGetInteger(ILDefines.IL_IMAGE_DEPTH);
            info.BitsPerPixel = ilGetInteger(ILDefines.IL_IMAGE_BITS_PER_PIXEL);
            info.BytesPerPixel = ilGetInteger(ILDefines.IL_IMAGE_BYTES_PER_PIXEL);
            info.Channels = ilGetInteger(ILDefines.IL_IMAGE_CHANNELS);
            info.Duration = ilGetInteger(ILDefines.IL_IMAGE_DURATION);
            info.SizeOfData = ilGetInteger(ILDefines.IL_IMAGE_SIZE_OF_DATA);
            info.OffsetX = ilGetInteger(ILDefines.IL_IMAGE_OFFX);
            info.OffsetY = ilGetInteger(ILDefines.IL_IMAGE_OFFY);
            info.PlaneSize = ilGetInteger(ILDefines.IL_IMAGE_PLANESIZE);
            info.FaceCount = ilGetInteger(ILDefines.IL_NUM_FACES) + 1;
            info.ImageCount = ilGetInteger(ILDefines.IL_NUM_IMAGES) + 1;
            info.LayerCount = ilGetInteger(ILDefines.IL_NUM_LAYERS) + 1;
            info.MipMapCount = ilGetInteger(ILDefines.IL_NUM_MIPMAPS) + 1;
            info.PaletteBytesPerPixel = ilGetInteger(ILDefines.IL_PALETTE_BPP);
            info.PaletteColumnCount = ilGetInteger(ILDefines.IL_PALETTE_NUM_COLS);
            return info;
        }

        /// <summary>
        /// Gets the quantization state.
        /// </summary>
        /// <returns>Quantization state</returns>
        public static Quantization GetQuantization() {
            return (Quantization) ilGetInteger((uint) ILDefines.IL_QUANTIZATION_MODE);
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilInvertSurfaceDxtcDataAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool InvertSurfaceDxtcDataAlpha();

        /// <summary>
        /// Initializes the DevIL subsystem. This needs to be called before any other function
        /// is called. The wrapper will filter out subsequent calls until Shutdown() is called.
        /// </summary>
        public static void Initialize() {
            if(!_init) {
                ilInit();
                _init = true;
            }
        }

        /// <summary>
        /// Checks if the enable bit is disabled.
        /// </summary>
        /// <param name="mode">Enable bit</param>
        /// <returns>True if disabled, false otherwise</returns>
        public static bool IsDisabled(ILEnable mode) {
            return ilIsDisabled((uint) mode);
        }

        /// <summary>
        /// Checks if the enable bit is enabled.
        /// </summary>
        /// <param name="mode">Enable bit</param>
        /// <returns>True if enabled, false otherwise</returns>
        public static bool IsEnabled(ILEnable mode) {
            return ilIsEnabled((uint) mode);
        }

        /// <summary>
        /// Converts the currently bound image data to the specified compressed format. The conversion
        /// occurs for each surface in the image (next image, and each image's mip map chain). This is identical to looping over
        /// these surfaces and calling SurfaceToDxtcData(CompressedDataFormat).
        /// </summary>
        /// <param name="format">Compressed format to convert image data to</param>
        /// <returns>True if the operation was successful</returns>
        public static bool ImageToDxtcData(CompressedDataFormat format) {
            return ilImageToDxtcData((uint) format);
        }

        /// <summary>
        /// Checks if the imageID is in fact an image.
        /// </summary>
        /// <param name="imageID">Image ID</param>
        /// <returns>True if an image, false otherwise</returns>
        public static bool IsImage(ImageID imageID) {
            if(imageID.ID < 0)
                return false;

            return ilIsImage((uint) imageID.ID);
        }

        /// <summary>
        /// Checks if the specified file is a valid image of the specified type.
        /// </summary>
        /// <param name="imageType">Image type</param>
        /// <param name="filename">Filename of the image</param>
        /// <returns>True if the file is of the specified image type, false otherwise</returns>
        public static bool IsValid(ImageType imageType, String filename) {
            if(imageType == ImageType.Unknown || String.IsNullOrEmpty(filename))
                return false;

            return ilIsValid((uint) imageType, filename);
        }

        /// <summary>
        /// Checks if the raw data is a valid image of the specified type.
        /// </summary>
        /// <param name="imageType">Image type</param>
        /// <param name="data">Raw data</param>
        /// <returns>True if the raw data is of the specified image type, false otherwise.</returns>
        public static bool IsValid(ImageType imageType, byte[] data) {
            if(imageType == ImageType.Unknown || data == null || data.Length == 0)
                return false;

            unsafe {
                fixed(byte* ptr = data) {
                    return ilIsValidL((uint) imageType, new IntPtr(ptr), (uint) data.Length);
                }
            }
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilLoadImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool LoadImage([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        public static bool LoadImage(ImageType imageType, String filename) {
            return ilLoad((uint) imageType, filename);
        }

        public static bool LoadImageFromStream(ImageType imageType, Stream stream) {
            if(imageType == ImageType.Unknown || stream == null || !stream.CanRead)
                return false;

            byte[] rawData = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint) rawData.Length;
            bool flag = false;
            unsafe {
                fixed(byte* ptr = rawData) {
                    flag = ilLoadL((uint) imageType, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        public static bool LoadImageFromStream(Stream stream) {
            if(stream == null || !stream.CanRead)
                return false;

            byte[] rawData = MemoryHelper.ReadStreamFully(stream, 0);
            uint size = (uint) rawData.Length;
            bool flag = false;
            ImageType imageExtension = DetermineImageType(rawData);
            unsafe {
                fixed(byte* ptr = rawData) {
                    flag = ilLoadL((uint) imageExtension, new IntPtr(ptr), size);
                }
            }

            return flag;
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilLoadPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool LoadPalette([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String fileName);

        /// <summary>
        /// Tries to read raw data of an image that was dumped to a file.
        /// </summary>
        /// <param name="filename">File to laod from</param>
        /// <param name="width">Known image width</param>
        /// <param name="height">Known image height</param>
        /// <param name="depth">Known image depth</param>
        /// <param name="componentCount">Number of components for each pixel (1, 3, or 4)</param>
        /// <returns></returns>
        public static bool LoadRawData(String filename, int width, int height, int depth, int componentCount) {
            if(String.IsNullOrEmpty(filename) || width < 1 || height < 1 || depth < 1)
                return false;

            if(componentCount != 1 || componentCount != 3 || componentCount != 4)
                return false;

            return ilLoadData(filename, (uint) width, (uint) height, (uint) depth, (byte) componentCount);
        }

        public static bool LoadRawData(byte[] data, int width, int height, int depth, int componentCount) {
            if(width < 1 || height < 1 || depth < 1)
                return false;

            if(componentCount != 1 || componentCount != 3 || componentCount != 4)
                return false;

            uint size = (uint) data.Length;

            unsafe {
                fixed(byte* ptr = data) {
                    return ilLoadDataL(new IntPtr(ptr), size, (uint) width, (uint) height, (uint) depth, (byte) componentCount);
                }
            }
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilModAlpha", CallingConvention = CallingConvention.StdCall)]
        public static extern void ModulateAlpha(double alphaValue);

        /// <summary>
        /// Overlays the source image, over the currently bound image at the offsets specified. This basically
        /// performs a blit behind the scenes, so set blit parameters accordingly.
        /// </summary>
        /// <param name="srcImageID">Source image id</param>
        /// <param name="destX">Destination x offset</param>
        /// <param name="destY">Destination y offset</param>
        /// <param name="destZ">Destination z offset</param>
        /// <returns></returns>
        public static bool OverlayImage(ImageID srcImageID, int destX, int destY, int destZ) {
            if(srcImageID.ID < 0) {
                return false;
            }

            return ilOverlayImage((uint) srcImageID.ID, destX, destY, destZ);
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilPopAttrib", CallingConvention = CallingConvention.StdCall)]
        public static extern void PopAttribute();

        public static void PushAttribute(AttributeBits bits) {
            ilPushAttrib((uint) bits);
        }

        [DllImport(ILDLL, EntryPoint = "ilSaveImage", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SaveImage([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String fileName);

        public static bool SaveImage(ImageType type, String filename) {
            return ilSave((uint) type, filename);
        }

        public static bool SaveImageToStream(ImageType imageType, Stream stream) {
            if(imageType == ImageType.Unknown || stream == null || !stream.CanWrite)
                return false;

            uint size = ilSaveL((uint) imageType, IntPtr.Zero, 0);

            if(size == 0)
                return false;

            byte[] buffer = new byte[size];

            unsafe {
                fixed(byte* ptr = buffer) {
                    if(ilSaveL((uint) imageType, new IntPtr(ptr), size) == 0)
                        return false;
                }
            }

            stream.Write(buffer, 0, buffer.Length);

            return true;
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilSaveData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SaveRawData([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String FileName);

        [DllImportAttribute(ILDLL, EntryPoint = "ilSavePal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SavePalette([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        [DllImportAttribute(ILDLL, EntryPoint = "ilSetAlpha", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool SetAlpha(double alphaValue);
      
        public static void SetBoolean(ILBooleanMode mode, bool value) {
            ilSetInteger((uint) mode, (value) ? 1 : 0);
        }

        public static bool SetCompressionAlgorithm(CompressionAlgorithm compressAlgorithm) {
            return ilCompressFunc((uint) compressAlgorithm);
        }

        public static bool SetDataFormat(DataFormat dataFormat) {
            return ilFormatFunc((uint) dataFormat);
        }

        /// <summary>
        /// Uploads the data to replace the currently bounded image's data. Ensure they're the same size before calling.
        /// </summary>
        /// <param name="data">Data to set</param>
        /// <returns>True if the operation was successful or not</returns>
        public static bool SetImageData(byte[] data) {
            unsafe {
                fixed(byte* ptr = data) {
                    return ilSetData(new IntPtr(ptr));
                }
            }
        }

        /// <summary>
        /// Sets the time duration of the currently bounded image should be displayed for (in an animation sequence).
        /// </summary>
        /// <param name="duration">Duration</param>
        /// <returns>True if the operation was successful or not</returns>
        public static bool SetDuration(int duration) {
            if(duration < 0)
                return false;

            return ilSetDuration((uint) duration);
        }

        public static void SetDxtcFormat(CompressedDataFormat format) {
            ilSetInteger((uint) ILDefines.IL_DXTC_FORMAT, (int) format);
        }

        public static bool SetDataType(DataType dataType) {
            return ilTypeFunc((uint) dataType);
        }

        [DllImportAttribute(ILDLL, EntryPoint = "ilKeyColour", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetKeyColor(float red, float green, float blue, float alpha);

        public static void SetKeyColor(Color color) {
            SetKeyColor(color.R, color.G, color.B, color.A);
        }

        public static void SetMemoryHint(MemoryHint hint) {
            ilHint((uint)ILDefines.IL_MEM_SPEED_HINT, (uint) hint);
        }

        public static void SetCompressionHint(CompressionHint hint) {
            ilHint((uint) ILDefines.IL_COMPRESSION_HINT, (uint) hint);
        }

        public static void SetJpgSaveFormat(JpgSaveFormat format) {
            ilSetInteger((uint) ILDefines.IL_JPG_SAVE_FORMAT, (int) format);
        }

        public static void SetInteger(ILIntegerMode mode, int value) {
            ilSetInteger((uint) mode, value);
        }

        public static void SetOriginLocation(OriginLocation origin) {
            ilOriginFunc((uint) origin);
        }

        public static void SetString(ILStringMode mode, String value) {
            if(value == null) {
                value = String.Empty;
            }

            ilSetString((uint) mode, value);
        }

        public static void SetQuantization(Quantization mode) {
            ilSetInteger((uint) ILDefines.IL_QUANTIZATION_MODE, (int) mode);
        }

        public static bool SetPixels(int xOffset, int yOffset, int zOffset, int width, int height, int depth, DataFormat format, DataType dataType, byte[] data) {
            if(data == null || data.Length == 0)
                return false;

            if(xOffset < 0 || yOffset < 0 || zOffset < 0 || width < 1 || height < 1 || depth < 1)
                return false;

            uint size = (uint) data.Length;

            unsafe {
                fixed(byte* ptr = data) {
                    ilSetPixels(xOffset, yOffset, zOffset, (uint) width, (uint) height, (uint) depth, (uint) format, (uint) dataType, new IntPtr(ptr));
                }
            }
            return true;
        }

        /// <summary>
        /// Shuts DevIL's subsystem down, freeing up memory allocated for images. After this call is made, to use the wrapper again you
        /// need to call Initialize().
        /// </summary>
        public static void Shutdown() {
            if(_init) {
                ilShutDown();
                _init = false;
            }
        }

        /// <summary>
        /// Converts the currently bound surface (image, mipmap, etc) to the specified compressed format.
        /// </summary>
        /// <param name="format">Comrpessed format</param>
        /// <returns>True if the operation was successful or not.</returns>
        public static bool SurfaceToDxtcData(CompressedDataFormat format) {
            return ilSurfaceToDxtcData((uint) format);
        }

        /// <summary>
        /// Resets the currently bounded image with the new parameters. This destroys all existing data.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="bytesPerComponent"></param>
        /// <param name="format"></param>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetTexImage(int width, int height, int depth, DataFormat format, DataType dataType, byte[] data) {
            if(data == null || data.Length == 0)
                return false;
            
            byte bpp = (byte) MemoryHelper.GetFormatComponentCount(format);

            unsafe {
                fixed(byte* ptr = data) {
                    return ilTexImage((uint) width, (uint) height, (uint) depth, bpp, (uint) format, (uint) dataType, new IntPtr(ptr));
                }
            }
            
        }

        /// <summary>
        /// Resets the currently bounded image with the new parameters. This destroys all existing data.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="format"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetTexImageDxtc(int width, int height, int depth, CompressedDataFormat format, byte[] data) {
            if(data == null || data.Length == 0)
                return false;
            unsafe {
                fixed(byte* ptr = data) {
                    return ilTexImageDxtc(width, height, depth, (uint) format, new IntPtr(ptr));
                }
            }
        }

        #endregion

        #region Library Info

        public static String GetVendorName() {
            IntPtr value = ilGetString(ILDefines.IL_VENDOR);
            if(value != IntPtr.Zero) {
                return Marshal.PtrToStringAnsi(value);
            }
            return "DevIL";
        }

        public static String GetVersion() {
            IntPtr value = ilGetString(ILDefines.IL_VERSION_NUM);
            if(value != IntPtr.Zero) {
                return Marshal.PtrToStringAnsi(value);
            }
            return "Unknown Version";
        }

        public static String[] GetImportExtensions() {
            IntPtr value = ilGetString(ILDefines.IL_LOAD_EXT);
            if(value != IntPtr.Zero) {
                String ext = Marshal.PtrToStringAnsi(value);
                String[] exts = ext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for(int i = 0; i < exts.Length; i++) {
                    String str = exts[i];
                    //Fix for what looks like a bug: Two entries don't have a space between them, whatmore the dds is
                    //a duplicate anyways
                    if(str.Equals("dcmdds")) {
                        str = str.Substring(0, "dcm".Length);
                    }
                    exts[i] = "." + str;
                }
                return exts;
            }
            return new String[0];
        }

        public static String[] GetExportExtensions() {
            IntPtr value = ilGetString(ILDefines.IL_SAVE_EXT);
            if(value != IntPtr.Zero) {
                String ext = Marshal.PtrToStringAnsi(value);
                String[] exts = ext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                for(int i = 0; i < exts.Length; i++) {
                    exts[i] = "." + exts[i];
                }

                return exts;
            }
            return new String[0];
        }
        #endregion

        #region IL Native Methods

        //Removed ilRegisterFormat to ilResetWrite. Might add the callbacks and reset mem stuff.
        //Also removed all load/saves/etc using file handles. Removed get int/bool versions using pass by ref
        //Removed SetMemory, SetRead, SetWrite, GetLumpPos

        [DllImportAttribute(ILDLL, EntryPoint = "ilActiveFace", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilActiveFace(uint Number);

        [DllImportAttribute(ILDLL, EntryPoint = "ilActiveImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilActiveImage(uint Number);

        [DllImportAttribute(ILDLL, EntryPoint = "ilActiveLayer", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilActiveLayer(uint Number);

        [DllImportAttribute(ILDLL, EntryPoint = "ilActiveMipmap", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilActiveMipmap(uint Number);

        ///InProfile: char*
        ///OutProfile: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilApplyProfile", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilApplyProfile(IntPtr InProfile, IntPtr OutProfile);

        [DllImportAttribute(ILDLL, EntryPoint = "ilBindImage", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilBindImage(uint Image);

        [DllImportAttribute(ILDLL, EntryPoint = "ilBlit", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilBlit(uint Source, int DestX, int DestY, int DestZ, uint SrcX, uint SrcY, uint SrcZ, uint Width, uint Height, uint Depth);

        [DllImportAttribute(ILDLL, EntryPoint = "ilCloneCurImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilCloneCurImage();

        [DllImportAttribute(ILDLL, EntryPoint = "ilCompressDXT", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ilCompressDXT(IntPtr Data, uint Width, uint Height, uint Depth, uint DXTCFormat, ref uint DXTCSize);

        [DllImportAttribute(ILDLL, EntryPoint = "ilCompressFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilCompressFunc(uint Mode);

        [DllImportAttribute(ILDLL, EntryPoint = "ilConvertImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilConvertImage(uint DestFormat, uint DestType);

        [DllImportAttribute(ILDLL, EntryPoint = "ilConvertPal", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilConvertPal(uint DestFormat);

        [DllImportAttribute(ILDLL, EntryPoint = "ilCopyImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilCopyImage(uint Src);

        /// Return Type: sizeOfData
        ///Data: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilCopyPixels", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilCopyPixels(uint XOff, uint YOff, uint ZOff, uint Width, uint Height, uint Depth, uint Format, uint Type, IntPtr Data);

        /// Looks like creates a subimage @ the num index and type is IL_SUB_* (Next, Mip, Layer), etc
        [DllImportAttribute(ILDLL, EntryPoint = "ilCreateSubImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilCreateSubImage(uint Type, uint Num);

        [DllImportAttribute(ILDLL, EntryPoint = "ilDeleteImage", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilDeleteImage(uint Num);

        /// Num is a Size_t
        [DllImportAttribute(ILDLL, EntryPoint = "ilDeleteImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilDeleteImages(UIntPtr Num, uint[] Images);

        /// Return Type: Image Type
        ///FileName: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilDetermineType", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilDetermineType([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        /// Return Type: Image Type
        ///Lump: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilDetermineTypeL", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilDetermineTypeL(IntPtr Lump, uint Size);

        [DllImportAttribute(ILDLL, EntryPoint = "ilDisable", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilDisable(uint Mode);

        [DllImportAttribute(ILDLL, EntryPoint = "ilEnable", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilEnable(uint Mode);

        [DllImportAttribute(ILDLL, EntryPoint = "ilFormatFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilFormatFunc(uint Mode);

        ///Num: ILsizei->size_t->unsigned int
        [DllImportAttribute(ILDLL, EntryPoint = "ilGenImages", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilGenImages(UIntPtr Num, uint[] Images);

        [DllImportAttribute(ILDLL, EntryPoint = "ilGenImage", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilGenImage();

        /// Return Type: ILubyte*
        ///Type: ILenum->unsigned int (Data type)
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetAlpha", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ilGetAlpha(uint Type);

        /// Return Type: ILubyte*
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetData", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ilGetData();

        /// Returns Size of Data, set Zero for BufferSize to get size initially.
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetDXTCData", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilGetDXTCData(IntPtr Buffer, uint BufferSize, uint DXTCFormat);

        /// Return Type: Error type
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetError", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilGetError();

        [DllImportAttribute(ILDLL, EntryPoint = "ilGetInteger", CallingConvention = CallingConvention.StdCall)]
        internal static extern int ilGetInteger(uint Mode);

        /// Return Type: ILubyte*, need to find size via current image's pal size
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetPalette", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ilGetPalette();

        /// Return Type: char*
        ///StringName: ILenum->unsigned int - String type enum
        [DllImportAttribute(ILDLL, EntryPoint = "ilGetString", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr ilGetString(uint StringName);

        ///Target: ILenum->unsigned int --> Type of hint
        ///Mode: ILenum->unsigned int ---> Hint value
        [DllImportAttribute(ILDLL, EntryPoint = "ilHint", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilHint(uint Target, uint Mode);

        [DllImportAttribute(ILDLL, EntryPoint = "ilInit", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilInit();

        /// Format Type
        [DllImportAttribute(ILDLL, EntryPoint = "ilImageToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilImageToDxtcData(uint Format);

        //Enable enum
        [DllImportAttribute(ILDLL, EntryPoint = "ilIsDisabled", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsDisabled(uint Mode);

        //Enable enum
        [DllImportAttribute(ILDLL, EntryPoint = "ilIsEnabled", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsEnabled(uint Mode);

        ///Checks if valid image - input is image id
        [DllImportAttribute(ILDLL, EntryPoint = "ilIsImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsImage(uint Image);

        ///Type: ILenum->unsigned int -- ImageType
        ///FileName: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilIsValid", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsValid(uint Type, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        /// Return Type: ILboolean->unsigned char - Image Type
        ///Lump: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilIsValidL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilIsValidL(uint Type, IntPtr Lump, uint Size);

        /// Type is Image Type
        [DllImportAttribute(ILDLL, EntryPoint = "ilLoad", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilLoad(uint Type, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        /// Type is Image Type
        ///Lump: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilLoadL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilLoadL(uint Type, IntPtr Lump, uint Size);

        /// Mode is origin type
        [DllImportAttribute(ILDLL, EntryPoint = "ilOriginFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilOriginFunc(uint Mode);

        /// SRC image, and coords are the offsets in a blit
        [DllImportAttribute(ILDLL, EntryPoint = "ilOverlayImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilOverlayImage(uint Source, int XCoord, int YCoord, int ZCoord);

        /// Attribute bit flags
        [DllImportAttribute(ILDLL, EntryPoint = "ilPushAttrib", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilPushAttrib(uint Bits);

        /// Image Type
        [DllImportAttribute(ILDLL, EntryPoint = "ilSave", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSave(uint Type, [InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        [DllImportAttribute(ILDLL, EntryPoint = "ilSaveImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSaveImage([InAttribute()] [MarshalAs(UnmanagedType.LPStr)] String FileName);

        ///ImageType, similar deal with GetDXTCData - returns size, pass in a NULL for lump to determine size
        [DllImportAttribute(ILDLL, EntryPoint = "ilSaveL", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilSaveL(uint Type, IntPtr Lump, uint Size);

        ///Data: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilSetData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSetData(IntPtr Data);

        [DllImportAttribute(ILDLL, EntryPoint = "ilSetDuration", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSetDuration(uint Duration);

        /// IntegerMode, and param is value
        [DllImportAttribute(ILDLL, EntryPoint = "ilSetInteger", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilSetInteger(uint Mode, int Param);

        ///Data: void*, dataFormat and DataType
        [DllImportAttribute(ILDLL, EntryPoint = "ilSetPixels", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilSetPixels(int XOff, int YOff, int ZOff, uint Width, uint Height, uint Depth, uint Format, uint Type, IntPtr Data);

        /// Return Type: void
        ///Mode: ILenum->unsigned int
        ///String: char*
        [DllImportAttribute(ILDLL, EntryPoint = "ilSetString", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilSetString(uint Mode, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String String);

        [DllImportAttribute(ILDLL, EntryPoint = "ilShutDown", CallingConvention = CallingConvention.StdCall)]
        private static extern void ilShutDown();

        /// compressed DataFormat
        [DllImportAttribute(ILDLL, EntryPoint = "ilSurfaceToDxtcData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilSurfaceToDxtcData(uint Format);

        /// dataFormat and DataType, destroys current data
        /// Bpp (NumChannels) bytes per pixel - e.g. 3 for RGB
        ///Data: void*
        [DllImportAttribute(ILDLL, EntryPoint = "ilTexImage", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilTexImage(uint Width, uint Height, uint Depth, byte Bpp, uint Format, uint Type, IntPtr Data);

        ///DxtcForamt is CompressedDataFormat, destroys current data
        [DllImportAttribute(ILDLL, EntryPoint = "ilTexImageDxtc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilTexImageDxtc(int Width, int Height, int Depth, uint DxtFormat, IntPtr Data);

        ///Image type from extension of file
        [DllImportAttribute(ILDLL, EntryPoint = "ilTypeFromExt", CallingConvention = CallingConvention.StdCall)]
        private static extern uint ilTypeFromExt([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String FileName);

        ///Sets the current DataType
        [DllImportAttribute(ILDLL, EntryPoint = "ilTypeFunc", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilTypeFunc(uint Mode);

        //Loads raw data from a file, bpp is only valid for 1, 3, 4
        [DllImportAttribute(ILDLL, EntryPoint = "ilLoadData", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilLoadData([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] String FileName, uint Width, uint Height, uint Depth, byte Bpp);

        //Loads raw data from a lump, bpp is only valid for 1, 3, 4
        [DllImportAttribute(ILDLL, EntryPoint = "ilLoadDataL", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ilLoadDataL(IntPtr Lump, uint Size, uint Width, uint Height, uint Depth, byte Bpp);

        #endregion
    }
}
