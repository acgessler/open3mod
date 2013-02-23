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
            catch(FileNotFoundException)
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
