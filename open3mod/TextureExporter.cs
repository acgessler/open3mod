using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace open3mod
{
    public class TextureExporter
    {
        private readonly Texture _texture;

        public TextureExporter(Texture texture)
        {
            _texture = texture;
            Debug.Assert(_texture != null);
        }


        public string[] GetExtensionList()
        {
            // GDI+ encoders
            // http://msdn.microsoft.com/de-de/library/vstudio/system.drawing.imaging.imageformat.aspx
            var gdi = new[] {"bmp","emf","exif","gif","ico","jpeg","png","tiff","wmf"};
            return gdi;
        }


        public bool Export(string path)
        {
            try
            {
                _texture.Image.Save(path);
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
    }
}
