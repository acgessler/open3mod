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

namespace DevIL {
    /// <summary>
    /// Helper static class containing functions that aid dealing with unmanaged memory to managed memory conversions.
    /// </summary>
    public static class MemoryHelper {

        /// <summary>
        /// Reads a byte buffer from unmanaged memory.
        /// </summary>
        /// <param name="pointer">Pointer to unmanaged memory</param>
        /// <param name="numBytes">Number of bytes to read</param>
        /// <returns>Byte buffer, or null if the pointer was no valid</returns>
        public static byte[] ReadByteBuffer(IntPtr pointer, int numBytes) {
            if(pointer == IntPtr.Zero)
                return null;

            byte[] bytes = new byte[numBytes];
            Marshal.Copy(pointer, bytes, 0, numBytes);
            return bytes;
        }

        /// <summary>
        /// Marshals a c-style pointer array to a managed array of structs. This will read
        /// from the start of the IntPtr provided and care should be taken in ensuring that the number
        /// of elements to read is correct.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pointer">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <returns>Managed array, or null if the pointer was not valid</returns>
        public static T[] MarshalArray<T>(IntPtr pointer, int length) where T : struct {
            return MarshalArray<T>(pointer, length, false);
        }

        /// <summary>
        /// Marshals a c-style pointer array to a manged array of structs. Takes in a parameter denoting if the
        /// pointer is a "pointer to a pointer" (void**) which requires some extra care. This will read from the start of
        /// the IntPtr and care should be taken in esnuring that the number of elements to read is correct.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pointer">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <param name="pointerToPointer">True if the unmanaged pointer is void** or not.</param>
        /// <returns>Managed array, or null if the pointer was not valid</returns>
        public static T[] MarshalArray<T>(IntPtr pointer, int length, bool pointerToPointer) where T : struct {
            if(pointer == IntPtr.Zero) {
                return null;
            }

            try {
                Type type = typeof(T);
                //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
                int stride = (pointerToPointer) ? IntPtr.Size : Marshal.SizeOf(typeof(T));
                T[] array = new T[length];

                for(int i = 0; i < length; i++) {
                    IntPtr currPos = new IntPtr(pointer.ToInt64() + (stride * i));
                    //If pointer is a void**, read the current position to get the proper pointer
                    if(pointerToPointer) {
                        currPos = Marshal.ReadIntPtr(currPos);
                    }
                    array[i] = (T) Marshal.PtrToStructure(currPos, type);
                }
                return array;
            } catch(Exception) {
                return null;
            }
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <returns>Marshaled structure</returns>
        public static T MarshalStructure<T>(IntPtr ptr) where T : struct {
            if(ptr == IntPtr.Zero) {
                return default(T);
            }
            return (T) Marshal.PtrToStructure(ptr, typeof(T));
        }

        /// <summary>
        /// Clamps a value within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static float Clamp(float value, float min, float max) {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Clamps a value within the specified range.
        /// </summary>
        /// <param name="value">Source value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Clamped value</returns>
        public static double Clamp(double value, double min, double max) {
            value = (value > max) ? max : value;
            value = (value < min) ? min : value;
            return value;
        }

        /// <summary>
        /// Rounds the integer up to the nearest power of two.
        /// </summary>
        /// <param name="value">Value to round up</param>
        /// <returns>Rounded up to the nearest power of two</returns>
        public static int RoundUpToPowerOfTwo(int value) {
            value = value - 1;
            value = value | (value >> 1);
            value = value | (value >> 2);
            value = value | (value >> 4);
            value = value | (value >> 8);
            value = value | (value >> 16);
            return value + 1;
        }

        /// <summary>
        /// Rounds the integer down to the nearest power of two.
        /// </summary>
        /// <param name="value">Value to round down</param>
        /// <returns>Rounded down to the nearest power of two</returns>
        public static int RoundDownToPowerOfTwo(int value) {
            value = value | (value >> 1);
            value = value | (value >> 2);
            value = value | (value >> 4);
            value = value | (value >> 8);
            value = value | (value >> 16);
            return value - (value >> 1);
        }

        /// <summary>
        /// Rounds the integer to the nearest power of two, this may round up or
        /// down, based on whichever is the closer value. If equidistant,
        /// rounding up is preferred.
        /// </summary>
        /// <param name="value">Value to be rounded to the nearest power of two</param>
        /// <returns>Neaerst power of two</returns>
        public static int RoundToNearestPowerOfTwo(int value) {
            int up = RoundUpToPowerOfTwo(value);
            int down = RoundDownToPowerOfTwo(value);
            int upDiff = Math.Abs(up - value);
            int downDiff = Math.Abs(value - down);

            //In the advent of a tie, prefer to round up
            if(downDiff < upDiff) {
                return down;
            }
            return up;
        }

        /// <summary>
        /// Gets the number of components for the format, e.g. RGB is three components. Multiply this
        /// by the size of each component to get the "BPP - bytes per pixel".
        /// </summary>
        /// <param name="format">DataFormat</param>
        /// <returns>Number of components.</returns>
        public static int GetFormatComponentCount(DataFormat format) {
            switch(format) {
                case DataFormat.ColorIndex:
                case DataFormat.Luminance:
                case DataFormat.Alpha:
                    return 1;
                case DataFormat.LuminanceAlpha:
                    return 2;
                case DataFormat.RGB:
                case DataFormat.BGR:
                    return 3;
                case DataFormat.RGBA:
                case DataFormat.BGRA:
                    return 4;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the palette's component/channel count.
        /// </summary>
        /// <param name="palette">Palette</param>
        /// <returns>Number of components/channels in the palette</returns>
        public static int GetPaletteComponentCount(PaletteType palette) {
            switch(palette) {
                case PaletteType.RGB24:
                case PaletteType.BGR24:
                    return 3;
                case PaletteType.RGB32:
                case PaletteType.RGBA32:
                case PaletteType.BGR32:
                case PaletteType.BGRA32:
                    return 4;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the palette's base format.
        /// </summary>
        /// <param name="palette">Palette</param>
        /// <returns>Base format</returns>
        public static DataFormat GetPaletteBaseFormat(PaletteType palette) {
            switch(palette) {
                case PaletteType.RGB24:
                    return DataFormat.RGB;
                case PaletteType.RGB32:
                    return DataFormat.RGBA; //DevIL says its not sure
                case PaletteType.RGBA32:
                    return DataFormat.RGBA;
                case PaletteType.BGR24:
                    return DataFormat.BGR;
                case PaletteType.BGR32:
                    return DataFormat.BGRA; //DevIL says its not sure
                case PaletteType.BGRA32:
                    return DataFormat.BGRA;
                default:
                    return DataFormat.RGBA;
            }
        }

        /// <summary>
        /// Gets the size of the data type. This will be the "bytes per component or channel" or BPC.
        /// </summary>
        /// <param name="dataType">DataType</param>
        /// <returns>Bytes per component/channel</returns>
        public static int GetDataTypeSize(DataType dataType) {
            switch(dataType) {
                case DataType.Byte:
                case DataType.UnsignedByte:
                    return 1;
                case DataType.Short:
                case DataType.UnsignedShort:
                case DataType.Half:
                    return 2;
                case DataType.Int:
                case DataType.UnsignedInt:
                case DataType.Float:
                    return 4;
                case DataType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculates the bytes per pixel for the specified format/data type.
        /// </summary>
        /// <param name="format">DataFormat</param>
        /// <param name="dataType">DataType</param>
        /// <returns>bytes per pixel</returns>
        public static int GetBpp(DataFormat format, DataType dataType) {
            return GetDataTypeSize(dataType) * GetFormatComponentCount(format);
        }

        /// <summary>
        /// Calculates the total (uncompressed) size of the data specified by these dimensions and data format/type.
        /// </summary>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <param name="depth">Depth of the image</param>
        /// <param name="format">DataFormat</param>
        /// <param name="dataType">DataType</param>
        /// <returns>Total size of the image, in bytes</returns>
        public static int GetDataSize(int width, int height, int depth, DataFormat format, DataType dataType) {
            if(width <= 0) {
                width = 1;
            }
            if(height <= 0) {
                height = 1;
            }
            if(depth <= 0) {
                depth = 1;
            }
            return width * height * depth * GetBpp(format, dataType);
        }

        /// <summary>
        /// Reads a stream until the end is reached into a byte array. Based on
        /// <a href="http://www.yoda.arachsys.com/csharp/readbinary.html">Jon Skeet's implementation</a>.
        /// It is up to the caller to dispose of the stream.
        /// </summary>
        /// <param name="stream">Stream to read all bytes from</param>
        /// <param name="initialLength">Initial buffer length, default is 32K</param>
        /// <returns>The byte array containing all the bytes from the stream</returns>
        public static byte[] ReadStreamFully(Stream stream, int initialLength) {
            if(initialLength < 1) {
                initialLength = 32768; //Init to 32K if not a valid initial length
            }

            byte[] buffer = new byte[initialLength];
            int position = 0;
            int chunk;

            while((chunk = stream.Read(buffer, position, buffer.Length - position)) > 0) {
                position += chunk;

                //If we reached the end of the buffer check to see if there's more info
                if(position == buffer.Length) {
                    int nextByte = stream.ReadByte();

                    //If -1 we reached the end of the stream
                    if(nextByte == -1) {
                        return buffer;
                    }

                    //Not at the end, need to resize the buffer
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[position] = (byte) nextByte;
                    buffer = newBuffer;
                    position++;
                }
            }

            //Trim the buffer before returning
            byte[] toReturn = new byte[position];
            Array.Copy(buffer, toReturn, position);
            return toReturn;
        }
    }
}
