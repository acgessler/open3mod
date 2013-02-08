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

namespace DevIL.Unmanaged {

    /// <summary>
    /// Enumerates error types.
    /// </summary>
    public enum ErrorType {

        /// <summary>
        /// Everything's A-OK, no error reported.
        /// </summary>
        NoError = 0x0000,

        /// <summary>
        /// Invalid enum.
        /// </summary>
        InvalidEnum = 0x0501,

        /// <summary>
        /// DevIL ran out of memory.
        /// </summary>
        OutOfMemory = 0x0502,

        /// <summary>
        /// Format is not supported.
        /// </summary>
        FormatNotSupported = 0x0503,

        /// <summary>
        /// There was an internal error.
        /// </summary>
        InternalError = 0x0504,

        /// <summary>
        /// Value was invalid.
        /// </summary>
        InvalidValue = 0x0505,

        /// <summary>
        /// Operation was illegal.
        /// </summary>
        IllegalOperation = 0x0506,

        /// <summary>
        /// File value not valid.
        /// </summary>
        IllegalFileValue = 0x0507,

        /// <summary>
        /// File header not valid.
        /// </summary>
        IllegalFileHeader = 0x0508,

        /// <summary>
        /// Invalid parameter.
        /// </summary>
        InvalidParameter = 0x0509,

        /// <summary>
        /// Could not open the file.
        /// </summary>
        CouldNotOpenFile = 0x050A,

        /// <summary>
        /// Extension not supported.
        /// </summary>
        InvalidExtension = 0x050B,

        /// <summary>
        /// File already exists.
        /// </summary>
        FileAlreadyExists = 0x050C,

        /// <summary>
        /// Output format is the same as input.
        /// </summary>
        OutFormatSame = 0x050D,

        /// <summary>
        /// Stack overflow error.
        /// </summary>
        StackOverflow = 0x050E,

        /// <summary>
        /// Stack underflow error.
        /// </summary>
        StackUnderflow = 0x050F,

        /// <summary>
        /// There was an error in conversion.
        /// </summary>
        InvalidConversion = 0x0510,

        /// <summary>
        /// Error with image dimensions.
        /// </summary>
        BadDimensions = 0x0511,

        /// <summary>
        /// Error in reading file.
        /// </summary>
        FileReadError = 0x0512,

        /// <summary>
        /// Error in writing file.
        /// </summary>
        FileWriteError = 0x0512,

        /// <summary>
        /// GIF format specific error.
        /// </summary>
        Gif_Error = 0x05E1,

        /// <summary>
        /// JPG format specific error.
        /// </summary>
        Jpeg_Error = 0x05E2,

        /// <summary>
        /// PNG format specific error.
        /// </summary>
        Png_Error = 0x05E3,

        /// <summary>
        /// TIFF format specific error.
        /// </summary>
        Tiff_Error = 0x05E4,

        /// <summary>
        /// MNG format specific error.
        /// </summary>
        Mng_Error = 0x05E5,

        /// <summary>
        /// JP2 format specific error.
        /// </summary>
        Jp2_Error = 0x05E6,

        /// <summary>
        /// EXR format specific error.
        /// </summary>
        Exr_Error = 0x05E7,

        /// <summary>
        /// Unknown error.
        /// </summary>
        UnknownError = 0x05FF
    }


    /// <summary>
    /// Enumerates attribute bit flags.
    /// </summary>
    [Flags]
    public enum AttributeBits {
        Origin = ILDefines.IL_ORIGIN_BIT,
        File = ILDefines.IL_FILE_BIT,
        Palette = ILDefines.IL_PAL_BIT,
        Format = ILDefines.IL_FORMAT_BIT,
        Type = ILDefines.IL_TYPE_BIT,
        Compress = ILDefines.IL_COMPRESS_BIT,
        LoadFail = ILDefines.IL_LOADFAIL_BIT,
        FormatSpecific = ILDefines.IL_FORMAT_SPECIFIC_BIT,
        All = ILDefines.IL_ALL_ATTRIB_BITS
    }


    /// <summary>
    /// Enumerates possible enable caps for DevIL's Enable/Disable function to enable/disable
    /// certain behaviors by the library.
    /// </summary>
    public enum ILEnable {
        /// <summary>
        /// If enabled, DevIL will convert (if need be) all images to the same data format (BGR, RGB, etc) that is specified by the user.
        /// </summary>
        AbsoluteFormat = ILDefines.IL_FORMAT_SET,

        /// <summary>
        /// If enabled, DevIL will convert (if need be) all images to the same data type (byte, float, etc) that is specified by the user.
        /// </summary>
        AbsoluteType = ILDefines.IL_TYPE_SET,

        /// <summary>
        /// If enabled, DevIL will use an absolute origin for all images.
        /// </summary>
        AbsoluteOrigin = ILDefines.IL_ORIGIN_SET,

        /// <summary>
        /// If enabled, DevIL will overwrite existing files when an image is saved.
        /// </summary>
        OverwriteExistingFile = ILDefines.IL_FILE_OVERWRITE,

        /// <summary>
        /// If enabled, DevIL will convert palettes to their base type (e.g. BGR palette to data format BGR).
        /// </summary>
        ConvertPalette = ILDefines.IL_CONV_PAL,

        /// <summary>
        /// If enabled, DevIL will use the nVidia Texture Tools (NVTT) library compression instead
        /// of the library default. If this is enabled with squish, NVTT takes higher priority and will be used
        /// instead.
        /// </summary>
        NvidiaCompression = ILDefines.IL_NVIDIA_COMPRESS,

