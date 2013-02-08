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


namespace DevIL.Unmanaged {
    /// <summary>
    /// IL Defines taken directly from the IL header. These are public, but shouldn't be used
    /// since the API will have enum equivalents. This is here as a matter of convienence and to 
    /// keep consistency.
    /// </summary>
    public static class ILDefines {

        public const int IL_TRUE = 1;
        public const int IL_FALSE = 0;
        public const int IL_VENDOR = 0x1F00;
        public const int IL_LOAD_EXT = 0x1F01;
        public const int IL_SAVE_EXT = 0x1F02;

        //Data formats
        public const int IL_COLOR_INDEX = 0x1900;
        public const int IL_ALPHA = 0x1906;
        public const int IL_RGB = 0x1907;
        public const int IL_RGBA = 0x1908;
        public const int IL_BGR = 0x80E0;
        public const int IL_BGRA = 0x80E1;
        public const int IL_LUMINANCE = 0x1909;
        public const int IL_LUMINANCE_ALPHA = 0x190A;

        //Data types
        public const int IL_BYTE = 0x1400;
        public const int IL_UNSIGNED_BYTE = 0x1401;
        public const int IL_SHORT = 0x1402;
        public const int IL_UNSIGNED_SHORT = 0x1403;
        public const int IL_INT = 0x1404;
        public const int IL_UNSIGNED_INT = 0x1405;
        public const int IL_FLOAT = 0x1406;
        public const int IL_DOUBLE = 0x140A;
        public const int IL_HALF = 0x140B;

        //Attribute bits
        public const int IL_ORIGIN_BIT = 0x00000001;
        public const int IL_FILE_BIT = 0x00000002;
        public const int IL_PAL_BIT = 0x00000004;
        public const int IL_FORMAT_BIT = 0x00000008;
        public const int IL_TYPE_BIT = 0x00000010;
        public const int IL_COMPRESS_BIT = 0x00000020;
        public const int IL_LOADFAIL_BIT = 0x00000040;
        public const int IL_FORMAT_SPECIFIC_BIT = 0x00000080;
        public const int IL_ALL_ATTRIB_BITS = 0x000FFFFF;

        //Palette types
        public const int IL_PAL_NONE = 0x0400;
        public const int IL_PAL_RGB24 = 0x0401;
        public const int IL_PAL_RGB32 = 0x0402;
        public const int IL_PAL_RGBA32 = 0x0403;
        public const int IL_PAL_BGR24 = 0x0404;
        public const int IL_PAL_BGR32 = 0x0405;
        public const int IL_PAL_BGRA32 = 0x0406;

        //Image types
        public const int IL_TYPE_UNKNOWN = 0x0000;
        public const int IL_BMP = 0x0420;
        public const int IL_CUT = 0x0421;
        public const int IL_DOOM = 0x0422;
        public const int IL_DOOM_FLAT = 0x0423;
        public const int IL_ICO = 0x0424;
        public const int IL_JPG = 0x0425;
        public const int IL_JFIF = 0x0425;
        public const int IL_ILBM = 0x0426;
        public const int IL_PCD = 0x0427;
        public const int IL_PCX = 0x0428;
        public const int IL_PIC = 0x0429;
        public const int IL_PNG = 0x042A;
        public const int IL_PNM = 0x042B;
        public const int IL_SGI = 0x042C;
        public const int IL_TGA = 0x042D;
        public const int IL_TIF = 0x042E;
        public const int IL_CHEAD = 0x042F;
        public const int IL_RAW = 0x0430;
        public const int IL_MDL = 0x0431;
        public const int IL_WAL = 0x0432;
        public const int IL_LIF = 0x0434;
        public const int IL_MNG = 0x0435;
        public const int IL_JNG = 0x0435;
        public const int IL_GIF = 0x0436;
        public const int IL_DDS = 0x0437;
        public const int IL_DCX = 0x0438;
        public const int IL_PSD = 0x0439;
        public const int IL_EXIF = 0x043A;
        public const int IL_PSP = 0x043B;
        public const int IL_PIX = 0x043C;
        public const int IL_PXR = 0x043D;
        public const int IL_XPM = 0x043E;
        public const int IL_HDR = 0x043F;
        public const int IL_ICNS = 0x0440;
        public const int IL_JP2 = 0x0441;
        public const int IL_EXR = 0x0442;
        public const int IL_WDP = 0x0443;
        public const int IL_VTF = 0x0444;
        public const int IL_WBMP = 0x0445;
        public const int IL_SUN = 0x0446;
        public const int IL_IFF = 0x0447;
        public const int IL_TPL = 0x0448;
        public const int IL_FITS = 0x0449;
        public const int IL_DICOM = 0x044A;
        public const int IL_TWI = 0x044B;
        public const int IL_BLP = 0x044C;
        public const int IL_FTX = 0x044D;
        public const int IL_ROT = 0x044E;
        public const int IL_TEXTURE = 0x044F;
        public const int IL_DPX = 0x0450;
        public const int IL_UTX = 0x0451;
        public const int IL_MP3 = 0x0452;
        public const int IL_JASC_PAL = 0x0475;

