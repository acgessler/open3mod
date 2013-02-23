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
    /// 
    /// Textures are uniquely identified by string identifiers, which are 
    /// (usually) their loading paths. These paths need not be unique and this is 
    /// also not enforced - if two paths are different but point to the
    /// same physical file, it may happen that this file gets loaded twice.
    /// 
    /// The class also handles replacing textures by other files (i.e. by
    /// dragging them onto the texture inspector view). Replacements are
    /// recorded so exporters and renderer can properly apply them. Replaced
    /// textures receive new, unique IDs even if they did already exist
    /// to avoid all the issues that could arise from cyclic replacements.
    /// </summary>
    public class TextureSet : IDisposable
    {
        private readonly string _baseDir;
        private readonly Dictionary<string, Texture> _dict;
        private readonly List<string> _loaded; 

        public delegate void TextureCallback(string name, Texture tex);
        private readonly List<TextureCallback> _textureCallbacks;

        private readonly Dictionary<string, KeyValuePair<string,string>> _replacements;
 

        public TextureSet(string baseDir)
        {
            _baseDir = baseDir;
            _dict = new Dictionary<string, Texture>();
            _replacements = new Dictionary<string, KeyValuePair<string, string> >();

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


        /// <summary>
        /// Add a texture to the TextureSet. This is the only place where
        /// Texture instances are actually created. This also schedules
        /// the texture for loading, which then happens asynchronously.
        /// </summary>
        /// <param name="path">Texture's given path</param>
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


        /// <summary>
        /// Check if a given texture exists.
        /// </summary>
        /// <param name="path">Texture id to check</param>
        public bool Exists(string path)
        {
            return _dict.ContainsKey(path);
        }


        /// <summary>
        /// Get a given texture instance. 
        /// </summary>
        /// <param name="path">Texture id to retrieve. A texture with
        /// this id _must_ be contained in the set</param>
        public Texture Get(string path)
        {
            Debug.Assert(Exists(path));
            return _dict[path];
        }


        /// <summary>
        /// Get a given texture instance or the texture instance it
        /// has been replaced with.
        /// </summary>
        /// <param name="path">Texture id to retrieve. A texture with
        /// this id _must_ either be contained in this set or it
        /// must have been replaced by another texture using Replace()</param>
        public Texture GetOriginalOrReplacement(string path)
        {
            Texture val;
            if(_dict.TryGetValue(path, out val))
            {
                return val;
            }

            Debug.Assert(_replacements.ContainsKey(path));
            return GetOriginalOrReplacement(_replacements[path].Key);
        }


        /// <summary>
        /// Delete a texture from the set.
        /// </summary>
        /// <param name="path">Texture id to be removed. A texture with
        /// this id _must_ be contained in the set</param>
        public void Delete(string path)
        {
            Debug.Assert(Exists(path));
            var old = Get(path);
            old.Dispose();
            _dict.Remove(path);

            // TODO what to do with _replacements?
        }


        /// <summary>
        /// Replace a texture with another texture. This does two things:
        /// i) it loads the new texture
        /// ii) it creates a note that the texture has been replaced
        /// 
        /// If the new texture does already exist in the texture set,
        /// the existing instance will _not_ be re-used. Instead,
        /// a new instance with an unique name will be created (this 
        /// prevents cyclic replacements).
        /// </summary>
        /// <param name="path">Old texture id, must exist</param>
        /// <param name="newPath">New texture id, may already exist</param>
        /// <returns>The id of the replacement texture. This is 
        ///    the same as the id passed for the newPath parameter unless
        ///    this id did already exist. </returns>
        public string Replace(string path, string newPath)
        {
            Debug.Assert(Exists(path));
            Debug.Assert(path != newPath);

            Delete(path);

            string newId;
            if(Exists(newPath))
            {
                newId = newPath + '_' + Guid.NewGuid();
            }
            else
            {
                newId = newPath;
                Add(newPath);
            }

            _replacements[path] = new KeyValuePair<string, string>(newId, newPath);
            return newId;
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
