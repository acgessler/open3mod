using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public TextureLoader(string path, string basedir)
        {
            _result = LoadResult.FileNotFound;
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
