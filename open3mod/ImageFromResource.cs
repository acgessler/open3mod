using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
