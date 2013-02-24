///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureLoader.cs]
// (c) 2012-2013, Alexander C. Gessler
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


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace open3mod
{
    /// <summary>
    /// Responsible for loading a texture from a given folder and file name.
    /// Employs some heuristics to find lost textures. One use only.
    /// </summary>
    public class TextureLoader
    {
        public enum LoadResult
        {
            Good,
            FileNotFound,
            UnknownFileFormat
        }

        private readonly LoadResult _result;
        private readonly Image _image;

        public TextureLoader(string name, string basedir)
        {
            try
            {
                using (var stream = ObtainStream(name, basedir))
                {
                    Debug.Assert(stream != null);

                    // try loading using standard .net first
                    try
                    {
                        _image = Image.FromStream(stream);
                        _result = LoadResult.Good;
                    }     
                    catch (Exception)
                    {
                        // TODO try using DevIL
                    }
                }
            }
            catch(IOException)
            {
                _result = LoadResult.FileNotFound;
            }
        }

        private Stream ObtainStream(string name, string basedir)
        {
            return new FileStream(Path.Combine(basedir, name), FileMode.Open, FileAccess.Read);
        }

        public Image Image
        {
            get { return _image; }
        }

        public LoadResult Result
        {
            get { return _result; }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 