        //Error types
        public const int IL_NO_ERROR = 0x0000;
        public const int IL_INVALID_ENUM = 0x0501;
        public const int IL_OUT_OF_MEMORY = 0x0502;
        public const int IL_FORMAT_NOT_SUPPORTED = 0x0503;
        public const int IL_INTERNAL_ERROR = 0x0504;
        public const int IL_INVALID_VALUE = 0x0505;
        public const int IL_ILLEGAL_OPERATION = 0x0506;
        public const int IL_ILLEGAL_FILE_VALUE = 0x0507;
        public const int IL_INVALID_FILE_HEADER = 0x0508;
        public const int IL_INVALID_PARAM = 0x0509;
        public const int IL_COULD_NOT_OPEN_FILE = 0x050A;
        public const int IL_INVALID_EXTENSION = 0x050B;
        public const int IL_FILE_ALREADY_EXISTS = 0x050C;
        public const int IL_OUT_FORMAT_SAME = 0x050D;
        public const int IL_STACK_OVERFLOW = 0x050E;
        public const int IL_STACK_UNDERFLOW = 0x050F;
        public const int IL_INVALID_CONVERSION = 0x0510;
        public const int IL_BAD_DIMENSIONS = 0x0511;
        public const int IL_FILE_READ_ERROR = 0x0512;
        public const int IL_FILE_WRITE_ERROR = 0x0512;
        public const int IL_LIB_GIF_ERROR = 0x05E1;
        public const int IL_LIB_JPEG_ERROR = 0x05E2;
        public const int IL_LIB_PNG_ERROR = 0x05E3;
        public const int IL_LIB_TIFF_ERROR = 0x05E4;
        public const int IL_LIB_MNG_ERROR = 0x05E5;
        public const int IL_LIB_JP2_ERROR = 0x05E6;
        public const int IL_LIB_EXR_ERROR = 0x05E7;
        public const int IL_UKNOWN_ERROR = 0x05FF;

        //Origin definitions
        public const int IL_ORIGIN_SET = 0x0600;
        public const int IL_ORIGIN_LOWER_LEFT = 0x0601;
        public const int IL_ORIGIN_UPPER_LEFT = 0x0602;
        public const int IL_ORIGIN_MODE = 0x0603;

        //Format and type mode definitions
        public const int IL_FORMAT_SET = 0x0610;
        public const int IL_FORMAT_MODE = 0x0611;
        public const int IL_TYPE_SET = 0x0612;
        public const int IL_TYPE_MODE = 0x0613;

        //File definitions
        public const int IL_FILE_OVERWRITE = 0x0620;
        public const int IL_FILE_MODE = 0x0621;

        //Palette definitions
        public const int IL_CONV_PAL = 0x0630;

        //load fail definitions
        public const int IL_DEFAULT_ON_FAIL = 0x0632;

        //Key color and alpha definitions
        public const int IL_USE_KEY_COLOR = 0x0635;
        public const int IL_BLIT_BLEND = 0x0636;

        //Interlace definitions
        public const int IL_SAVE_INTERLACED = 0x0639;
        public const int IL_INTERLACE_MODE = 0x063A;

        //Quantization definitions
        public const int IL_QUANTIZATION_MODE = 0x0640;
        public const int IL_WU_QUANT = 0x0641;
        public const int IL_NEU_QUANT = 0x0642;
        public const int IL_NEU_QUANT_SAMPLE = 0x0643;
        public const int IL_MAX_QUANT_INDICES = 0x0644;

        //Hints
        public const int IL_FASTEST = 0x0660;
        public const int IL_LESS_MEM = 0x0661;
        public const int IL_DONT_CARE = 0x0662;
        public const int IL_MEM_SPEED_HINT = 0x0665;
        public const int IL_USE_COMPRESSION = 0x0666;
        public const int IL_NO_COMPRESSION = 0x0667;
        public const int IL_COMPRESSION_HINT = 0x0668;

        //Compression
        public const int IL_NVIDIA_COMPRESS = 0x0670;
        public const int IL_SQUISH_COMPRESS = 0x0671;

        //Subimage types
        public const int IL_SUB_NEXT = 0x0680;
        public const int IL_SUB_MIPMAP = 0x0681;
        public const int IL_SUB_LAYER = 0x0682;

        //Compression definitions
        public const int IL_COMPRESS_MODE = 0x0700;
        public const int IL_COMPRESS_NONE = 0x0701;
        public const int IL_COMPRESS_RLE = 0x0702;
        public const int IL_COMPRESS_LZO = 0x0703;
        public const int IL_COMPRESS_ZLIB = 0x0704;

