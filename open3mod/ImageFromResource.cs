///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [ImageFromResource.cs]
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace open3mod
{
    public static class ImageFromResource
    {
        // this only exists to keep references to all image streams
        private static readonly List<Stream> StreamRefs = new List<Stream>();
        private static readonly Dictionary<string,Image> Cache = new Dictionary<string, Image>(); 

        /// <summary>
        /// Load a given embedded image resource.
        /// Images are cached so redundant calls are ok to make.
        ///  </summary>
        /// <param name="resPath">Resource identifier</param>
        /// <returns></returns>
        public static Image Get(string resPath)
        {
            Image img;
            if (Cache.TryGetValue(resPath, out img))
            {
                return img;
            }

            var assembly = Assembly.GetExecutingAssembly();
            // for some reason we need to keep the stream open for the _lifetime_ of the Image,
            // therefore the Dispose() is _not_ missing here.
            var stream = assembly.GetManifestResourceStream(resPath);

            StreamRefs.Add(stream);

            Debug.Assert(stream != null);
            img = Image.FromStream(stream);

            Cache[resPath] = img;
            return img;
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 