        /// <summary>
        /// If enabled, DevIL will use the Squish library compression. Squish is slower than NVTT
        /// but generates images with the highest quality possible. If this is set with NvidiaCompression,
        /// the NVTT library is used instead. Note: Doesn't seem to be used anymore?
        /// </summary>
        SquishCompression = ILDefines.IL_SQUISH_COMPRESS,

        /// <summary>
        /// If enabled, when loading an image fails, DevIL will load the default image instead.
        /// </summary>
        DefaultOnFail = ILDefines.IL_DEFAULT_ON_FAIL,

        /// <summary>
        /// If enabled, DevIL will use a color key.
        /// </summary>
        UseColorKey = ILDefines.IL_USE_KEY_COLOR,

        /// <summary>
        /// If enabled, DevIL will blend the source image with the destination
        /// image if the source has an alpha channel present.
        /// </summary>
        BlitBlend = ILDefines.IL_BLIT_BLEND,

        /// <summary>
        /// If enabled, DevIL will do interlaced saving for PNG images.
        /// </summary>
        SaveInterlaced = ILDefines.IL_SAVE_INTERLACED,

        /// <summary>
        /// If enabled, DevIL will do "progressive" saving of JPG images.
        /// </summary>
        JpgProgressive = ILDefines.IL_JPG_PROGRESSIVE
    }

    public enum ILIntegerMode {
        CurrentImage = ILDefines.IL_CUR_IMAGE,
        MaxQuantizationIndices = ILDefines.IL_MAX_QUANT_INDICES,
        NeuQuantizationSample = ILDefines.IL_NEU_QUANT_SAMPLE,
        VersionNumber = ILDefines.IL_VERSION_NUM,
        VTFCompression = ILDefines.IL_VTF_COMP,
        JpgQuality = ILDefines.IL_JPG_QUALITY,
        PcdPicNumber = ILDefines.IL_PCD_PICNUM,
        PngAlphaIndex = ILDefines.IL_PNG_ALPHA_INDEX,
        ActiveImage = ILDefines.IL_ACTIVE_IMAGE,
        ActiveMipMap = ILDefines.IL_ACTIVE_MIPMAP,
        ActiveLayer = ILDefines.IL_ACTIVE_LAYER,

        ImageDuration = ILDefines.IL_IMAGE_DURATION,
        ImageOffsetX = ILDefines.IL_IMAGE_OFFX,
        ImageOffsetY = ILDefines.IL_IMAGE_OFFY,

        ImageWidth = ILDefines.IL_IMAGE_WIDTH,
        ImageHeight = ILDefines.IL_IMAGE_HEIGHT,
        ImageDepth = ILDefines.IL_IMAGE_DEPTH,
        ImageBitsPerPixel = ILDefines.IL_IMAGE_BITS_PER_PIXEL,
        ImageBytesPerPixel = ILDefines.IL_IMAGE_BYTES_PER_PIXEL,
        ImageChannels = ILDefines.IL_IMAGE_CHANNELS,
        ImageSizeOfData = ILDefines.IL_IMAGE_SIZE_OF_DATA,
        ImagePlaneSize = ILDefines.IL_IMAGE_PLANESIZE,
        ImageFaceCount = ILDefines.IL_NUM_FACES,
        ImageArrayCount = ILDefines.IL_NUM_IMAGES,
        ImageLayerCount = ILDefines.IL_NUM_LAYERS,
        ImageMipMapCount = ILDefines.IL_NUM_MIPMAPS,
        ImagePaletteBytesPerPixel = ILDefines.IL_PALETTE_BPP,
        ImagePaletteColumnCount = ILDefines.IL_PALETTE_NUM_COLS
    }

    public enum ILBooleanMode {
        KeepDxtcData = ILDefines.IL_KEEP_DXTC_DATA,
        BmpRLE = ILDefines.IL_BMP_RLE,
        PngInterlace = ILDefines.IL_PNG_INTERLACE,
        SgiRLE = ILDefines.IL_SGI_RLE,
        TgaCreateStamp = ILDefines.IL_TGA_CREATE_STAMP,
        TgaRLE = ILDefines.IL_TGA_RLE
    }

    public enum ILStringMode {
        TgaID = ILDefines.IL_TGA_ID_STRING,
        TgaAuthorName = ILDefines.IL_TGA_AUTHNAME_STRING,
        TgaAuthorComment = ILDefines.IL_TGA_AUTHCOMMENT_STRING,
        PngAuthorName = ILDefines.IL_PNG_AUTHNAME_STRING,
        PngTitle = ILDefines.IL_PNG_TITLE_STRING,
        PngDescription = ILDefines.IL_PNG_DESCRIPTION_STRING,
        TifDescription = ILDefines.IL_TIF_DESCRIPTION_STRING,
        TifHostComputer = ILDefines.IL_TIF_HOSTCOMPUTER_STRING,
        TifDocumentName = ILDefines.IL_TIF_DOCUMENTNAME_STRING,
        TifAuthorName = ILDefines.IL_TIF_AUTHNAME_STRING,
        CHeadHeader = ILDefines.IL_CHEAD_HEADER_STRING
    }

    /// <summary>
    /// Enumerates sub image types.
    /// </summary>
    public enum SubImageType {
        /// <summary>
        /// Denotes a "next" image in the chain, e.g. animation or face.
        /// </summary>
        Image = (int) ILDefines.IL_SUB_NEXT,

        /// <summary>
        /// Denotes a mipmap.
        /// </summary>
        MipMap = (int) ILDefines.IL_SUB_MIPMAP,

        /// <summary>
        /// Denotes a layer.
        /// </summary>
        Layer = (int) ILDefines.IL_SUB_LAYER
    }
}
