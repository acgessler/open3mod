///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [TextureLoader.cs]
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


using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DevIL;
using Image = System.Drawing.Image;


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
        protected string _actualLocation;


        protected TextureLoader()
        {
            _result = LoadResult.UnknownFileFormat;
        }


        public TextureLoader(string name, string basedir)
        {
            try
            {
                using (var stream = ObtainStream(name, basedir, out _actualLocation))
                {
                    Debug.Assert(stream != null);
                    SetFromStream(stream);
                }
            }
            catch(Exception) 
            {
                _result = LoadResult.FileNotFound;
            }
        }


        protected void SetFromStream(Stream stream)
        {
            // try loading using standard .net first
            try
            {
                // We need to copy the stream over to a new, in-memory bitmap to
                // avoid keeping the input stream open.
                // See http://support.microsoft.com/kb/814675/en-us
                using(var img = Image.FromStream(stream))
                {
                    _image = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);

                    using (var gfx = Graphics.FromImage(_image))
                    {
                        gfx.DrawImage(img, 0, 0, img.Width, img.Height);
                    }

                    _result = LoadResult.Good;
                }         
            }
            catch (Exception)
            {
                // if this fails, load using DevIL
                using (var imp = new DevIL.ImageImporter())
                {
                    try
                    {
                        using(var devilImage = imp.LoadImageFromStream(stream))
                        {
                            devilImage.Bind();

                            var info = DevIL.Unmanaged.IL.GetImageInfo();
                            var bitmap = new Bitmap(info.Width, info.Height, PixelFormat.Format32bppArgb);
                            var rect = new Rectangle(0, 0, info.Width, info.Height);
                            var data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                            DevIL.Unmanaged.IL.CopyPixels(0, 0, 0, info.Width, info.Height, 1, DataFormat.BGRA, DataType.UnsignedByte, data.Scan0);

                            bitmap.UnlockBits(data);
                            _image = bitmap;
                            _result = LoadResult.Good;
                        }                
                    }
                    catch (Exception)
                    {
                        // TODO any other viable fall back image loaders?
                        _result = LoadResult.UnknownFileFormat;
                    }
                }
            }
        }


        /// <summary>
        /// Try to obtain a read stream to a given file, looking at some alternative
        /// locations if direct access fails. In case of failure, this method
        /// throws IOException.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="basedir"></param>
        /// <param name="actualLocation"></param>
        /// <returns>A valid stream</returns>
        public static Stream ObtainStream(string name, string basedir, out string actualLocation)
        {
            Debug.Assert(name != null);
            Debug.Assert(basedir != null);

            Stream s = null;
            string path = null;
            try
            {
                path = Path.Combine(basedir, name);
                s = new FileStream(path, FileMode.Open, FileAccess.Read);
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
                    path = Path.Combine(basedir, fileName);
                    s = new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                catch (IOException)
                {
                    try
                    {
                        path = name;
                        s = new FileStream(name, FileMode.Open, FileAccess.Read);
                    }
                    catch (IOException)
                    {
                        if (CoreSettings.CoreSettings.Default.AdditionalTextureFolders != null)
                        {
                            foreach (var folder in CoreSettings.CoreSettings.Default.AdditionalTextureFolders)
                            {
                                try
                                {
                                    path = Path.Combine(folder, fileName);
                                    s = new FileStream(path, FileMode.Open, FileAccess.Read);
                                    break;
                                }
                                catch (IOException)
                                {
                                    continue;
                                }
                            }
                        }
                        if (s == null)
                        {
                            throw new IOException();
                        }
                    }
                }
            }

            Debug.Assert(s != null);
            Debug.Assert(path != null);
            actualLocation = path;
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

        public string ActualLocation
        {
            get { return _actualLocation; }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 