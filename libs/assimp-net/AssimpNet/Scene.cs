/*
* Copyright (c) 2012-2013 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Represents a completely imported model or scene. Everything that was imported from the given file can be
    /// accessed from here. Once the scene is loaded from unmanaged memory, it resides solely in managed memory
    /// and Assimp's read only copy is released.
    /// </summary>
    public sealed class Scene {
        private SceneFlags m_flags;
        private Node m_rootNode;
        private Mesh[] m_meshes;
        private Light[] m_lights;
        private Camera[] m_cameras;
        private Texture[] m_textures;
        private Animation[] m_animations;
        private Material[] m_materials;

        /// <summary>
        /// Gets the state of the imported scene. By default no flags are set, but
        /// issues can arise if the flag is set to incomplete.
        /// </summary>
        public SceneFlags SceneFlags {
            get {
                return m_flags;
            }
        }

        /// <summary>
        /// Gets the root node of the scene graph. There will always be at least the root node
        /// if the import was successful and no special flags have been set. Presence of further nodes
        /// depends on the format and content of the imported file.
        /// </summary>
        public Node RootNode {
            get {
                return m_rootNode;
            }
        }

        /// <summary>
        /// Gets the number of meshes in the scene.
        /// </summary>
        public int MeshCount {
            get {
                return (m_meshes == null) ? 0 : m_meshes.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains meshes. Unless if no special scene flags are set
        /// this should always be true.
        /// </summary>
        public bool HasMeshes {
            get {
                return m_meshes != null;
            }
        }

        /// <summary>
        /// Gets the meshes contained in the scene, if any.
        /// </summary>
        public Mesh[] Meshes {
            get {
                return m_meshes;
            }
        }

        /// <summary>
        /// Gets the number of lights in the scene.
        /// </summary>
        public int LightCount {
            get {
                return (m_lights == null) ? 0 : m_lights.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains any lights.
        /// </summary>
        public bool HasLights {
            get {
                return m_lights != null;
            }
        }

        /// <summary>
        /// Gets the lights in the scene, if any.
        /// </summary>
        public Light[] Lights {
            get {
                return m_lights;
            }
        }

        /// <summary>
        /// Gets the number of cameras in the scene.
        /// </summary>
        public int CameraCount {
            get {
                return (m_cameras == null) ? 0 : m_cameras.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains any cameras.
        /// </summary>
        public bool HasCameras {
            get {
                return m_cameras != null;
            }
        }

        /// <summary>
        /// Gets the cameras in the scene, if any.
        /// </summary>
        public Camera[] Cameras {
            get {
                return m_cameras;
            }
        }

        /// <summary>
        /// Gets the number of embedded textures in the scene.
        /// </summary>
        public int TextureCount {
            get {
                return (m_textures == null) ? 0 : m_textures.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains embedded textures.
        /// </summary>
        public bool HasTextures {
            get {
                return m_textures != null;
            }
        }

        /// <summary>
        /// Gets the embedded textures in the scene, if any.
        /// </summary>
        public Texture[] Textures {
            get {
                return m_textures;
            }
        }

        /// <summary>
        /// Gets the number of animations in the scene.
        /// </summary>
        public int AnimationCount {
            get {
                return (m_animations == null) ? 0 : m_animations.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains any animations.
        /// </summary>
        public bool HasAnimations {
            get {
                return m_animations != null;
            }
        }

        /// <summary>
        /// Gets the animations in the scene, if any.
        /// </summary>
        public Animation[] Animations {
            get {
                return m_animations;
            }
        }

        /// <summary>
        /// Gets the number of materials in the scene. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public int MaterialCount {
            get {
                return (m_materials == null) ? 0 : m_materials.Length;
            }
        }

        /// <summary>
        /// Checks if the scene contains any materials. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public bool HasMaterials {
            get {
                return m_materials != null;
            }
        }

        /// <summary>
        /// Gets the materials in the scene.
        /// </summary>
        public Material[] Materials {
            get {
                return m_materials;
            }
        }

        /// <summary>
        /// Constructs a new Scene.
        /// </summary>
        /// <param name="scene">Unmanaged AiScene struct.</param>
        internal Scene(AiScene scene) {
            m_flags = scene.Flags;

            //Read materials
            if(scene.NumMaterials > 0 && scene.Materials != IntPtr.Zero) {
                AiMaterial[] materials = MemoryHelper.MarshalArray<AiMaterial>(scene.Materials, (int) scene.NumMaterials, true);
                m_materials = new Material[materials.Length];
                for(int i = 0; i < m_materials.Length; i++) {
                    m_materials[i] = new Material(ref materials[i]);
                }
            }

            //Read scenegraph
            if(scene.RootNode != IntPtr.Zero) {
                AiNode aiRootNode = MemoryHelper.MarshalStructure<AiNode>(scene.RootNode);
                m_rootNode = new Node(ref aiRootNode, null);
            }

            //Read meshes
            if(scene.NumMeshes > 0 && scene.Meshes != IntPtr.Zero) {
                AiMesh[] meshes = MemoryHelper.MarshalArray<AiMesh>(scene.Meshes, (int) scene.NumMeshes, true);
                m_meshes = new Mesh[meshes.Length];
                for(int i = 0; i < m_meshes.Length; i++) {
                    m_meshes[i] = new Mesh(ref meshes[i]);
                }
            }

            //Read lights
            if(scene.NumLights > 0 && scene.Lights != IntPtr.Zero) {
                AiLight[] lights = MemoryHelper.MarshalArray<AiLight>(scene.Lights, (int) scene.NumLights, true);
                m_lights = new Light[lights.Length];
                for(int i = 0; i < m_lights.Length; i++) {
                    m_lights[i] = new Light(ref lights[i]);
                }
            }

            //Read cameras
            if(scene.NumCameras > 0 && scene.Cameras != IntPtr.Zero) {
                AiCamera[] cameras = MemoryHelper.MarshalArray<AiCamera>(scene.Cameras, (int) scene.NumCameras, true);
                m_cameras = new Camera[cameras.Length];
                for(int i = 0; i < m_cameras.Length; i++) {
                    m_cameras[i] = new Camera(ref cameras[i]);
                }
            }

            //Read Textures
            if(scene.NumTextures > 0 && scene.Textures != IntPtr.Zero) {
                AiTexture[] textures = MemoryHelper.MarshalArray<AiTexture>(scene.Textures, (int) scene.NumTextures, true);
                m_textures = new Texture[textures.Length];
                for(int i = 0; i < m_textures.Length; i++) {
                    m_textures[i] = Texture.CreateTexture(ref textures[i]);
                }
            }

            //Read animations
            if(scene.NumAnimations > 0 && scene.Animations != IntPtr.Zero) {
                AiAnimation[] animations = MemoryHelper.MarshalArray<AiAnimation>(scene.Animations, (int) scene.NumAnimations, true);
                m_animations = new Animation[animations.Length];
                for(int i = 0; i < m_animations.Length; i++) {
                    m_animations[i] = new Animation(ref animations[i]);
                }
            }
        }
    }
}
