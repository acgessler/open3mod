///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [EmbeddedTextureLoader.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Assimp;

namespace open3mod
{
    public class EmbeddedTextureLoader : TextureLoader
    {
        public EmbeddedTextureLoader(EmbeddedTexture tex) 
        {
            if(tex.IsCompressed)
            {
                var compTex = tex;
                if(!compTex.HasCompressedData)
                {
                    return;
                }

                // note: have to keep the stream open for the lifetime of the image, so don't Dispose()
                SetFromStream(new MemoryStream(compTex.CompressedData));
                return;
            }

            var rawTex = tex;
            if (!rawTex.HasNonCompressedData || rawTex.Width < 1 || rawTex.Height < 1)
            {
                return;
            }
            var texels = rawTex.NonCompressedData;

            
            var image = new Bitmap(rawTex.Width, rawTex.Height, PixelFormat.Format32bppArgb);

            var bounds = new Rectangle(0, 0, rawTex.Width, rawTex.Height);
            BitmapData bmpData;

            try
            {
                bmpData = image.LockBits(bounds, ImageLockMode.WriteOnly, image.PixelFormat);
                // ignore exceptions thrown by LockBits - we just can't read the image in this case
            }
            catch
            {
                return;
            }

            var ptr = bmpData.Scan0;

            Debug.Assert(bmpData.Stride > 0);

            var countBytes = bmpData.Stride*image.Height;
            var tempBuffer = new byte[countBytes];

            var dataLineLength = image.Width*4;
            var padding = bmpData.Stride - dataLineLength;
            Debug.Assert(padding >= 0);

            var n = 0;
            foreach(var texel in texels)
            {
                tempBuffer[n++] = texel.B;
                tempBuffer[n++] = texel.G;
                tempBuffer[n++] = texel.R;
                tempBuffer[n++] = texel.A;

                if(n % dataLineLength == 0)
                {
                    n += padding;
                }
            }

            Marshal.Copy(tempBuffer, 0, ptr, countBytes);
            image.UnlockBits(bmpData);

            _image = image;
            _result = LoadResult.Good;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 