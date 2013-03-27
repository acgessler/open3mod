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

        protected LoadResult _result;
        protected Image _image;


        protected TextureLoader()
        {
            _result = LoadResult.UnknownFileFormat;
        }


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
                        if (_image != null)
                        {
                            _result = LoadResult.Good;
                        }
                        else
                        {
                            _result = LoadResult.UnknownFileFormat;   
                        }
                    }     
                    catch (Exception)
                    {
                        // TODO try using DevIL
                    }
                }
            }
            catch(Exception) 
            {
                _result = LoadResult.FileNotFound;
            }
        }

        /// <summary>
        /// Try to obtain a read stream to a given file, looking at some alternative
        /// locations if direct access fails. In case of failure, this method
        /// throws IOException.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="basedir"></param>
        /// <returns>A valid stream</returns>
        private Stream ObtainStream(string name, string basedir)
        {
            Debug.Assert(name != null);
            Debug.Assert(basedir != null);

            Stream s = null;
            try
            {
                s = new FileStream(Path.Combine(basedir, name), FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                var fileName = Path.GetFileName(name);
                if (fileName == null)
                {
                    throw;
                }
                try
                {
                    s = new FileStream(Path.Combine(basedir, fileName), FileMode.Open, FileAccess.Read);
                }
                catch (IOException)
                {
                    s = new FileStream(name, FileMode.Open, FileAccess.Read);
                }
            }

            Debug.Assert(s != null);
            return s;
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