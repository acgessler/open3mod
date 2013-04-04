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
using System.IO;
using System.Runtime.InteropServices;

namespace Assimp {

    /// <summary>
    /// Delegate for performing unmanaged memory cleanup.
    /// </summary>
    /// <param name="nativeValue">Location in unmanaged memory of the value to cleanup</param>
    /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise</param>
    public delegate void FreeNativeDelegate(IntPtr nativeValue, bool freeNative);

    /// <summary>
    /// Helper static class containing functions that aid dealing with unmanaged memory to managed memory conversions.
    /// </summary>
    public static class MemoryHelper {
        public static int ALLOCCOUNT = 0;
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
        /// the IntPtr and care should be taken in ensuring that the number of elements to read is correct.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pointer">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <param name="arrayOfPointers">True if the unmanaged pointer is void** (array of pointers) or not.</param>
        /// <returns>Managed array</returns>
        public static T[] MarshalArray<T>(IntPtr pointer, int length, bool arrayOfPointers) where T : struct {
            if(pointer == IntPtr.Zero) {
                return new T[0];
            }

            try {
                Type type = typeof(T);
                //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
                int stride = (arrayOfPointers) ? IntPtr.Size : Marshal.SizeOf(typeof(T));
                T[] array = new T[length];

                for(int i = 0; i < length; i++) {
                    IntPtr currPos = AddIntPtr(pointer, stride * i);
                    //If pointer is a void**, read the current position to get the proper pointer
                    if(arrayOfPointers) {
                        currPos = Marshal.ReadIntPtr(currPos);
                    }
                    array[i] = (T) Marshal.PtrToStructure(currPos, type);
                }

                return array;
            } catch(Exception) {
                return new T[0];
            }
        }

        /// <summary>
        /// Marshals a managed array of structs to a c-style pointer array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="array">Managed array</param>
        /// <returns>Pointer to unmanaged array</returns>
        public static IntPtr MarshalArray<T>(T[] array) where T : struct {
            if(array == null || array.Length == 0)
                return IntPtr.Zero;

            int size = SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(size * array.Length);

            Write<T>(ptr, array, 0, array.Length);

            return ptr;
        }

        public static T[] MarshalArrayFromNative<T>(IntPtr ptr, int length) where T : struct {
            if(ptr == IntPtr.Zero || length == 0)
                return new T[0];

            T[] array = new T[length];

            Read<T>(ptr, array, 0, length);

            return array;
        }

        #region Marshaling Interop

        /// <summary>
        /// Marshals an array of managed values to a c-style unmanaged array (void*).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="managedArray">Array of managed values</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<Managed, Native>(Managed[] managedArray)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct {

            return ToNativeArray<Managed, Native>(managedArray, false);
        }

        /// <summary>
        /// Marshals an array of managed values to a c-style unmanaged array (void*). This also can optionally marshal to
        /// an unmanaged array of pointers (void**).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="managedArray">Array of managed values</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<Managed, Native>(Managed[] managedArray, bool arrayOfPointers) 
            where Managed : class, IMarshalable<Managed, Native>, new() 
            where Native : struct {

            if(managedArray == null || managedArray.Length == 0)
                return IntPtr.Zero;

            bool isNativeBlittable = IsNativeBlittable<Managed, Native>(managedArray);
            int sizeofNative = (isNativeBlittable) ? SizeOf<Native>() : MarshalSizeOf<Native>();

            //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
            int stride = (arrayOfPointers) ? IntPtr.Size : sizeofNative;
            IntPtr nativeArray = (arrayOfPointers) ? AllocateMemory(managedArray.Length * IntPtr.Size) : AllocateMemory(managedArray.Length * sizeofNative);

            for(int i = 0; i < managedArray.Length; i++) {
                IntPtr currPos = AddIntPtr(nativeArray, stride * i);

                Managed managedValue = managedArray[i];

                //Setup unmanaged data - do the actual ToNative later on, that way we can pass the thisPtr if the object is a pointer type.
                Native nativeValue = default(Native);

                //If array of pointers, each entry is a pointer so allocate memory, fill it, and write pointer to array, 
                //otherwise just write the data to the array location
                if(arrayOfPointers) {
                    IntPtr ptr = IntPtr.Zero;

                    //If managed value is null, write out a NULL ptr rather than wasting our time here
                    if(managedValue != null) {
                        ptr = AllocateMemory(sizeofNative);

                        managedValue.ToNative(ptr, out nativeValue);

                        if(isNativeBlittable) {
                            Write<Native>(ptr, ref nativeValue);
                        } else {
                            Marshal.StructureToPtr(nativeValue, ptr, true);
                        }
                    }

                    Write<IntPtr>(currPos, ref ptr);
                } else {

                    if(managedArray != null)
                        managedValue.ToNative(IntPtr.Zero, out nativeValue);

                    if(isNativeBlittable) {
                        Write<Native>(currPos, ref nativeValue);
                    } else {
                        Marshal.StructureToPtr(nativeValue, currPos, true);
                    }
                }
            }

            return nativeArray;
        }

