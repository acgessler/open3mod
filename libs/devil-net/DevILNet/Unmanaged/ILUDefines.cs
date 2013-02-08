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
    public static class ILUDefines {
        public const int ILU_FILTER = 0x2600;
        public const int ILU_NEAREST = 0x2601;
        public const int ILU_LINEAR = 0x2602;
        public const int ILU_BILINEAR = 0x2603;
        public const int ILU_SCALE_BOX = 0x2604;
        public const int ILU_SCALE_TRIANGLE = 0x2605;
        public const int ILU_SCALE_BELL = 0x2606;
        public const int ILU_SCALE_BSPLINE = 0x2607;
        public const int ILU_SCALE_LANCZOS3 = 0x2608;
        public const int ILU_SCALE_MITCHELL = 0x2609;

        public const int ILU_INVALID_ENUM = 0x0501;
        public const int ILU_OUT_OF_MEMORY = 0x0502;
        public const int ILU_INTERNAL_ERROR = 0x0504;
        public const int ILU_INVALID_VALUE = 0x0505;
        public const int ILU_ILLEGAL_OPERATION = 0x0506;
        public const int ILU_INVALID_PARAM = 0x0509;

        public const int ILU_PLACEMENT = 0x0700;
        public const int ILU_LOWER_LEFT = 0x0701;
        public const int ILU_LOWER_RIGHT = 0x0702;
        public const int ILU_UPPER_LEFT = 0x0703;
        public const int ILU_UPPER_RIGHT = 0x0704;
        public const int ILU_CENTER = 0x0705;
        public const int ILU_CONVOLUTION_MATRIX = 0x0710;
  
        public const int ILU_ENGLISH = 0x0800;
        public const int ILU_ARABIC = 0x0801;
        public const int ILU_DUTCH = 0x0802;
        public const int ILU_JAPANESE = 0x0803;
        public const int ILU_SPANISH = 0x0804;
        public const int ILU_GERMAN = 0x0805;
        public const int ILU_FRENCH = 0x0806;
    }
}
