using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Maintains the textures for a scene. 
    /// </summary>
    public class TextureSet : IDisposable
    {
        private readonly Dictionary<string, Texture> _dict;
        private readonly List<string> _loaded; 

        public delegate void TextureCallback(string name, Texture tex);
        private readonly List<TextureCallback> _textureCallbacks;


        public TextureSet()
        {
            _dict = new Dictionary<string, Texture>();
            _loaded = new List<string>();
            _textureCallbacks = new List<TextureCallback>();
        }


        public void AddCallback(TextureCallback callback)
        {
            Debug.Assert(callback != null);

            lock (_loaded)
            {
                foreach(var s in _loaded)
                {
                    callback(s, _dict[s]);
                }
                _textureCallbacks.Add(callback);
            }           
        }


        public void Add(string path)
        {
            if(_dict.ContainsKey(path))
            {
                return;
            }
            _dict.Add(path, new Texture(path, self =>
            {
                lock(_loaded)
                {
                    _loaded.Add(path);
                    _textureCallbacks.ForEach(callback => callback(path, self));
                }               
            }));
        }

        public void Dispose()
        {
            foreach(var k in _dict)
            {
                k.Value.Dispose();
            }
            _dict.Clear();
        }
    }
}
