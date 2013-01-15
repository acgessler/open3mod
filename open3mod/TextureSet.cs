using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Maintains the textures for a scene. 
    /// </summary>
    public class TextureSet : Dictionary<string, Texture>, IDisposable
    {
        
        public void Dispose()
        {
            foreach(var k in this)
            {
                k.Value.Dispose();
            }
            Clear();
        }
    }
}
