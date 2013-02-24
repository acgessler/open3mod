///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v0.1)
// [TextureSet.cs]
// (c) 2012-2013, Alexander C. Gessler
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

        /// <summary>
        /// Texture callback delegate, see AddCallback()
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tex"></param>
        /// <returns>Return false to unregister the callback</returns>
        public delegate bool TextureCallback(string name, Texture tex);
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
                    if(!callback(s, _dict[s]))
                    {
                        return;
                    }
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
                    for (int i = 0, e = _textureCallbacks.Count; i < e; )
                    {
                        var callback = _textureCallbacks[i];
                        if (!callback(path, self))
                        {
                            _textureCallbacks.RemoveAt(i);
                            --e;
                            continue;
                        }
                        ++i;
                    }
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

/* vi: set shiftwidth=4 tabstop=4: */ 