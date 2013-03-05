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

        public static Image Get(string resPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            // for some reason we need to keep the stream open for the _lifetime_ of the Image,
            // therefore the Dispose() is _not_ missing here.
            var stream = assembly.GetManifestResourceStream(resPath);

            StreamRefs.Add(stream);

            Debug.Assert(stream != null);
            return Image.FromStream(stream);
        }
    }
}