        /// <summary>
        /// Marshals an array of managed values from a c-style unmanaged array (void*).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <returns>Marshaled managed values</returns>
        public static Managed[] FromNativeArray<Managed, Native>(IntPtr nativeArray, int length)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct {

            return FromNativeArray<Managed, Native>(nativeArray, length, false);
        }

        /// <summary>
        /// Marshals an array of managed values from a c-style unmanaged array (void*). This also can optionally marshal from 
        /// an unmanaged array of pointers (void**).
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Native type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to marshal</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        /// <returns>Marshaled managed values</returns>
        public static Managed[] FromNativeArray<Managed, Native>(IntPtr nativeArray, int length, bool arrayOfPointers) 
            where Managed : class, IMarshalable<Managed, Native>, new() 
            where Native : struct {

            if(nativeArray == IntPtr.Zero || length == 0)
                return new Managed[0];

            //If the pointer is a void** we need to step by the pointer size, otherwise it's just a void* and step by the type size.
            int stride = (arrayOfPointers) ? IntPtr.Size : MarshalSizeOf<Native>();
            Type nativeValueType = typeof(Native);
            Managed[] managedArray = new Managed[length];

            for(int i = 0; i < length; i++) {
                IntPtr currPos = AddIntPtr(nativeArray, stride * i);

                //If pointer is a void**, read the current position to get the proper pointer
                if(arrayOfPointers)
                    currPos = Read<IntPtr>(currPos);

                Managed managedValue = Activator.CreateInstance<Managed>();

                //Marshal structure from the currentPointer position
                Native nativeValue;

                if(managedValue.IsNativeBlittable) {
                    nativeValue = Read<Native>(currPos);
                } else {
                    MarshalStructure<Native>(currPos, out nativeValue);
                }

                //Populate managed data
                managedValue.FromNative(ref nativeValue);

                managedArray[i] = managedValue;
            }

            return managedArray;
        }

        /// <summary>
        /// Marshals an array of blittable structs to a c-style unmanaged array (void*). This should not be used on types
        /// that require marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="managedArray">Managed array of structs</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativeArray<T>(T[] managedArray) where T : struct {
            if(managedArray == null || managedArray.Length == 0)
                return IntPtr.Zero;

            IntPtr ptr = AllocateMemory(SizeOf<T>() * managedArray.Length);

            Write<T>(ptr, managedArray, 0, managedArray.Length);