        //File format-specific values
        public const int IL_TGA_CREATE_STAMP = 0x0710;
        public const int IL_JPG_QUALITY = 0x0711;
        public const int IL_PNG_INTERLACE = 0x0712;
        public const int IL_TGA_RLE = 0x0713;
        public const int IL_BMP_RLE = 0x0714;
        public const int IL_SGI_RLE = 0x0715;
        public const int IL_TGA_ID_STRING = 0x0717;
        public const int IL_TGA_AUTHNAME_STRING = 0x0718;
        public const int IL_TGA_AUTHCOMMENT_STRING = 0x0719;
        public const int IL_PNG_AUTHNAME_STRING = 0x071A;
        public const int IL_PNG_TITLE_STRING = 0x071B;
        public const int IL_PNG_DESCRIPTION_STRING = 0x071C;
        public const int IL_TIF_DESCRIPTION_STRING = 0x071D;
        public const int IL_TIF_HOSTCOMPUTER_STRING = 0x071E;
        public const int IL_TIF_DOCUMENTNAME_STRING = 0x071F;
        public const int IL_TIF_AUTHNAME_STRING = 0x0720;
        public const int IL_JPG_SAVE_FORMAT = 0x0721;
        public const int IL_CHEAD_HEADER_STRING = 0x0722;
        public const int IL_PCD_PICNUM = 0x0723;
        //The color in the palette at this index value (0-255) is considered transparent, -1 for no trasparent color
        public const int IL_PNG_ALPHA_INDEX = 0x0724;
        public const int IL_JPG_PROGRESSIVE = 0x0725;
        public const int IL_VTF_COMP = 0x0726;

        //DXTC definitions
        public const int IL_DXTC_FORMAT = 0x0705;
        public const int IL_DXT1 = 0x0706;
        public const int IL_DXT2 = 0x0707;
        public const int IL_DXT3 = 0x0708;
        public const int IL_DXT4 = 0x0709;
        public const int IL_DXT5 = 0x070A;
        public const int IL_DXT_NO_COMP = 0x070B;
        public const int IL_KEEP_DXTC_DATA = 0x070C;
        public const int IL_DXTC_DATA_FORMAT = 0x070D;
        public const int IL_3DC = 0x070E;
        public const int IL_RXGB = 0x070F;
        public const int IL_ATI1N = 0x0710;
        //Normally the same as IL_DXT1, except for nVidia Texture Tools.
        public const int IL_DXT1A = 0x0711;

        //Environment map definitions
        public const int IL_CUBEMAP_POSITIVEX = 0x00000400;
        public const int IL_CUBEMAP_NEGATIVEX = 0x00000800;
        public const int IL_CUBEMAP_POSITIVEY = 0x00001000;
        public const int IL_CUBEMAP_NEGATIVEY = 0x00002000;
        public const int IL_CUBEMAP_POSITIVEZ = 0x00004000;
        public const int IL_CUBEMAP_NEGATIVEZ = 0x00008000;
        public const int IL_SPHEREMAP = 0x00010000;


        //Values
        public const int IL_VERSION_NUM = 0x0DE2;
        public const int IL_IMAGE_WIDTH = 0x0DE4;
        public const int IL_IMAGE_HEIGHT = 0x0DE5;
        public const int IL_IMAGE_DEPTH = 0x0DE6;
        public const int IL_IMAGE_SIZE_OF_DATA = 0x0DE7;
        public const int IL_IMAGE_BYTES_PER_PIXEL = 0x0DE8;
        public const int IL_IMAGE_BITS_PER_PIXEL = 0x0DE9;
        public const int IL_IMAGE_FORMAT = 0x0DEA;
        public const int IL_IMAGE_TYPE = 0x0DEB;
        public const int IL_PALETTE_TYPE = 0x0DEC;
        public const int IL_PALETTE_SIZE = 0x0DED;
        public const int IL_PALETTE_BPP = 0x0DEE;
        public const int IL_PALETTE_NUM_COLS = 0x0DEF;
        public const int IL_PALETTE_BASE_TYPE = 0x0DF0;
        public const int IL_NUM_FACES = 0x0DE1;
        public const int IL_NUM_IMAGES = 0x0DF1;
        public const int IL_NUM_MIPMAPS = 0x0DF2;
        public const int IL_NUM_LAYERS = 0x0DF3;
        public const int IL_ACTIVE_IMAGE = 0x0DF4;
        public const int IL_ACTIVE_MIPMAP = 0x0DF5;
        public const int IL_ACTIVE_LAYER = 0x0DF6;
        public const int IL_ACTIVE_FACE = 0x0E00;
        public const int IL_CUR_IMAGE = 0x0DF7;
        public const int IL_IMAGE_DURATION = 0x0DF8;
        public const int IL_IMAGE_PLANESIZE = 0x0DF9;
        public const int IL_IMAGE_BPC = 0x0DFA;
        public const int IL_IMAGE_OFFX = 0x0DFB;
        public const int IL_IMAGE_OFFY = 0x0DFC;
        public const int IL_IMAGE_CUBEFLAGS = 0x0DFD;
        public const int IL_IMAGE_ORIGIN = 0x0DFE;
        public const int IL_IMAGE_CHANNELS = 0x0DFF;
    }
}
