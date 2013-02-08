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
    /// Maintains all the textures for a scene. The scene just adds the names
    /// of the textures it needs and TextureSet loads them asynchronously,
    /// offering a callback whenever a texture finishes.
    /// </summary>
    public class TextureSet : IDisposable
    {
        private readonly string _baseDir;
        private readonly Dictionary<string, Texture> _dict;
        private readonly List<string> _loaded; 

        public delegate void TextureCallback(string name, Texture tex);
        private readonly List<TextureCallback> _textureCallbacks;


        public TextureSet(string baseDir)
        {
            _baseDir = baseDir;
            _dict = new Dictionary<string, Texture>();
            _loaded = new List<string>();
            _textureCallbacks = new List<TextureCallback>();
        }


        /// <summary>
        /// Register a callback method that will be invoked once a texture
        /// has finished loading. 
        /// </summary>
        /// <param name="callback">A callback that is safe to be invoked
        ///    from any thread.</param>
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
            _dict.Add(path, new Texture(path, _baseDir, self =>
            {
                lock(_loaded)
                {
                    Debug.Assert(_dict.ContainsKey(path));
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
            lock (_loaded) // TODO don't know if this is safe here
            {
                _dict.Clear();
                _loaded.Clear();
            }
        }
    }
}