            return ptr;
        }

        /// <summary>
        /// Marshals an array of blittable structs from a c-style unmanaged array (void*).This should not be used on types
        /// that require marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to read</param>
        /// <returns>Managed array</returns>
        public static T[] FromNativeArray<T>(IntPtr nativeArray, int length) where T : struct {
            if(nativeArray == IntPtr.Zero || length == 0)
                return new T[0];

            T[] managedArray = new T[length];

            Read<T>(nativeArray, managedArray, 0, length);

            return managedArray;
        }

        /// <summary>
        /// Frees an unmanaged array and performs cleanup for each value. This can be used on any type that can be
        /// marshaled into unmanaged memory.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to free</param>
        /// <param name="action">Delegate that performs the necessary cleanup</param>
        public static void FreeNativeArray<T>(IntPtr nativeArray, int length, FreeNativeDelegate action) where T : struct {
            FreeNativeArray<T>(nativeArray, length, action, false);
        }

        /// <summary>
        /// Frees an unmanaged array and performs cleanup for each value. Optionally can free an array of pointers. This can be used on any type that can be
        /// marshaled into unmanaged memory.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="nativeArray">Pointer to unmanaged memory</param>
        /// <param name="length">Number of elements to free</param>
        /// <param name="action">Delegate that performs the necessary cleanup</param>
        /// <param name="arrayOfPointers">True if the pointer is an array of pointers, false otherwise.</param>
        public static void FreeNativeArray<T>(IntPtr nativeArray, int length, FreeNativeDelegate action, bool arrayOfPointers) where T : struct {
            if(nativeArray == IntPtr.Zero || length == 0 || action == null)
                return;

            //If the pointer is a void** we need tp step by the pointer eize, otherwise its just a void* and step by the type size
            int stride = (arrayOfPointers) ? IntPtr.Size : MarshalSizeOf<T>();

            for(int i = 0; i < length; i++) {
                IntPtr currPos = AddIntPtr(nativeArray, stride * i);

                //If pointer is a void**, read the current position to get the proper pointer
                if(arrayOfPointers)
                    currPos = Read<IntPtr>(currPos);

                //Invoke cleanup
                action(currPos, arrayOfPointers);
            }

            FreeMemory(nativeArray);
        }

        /// <summary>
        /// Marshals a managed value to unmanaged memory.
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Unmanaged type</typeparam>
        /// <param name="managedValue">Managed value to marshal</param>
        /// <returns>Pointer to unmanaged memory</returns>
        public static IntPtr ToNativePointer<Managed, Native>(Managed managedValue)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct {

            if(managedValue == null)
                return IntPtr.Zero;

            int sizeofNative = (managedValue.IsNativeBlittable) ? SizeOf<Native>() : MarshalSizeOf<Native>();

            //Allocate memory
            IntPtr ptr = AllocateMemory(sizeofNative);

            //Setup unmanaged data
            Native nativeValue;
            managedValue.ToNative(ptr, out nativeValue);

            if(managedValue.IsNativeBlittable) {
                Write<Native>(ptr, ref nativeValue);
            } else {
                Marshal.StructureToPtr(nativeValue, ptr, true);
            }

            return ptr;
        }

        /// <summary>
        /// Marshals a managed value from unmanaged memory.
        /// </summary>
        /// <typeparam name="Managed">Managed type</typeparam>
        /// <typeparam name="Native">Unmanaged type</typeparam>
        /// <param name="ptr">Pointer to unmanaged memory</param>
        /// <returns>The marshaled managed value</returns>
        public static Managed FromNativePointer<Managed, Native>(IntPtr ptr) 
            where Managed : class, IMarshalable<Managed, Native>, new() 
            where Native : struct {

            if(ptr == IntPtr.Zero)
                return null;

            Managed managedValue = Activator.CreateInstance<Managed>();

            //Marshal pointer to structure
            Native nativeValue;

            if(managedValue.IsNativeBlittable) {
                nativeValue = Read<Native>(ptr);
            } else {
                MarshalStructure<Native>(ptr, out nativeValue);
            }

            //Populate managed value
            managedValue.FromNative(ref nativeValue);

            return managedValue;
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <param name="value">The marshaled structure</param>
        public static void MarshalStructure<T>(IntPtr ptr, out T value) where T : struct {
            if(ptr == IntPtr.Zero)
                value = default(T);

            value = (T) Marshal.PtrToStructure(ptr, typeof(T));
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <returns>The marshaled structure</returns>
        public static T MarshalStructure<T>(IntPtr ptr) where T : struct {
            if(ptr == IntPtr.Zero)
                return default(T);

            return (T) Marshal.PtrToStructure(ptr, typeof(T));
        }

        /// <summary>
        /// Computes the size of the struct type using Marshal SizeOf. Required for any struct that requires
        /// marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        public static unsafe int MarshalSizeOf<T>() where T : struct {
            return Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        /// Computes the size of the struct array using Marshal SizeOf. Required for any struct that requires
        /// marshaling by the runtime (e.g. has MarshalAs attributes).
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="array">Array of structs</param>
        /// <returns>Total size, in bytes, of the array's contents.</returns>
        public static int MarshalSizeOf<T>(T[] array) where T : struct {
            return array == null ? 0 : array.Length * Marshal.SizeOf(typeof(T));
        }

        #endregion

        #region Fast Interop Methods

        /// <summary>
        /// Allocates a chunk of memory.
        /// </summary>
        /// <param name="sizeInBytes">Size in bytes to allocate</param>
        /// <returns>Pointer to allocated memory</returns>
        public static IntPtr AllocateMemory(int sizeInBytes) {
            ALLOCCOUNT++;
            return Marshal.AllocHGlobal(sizeInBytes);
        }

        /// <summary>
        /// Frees a chunk of memory.
        /// </summary>
        /// <param name="memoryPtr">Pointer to memory</param>
        public static void FreeMemory(IntPtr memoryPtr) {
            if(memoryPtr != IntPtr.Zero) {
                Marshal.FreeHGlobal(memoryPtr);
                ALLOCCOUNT--;
            }
        }

        /// <summary>
        /// Frees an array of pointers.
        /// </summary>
        /// <param name="memoryPtr">Pointer to memory</param>
        /// <param name="length">Number of elements</param>
        public static void FreeMemory(IntPtr memoryPtr, int length) {
            if(memoryPtr == IntPtr.Zero || length == 0)
                return;

            int stride = IntPtr.Size;

            for(int i = 0; i < length; i++) {
                IntPtr currPos = Read<IntPtr>(AddIntPtr(memoryPtr, stride * i));

                if(currPos != IntPtr.Zero) {
                    Marshal.FreeHGlobal(currPos);
                    ALLOCCOUNT--;
                }
            }

            Marshal.FreeHGlobal(memoryPtr);
            ALLOCCOUNT--;
        }

        /// <summary>
        /// Clears the memory to the specified value.
        /// </summary>
        /// <param name="memoryPtr">Pointer to the memory.</param>
        /// <param name="clearValue">Value the memory will be cleared to.</param>
        /// <param name="sizeInBytesToClear">Number of bytes, starting from the memory pointer, to clear.</param>
        public static unsafe void ClearMemory(IntPtr memoryPtr, byte clearValue, int sizeInBytesToClear) {
            InternalInterop.MemSetInline((void*) memoryPtr, clearValue, sizeInBytesToClear);
        }

        /// <summary>
        /// Computes the size of the struct type. Not safe if any fields have a MarshalAs attribute, use
        /// <see cref="MarshalSizeOf"/> instead.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        public static unsafe int SizeOf<T>() where T : struct {
            return InternalInterop.SizeOfInline<T>();
        }

        /// <summary>
        /// Computes the size of the struct array. Not safe if any fields have a MarshalAs attribute, use
        /// <see cref="MarshalSizeOf"/> instead.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="array">Array of structs</param>
        /// <returns>Total size, in bytes, of the array's contents.</returns>
        public static int SizeOf<T>(T[] array) where T : struct {
            return array == null ? 0 : array.Length * InternalInterop.SizeOfInline<T>();
        }

        /// <summary>
        /// Adds an offset to the pointer.
        /// </summary>
        /// <param name="ptr">Pointer.</param>
        /// <param name="offset">Offset</param>
        /// <returns>New pointer</returns>
        public static IntPtr AddIntPtr(IntPtr ptr, int offset) {
            return new IntPtr(ptr.ToInt64() + offset);
        }

        /// <summary>
        /// Performs a memcopy that copies data from the memory pointed to by the source pointer to the memory pointer by the destination pointer.
        /// </summary>
        /// <param name="pDest">Destination memory location</param>
        /// <param name="pSrc">Source memory location</param>
        /// <param name="sizeInBytesToCopy">Number of bytes to copy</param>
        public static unsafe void CopyMemory(IntPtr pDest, IntPtr pSrc, int sizeInBytesToCopy) {
            InternalInterop.MemCopyInline((void*) pDest, (void*) pSrc, sizeInBytesToCopy);
        }

        /// <summary>
        /// Converts typed element array to a byte array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="source">Element array</param>
        /// <returns>Byte array copy or null if the array was not valid.</returns>
        public static unsafe byte[] ToByteArray<T>(T[] source) where T : struct {
            if(source == null || source.Length == 0)
                return null;

            byte[] buffer = new byte[SizeOf<T>() * source.Length];

            fixed(void* pBuffer = buffer) {
                Write<T>((IntPtr) pBuffer, source, 0, source.Length);
            }

            return buffer;
        }

        /// <summary>
        /// Reads data from the memory location into the array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <param name="data">Array to store the copied data</param>
        /// <param name="startIndexInArray">Zero-based element index to start writing data to in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Read<T>(IntPtr pSrc, T[] data, int startIndexInArray, int count) where T : struct {
            InternalInterop.ReadArray<T>(pSrc, data, startIndexInArray, count);
        }

        /// <summary>
        /// Reads a single element from the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <returns>The read value</returns>
        public static unsafe T Read<T>(IntPtr pSrc) where T : struct {
            return InternalInterop.ReadInline<T>((void*) pSrc);
        }

        /// <summary>
        /// Writes data from the array to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">Array containing data to write</param>
        /// <param name="startIndexInArray">Zero-based element index to start reading data from in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Write<T>(IntPtr pDest, T[] data, int startIndexInArray, int count) where T : struct {
            InternalInterop.WriteArray<T>(pDest, data, startIndexInArray, count);
        }

        /// <summary>
        /// Writes a single element to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">The value to write</param>
        public static unsafe void Write<T>(IntPtr pDest, ref T data) where T : struct {
            InternalInterop.WriteInline<T>((void*) pDest, ref data);
        }

        #endregion

        #region Misc

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

        //Helper for asking if the IMarshalable's native struct is blittable.
        private static bool IsNativeBlittable<Managed, Native>(Managed managedValue) 
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct {

            return (managedValue != null) ? managedValue.IsNativeBlittable : false;
        }

        //Helper for asking if the IMarshalable's in the array have native structs that are blittable.
        private static bool IsNativeBlittable<Managed, Native>(Managed[] managedArray)
            where Managed : class, IMarshalable<Managed, Native>, new()
            where Native : struct {

           if(managedArray == null || managedArray.Length == 0)
                return false;

            for(int i = 0; i < managedArray.Length; i++) {
                Managed managedValue = managedArray[i];

                if(managedValue != null)
                    return managedValue.IsNativeBlittable;
            }

            return false;
        }

        #endregion
    }
}
