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
using DevIL.Unmanaged;

namespace DevIL {
    public sealed class ImageExporter : IDisposable {
        private bool m_isDisposed;

        public bool IsDisposed {
            get {
                return m_isDisposed;
            }
        }

        public ImageExporter() {
            m_isDisposed = false;
            IL.AddRef();
        }

        ~ImageExporter() {
            Dispose(false);
        }

        public bool SaveImage(Image image, String filename) {
            if(!image.IsValid || String.IsNullOrEmpty(filename)) {
                return false;
            }

            CheckDisposed();

            IL.BindImage(image.ImageID);
            return IL.SaveImage(filename);
        }

        public bool SaveImage(Image image, ImageType imageType, String filename) {
            if(!image.IsValid || imageType == ImageType.Unknown || String.IsNullOrEmpty(filename)) {
                return false;
            }

            CheckDisposed();

            IL.BindImage(image.ImageID);
            return IL.SaveImage(imageType, filename);
        }

        public bool SaveImageToStream(Image image, ImageType imageType, Stream stream) {
            if(!image.IsValid || imageType == ImageType.Unknown || stream == null || !stream.CanWrite) {
                return false;
            }

            CheckDisposed();

            IL.BindImage(image.ImageID);
            return IL.SaveImageToStream(imageType, stream);
        }

        public String[] GetSupportedExtensions() {
            return IL.GetExportExtensions();
        }

        private void CheckDisposed() {
            if(m_isDisposed) {
                throw new ObjectDisposedException("Exporter has been disposed.");
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(!m_isDisposed) {

                IL.Release();

                m_isDisposed = true;
            }
        }
    }
}
