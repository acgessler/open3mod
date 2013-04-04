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
using System.Collections.Generic;
using Assimp.Unmanaged;

namespace Assimp {
    /// <summary>
    /// Represents a completely imported model or scene. Everything that was imported from the given file can be
    /// accessed from here. Once the scene is loaded from unmanaged memory, it resides solely in managed memory
    /// and Assimp's read only copy is released.
    /// </summary>
    public sealed class Scene : IMarshalable<Scene, AiScene> {
        private SceneFlags m_flags;
        private Node m_rootNode;
        private List<Mesh> m_meshes;
        private List<Light> m_lights;
        private List<Camera> m_cameras;
        private List<EmbeddedTexture> m_textures;
        private List<Animation> m_animations;
        private List<Material> m_materials;

        /// <summary>
        /// Gets or sets the state of the imported scene. By default no flags are set, but
        /// issues can arise if the flag is set to incomplete.
        /// </summary>
        public SceneFlags SceneFlags {
            get {
                return m_flags;
            }
            set {
                m_flags = value;
            }
        }

        /// <summary>
        /// Gets or sets the root node of the scene graph. There will always be at least the root node
        /// if the import was successful and no special flags have been set. Presence of further nodes
        /// depends on the format and content of the imported file.
        /// </summary>
        public Node RootNode {
            get {
                return m_rootNode;
            }
            set {
                m_rootNode = value;
            }
        }

        /// <summary>
        /// Gets if the scene contains meshes. Unless if no special scene flags are set
        /// this should always be true.
        /// </summary>
        public bool HasMeshes {
            get {
                return m_meshes.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of meshes in the scene.
        /// </summary>
        public int MeshCount {
            get {
                return m_meshes.Count;
            }
        }

        /// <summary>
        /// Gets the meshes contained in the scene, if any.
        /// </summary>
        public List<Mesh> Meshes {
            get {
                return m_meshes;
            }
        }


        /// <summary>
        /// Gets if the scene contains any lights.
        /// </summary>
        public bool HasLights {
            get {
                return m_lights.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of lights in the scene.
        /// </summary>
        public int LightCount {
            get {
                return m_lights.Count;
            }
        }

        /// <summary>
        /// Gets the lights in the scene, if any.
        /// </summary>
        public List<Light> Lights {
            get {
                return m_lights;
            }
        }

        /// <summary>
        /// Gets if the scene contains any cameras.
        /// </summary>
        public bool HasCameras {
            get {
                return m_cameras.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of cameras in the scene.
        /// </summary>
        public int CameraCount {
            get {
                return m_cameras.Count;
            }
        }
        /// <summary>
        /// Gets the cameras in the scene, if any.
        /// </summary>
        public List<Camera> Cameras {
            get {
                return m_cameras;
            }
        }

        /// <summary>
        /// Gets if the scene contains embedded textures.
        /// </summary>
        public bool HasTextures {
            get {
                return m_textures.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of embedded textures in the scene.
        /// </summary>
        public int TextureCount {
            get {
                return m_textures.Count;
            }
        }

        /// <summary>
        /// Gets the embedded textures in the scene, if any.
        /// </summary>
        public List<EmbeddedTexture> Textures {
            get {
                return m_textures;
            }
        }

        /// <summary>
        /// Gets if the scene contains any animations.
        /// </summary>
        public bool HasAnimations {
            get {
                return m_animations.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of animations in the scene.
        /// </summary>
        public int AnimationCount {
            get {
                return m_animations.Count;
            }
        }

        /// <summary>
        /// Gets the animations in the scene, if any.
        /// </summary>
        public List<Animation> Animations {
            get {
                return m_animations;
            }
        }

        /// <summary>
        /// Gets if the scene contains any materials. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public bool HasMaterials {
            get {
                return m_materials.Count > 0;
            }
        }

        /// <summary>
        /// Gets the number of materials in the scene. There should always be at least the
        /// default Assimp material if no materials were loaded.
        /// </summary>
        public int MaterialCount {
            get {
                return m_materials.Count;
            }
        }

        /// <summary>
        /// Gets the materials in the scene.
        /// </summary>
        public List<Material> Materials {
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
            m_meshes = new List<Mesh>();
            m_lights = new List<Light>();
            m_cameras = new List<Camera>();
            m_textures = new List<EmbeddedTexture>();
            m_animations = new List<Animation>();
            m_materials = new List<Material>();

            //Read materials
            if(scene.NumMaterials > 0 && scene.Materials != IntPtr.Zero) {
                AiMaterial[] materials = MemoryHelper.MarshalArray<AiMaterial>(scene.Materials, (int) scene.NumMaterials, true);
                for(int i = 0; i < materials.Length; i++) {
                    m_materials.Add(new Material(ref materials[i]));
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
                for(int i = 0; i < meshes.Length; i++) {
                    m_meshes.Add(new Mesh(ref meshes[i]));
                }
            }

            //Read lights
            if(scene.NumLights > 0 && scene.Lights != IntPtr.Zero) {
                AiLight[] lights = MemoryHelper.MarshalArray<AiLight>(scene.Lights, (int) scene.NumLights, true);
                for(int i = 0; i < lights.Length; i++) {
                    m_lights.Add(new Light(ref lights[i]));
                }
            }

            //Read cameras
            if(scene.NumCameras > 0 && scene.Cameras != IntPtr.Zero) {
                AiCamera[] cameras = MemoryHelper.MarshalArray<AiCamera>(scene.Cameras, (int) scene.NumCameras, true);
                for(int i = 0; i < cameras.Length; i++) {
                    m_cameras.Add(new Camera(ref cameras[i]));
                }
            }
            /*
            //Read Textures
            if(scene.NumTextures > 0 && scene.Textures != IntPtr.Zero) {
                AiTexture[] textures = MemoryHelper.MarshalArray<AiTexture>(scene.Textures, (int) scene.NumTextures, true);
                for(int i = 0; i < textures.Length; i++) {
                    m_textures[i] = EmbeddedTexture.CreateTexture(ref textures[i]);
                }
            }*/

            //Read animations
            if(scene.NumAnimations > 0 && scene.Animations != IntPtr.Zero) {
                AiAnimation[] animations = MemoryHelper.MarshalArray<AiAnimation>(scene.Animations, (int) scene.NumAnimations, true);
                for(int i = 0; i < animations.Length; i++) {
                    m_animations.Add(new Animation(ref animations[i]));
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Scene"/> class.
        /// </summary>
        public Scene() {
            m_flags = SceneFlags.None;
            m_rootNode = null;
            m_meshes = new List<Mesh>();
            m_lights = new List<Light>();
            m_cameras = new List<Camera>();
            m_textures = new List<EmbeddedTexture>();
            m_animations = new List<Animation>();
            m_materials = new List<Material>();
        }

        /// <summary>
        /// Clears the scene of all components.
        /// </summary>
        public void Clear() {
            m_rootNode = null;
            m_meshes.Clear();
            m_lights.Clear();
            m_cameras.Clear();
            m_textures.Clear();
            m_animations.Clear();
            m_materials.Clear();
        }

        public static IntPtr ToUnmanagedScene(Scene scene) {
            if(scene == null)
                return IntPtr.Zero;

            return MemoryHelper.ToNativePointer<Scene, AiScene>(scene);
        }

        public static Scene FromUnmanagedScene(IntPtr scenePtr) {
            if(scenePtr == IntPtr.Zero)
                return null;

            return MemoryHelper.FromNativePointer<Scene, AiScene>(scenePtr);
        }

        public static void FreeUnmanagedScene(IntPtr scenePtr) {
            if(scenePtr == IntPtr.Zero)
                return;

            FreeNative(scenePtr, true);
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Scene, AiScene>.IsNativeBlittable {
            get { return true; }
        }

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Scene, AiScene>.ToNative(IntPtr thisPtr, out AiScene nativeValue) {
            nativeValue.Flags = m_flags;
            nativeValue.Materials = IntPtr.Zero;
            nativeValue.RootNode = IntPtr.Zero;
            nativeValue.Meshes = IntPtr.Zero;
            nativeValue.Lights = IntPtr.Zero;
            nativeValue.Cameras = IntPtr.Zero;
            nativeValue.Textures = IntPtr.Zero;
            nativeValue.Animations = IntPtr.Zero;
            nativeValue.PrivateData = IntPtr.Zero;

            nativeValue.NumMaterials = (uint) MaterialCount;
            nativeValue.NumMeshes = (uint) MeshCount;
            nativeValue.NumLights = (uint) LightCount;
            nativeValue.NumCameras = (uint) CameraCount;
            nativeValue.NumTextures = (uint) TextureCount;
            nativeValue.NumAnimations = (uint) AnimationCount;

            //Write materials
            if(nativeValue.NumMaterials > 0)
                nativeValue.Materials = MemoryHelper.ToNativeArray<Material, AiMaterial>(m_materials.ToArray(), true);

            //Write scenegraph
            if(m_rootNode != null)
                nativeValue.RootNode = MemoryHelper.ToNativePointer<Node, AiNode>(m_rootNode);

            //Write meshes
            if(nativeValue.NumMeshes > 0)
                nativeValue.Meshes = MemoryHelper.ToNativeArray<Mesh, AiMesh>(m_meshes.ToArray(), true);

            //Write lights
            if(nativeValue.NumLights > 0)
                nativeValue.Lights = MemoryHelper.ToNativeArray<Light, AiLight>(m_lights.ToArray(), true);

            //Write cameras
            if(nativeValue.NumCameras > 0)
                nativeValue.Cameras = MemoryHelper.ToNativeArray<Camera, AiCamera>(m_cameras.ToArray(), true);

            //Write textures
            if(nativeValue.NumTextures > 0)
                nativeValue.Textures = MemoryHelper.ToNativeArray<EmbeddedTexture, AiTexture>(m_textures.ToArray(), true);

            //Write animations
            if(nativeValue.NumAnimations > 0)
                nativeValue.Animations = MemoryHelper.ToNativeArray<Animation, AiAnimation>(m_animations.ToArray(), true);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Scene, AiScene>.FromNative(ref AiScene nativeValue) {
            Clear();

            m_flags = nativeValue.Flags;

            //Read materials
            if(nativeValue.NumMaterials > 0 && nativeValue.Materials != IntPtr.Zero)
                m_materials.AddRange(MemoryHelper.FromNativeArray<Material, AiMaterial>(nativeValue.Materials, (int) nativeValue.NumMaterials, true));

            //Read scenegraph
            if(nativeValue.RootNode != IntPtr.Zero)
                m_rootNode = MemoryHelper.FromNativePointer<Node, AiNode>(nativeValue.RootNode);

            //Read meshes
            if(nativeValue.NumMeshes > 0 && nativeValue.Meshes != IntPtr.Zero)
                m_meshes.AddRange(MemoryHelper.FromNativeArray<Mesh, AiMesh>(nativeValue.Meshes, (int) nativeValue.NumMeshes, true));

            //Read lights
            if(nativeValue.NumLights > 0 && nativeValue.Lights != IntPtr.Zero)
                m_lights.AddRange(MemoryHelper.FromNativeArray<Light, AiLight>(nativeValue.Lights, (int) nativeValue.NumLights, true));

            //Read cameras
            if(nativeValue.NumCameras > 0 && nativeValue.Cameras != IntPtr.Zero)
                m_cameras.AddRange(MemoryHelper.FromNativeArray<Camera, AiCamera>(nativeValue.Cameras, (int) nativeValue.NumCameras, true));

            //Read textures
            if(nativeValue.NumTextures > 0 && nativeValue.Textures != IntPtr.Zero)
                m_textures.AddRange(MemoryHelper.FromNativeArray<EmbeddedTexture, AiTexture>(nativeValue.Textures, (int) nativeValue.NumTextures, true));

            //Read animations
            if(nativeValue.NumAnimations > 0 && nativeValue.Animations != IntPtr.Zero)
                m_animations.AddRange(MemoryHelper.FromNativeArray<Animation, AiAnimation>(nativeValue.Animations, (int) nativeValue.NumAnimations, true));
        }


        /// <summary>
        /// Frees unmanaged memory created by <see cref="ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative) {
            if(nativeValue == IntPtr.Zero)
                return;

            AiScene aiScene = MemoryHelper.Read<AiScene>(nativeValue);

            if(aiScene.NumMaterials > 0 && aiScene.Materials != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMaterial>(aiScene.Materials, (int) aiScene.NumMaterials, Material.FreeNative, true);

            if(aiScene.RootNode != IntPtr.Zero)
                Node.FreeNative(aiScene.RootNode, true);

            if(aiScene.NumMeshes > 0 && aiScene.Meshes != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiMesh>(aiScene.Meshes, (int) aiScene.NumMeshes, Mesh.FreeNative, true);

            if(aiScene.NumLights > 0 && aiScene.Lights != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiLight>(aiScene.Lights, (int) aiScene.NumLights, Light.FreeNative, true);

            if(aiScene.NumCameras > 0 && aiScene.Cameras != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiCamera>(aiScene.Cameras, (int) aiScene.NumCameras, Camera.FreeNative, true);

            if(aiScene.NumTextures > 0 && aiScene.Textures != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiTexture>(aiScene.Textures, (int) aiScene.NumTextures, EmbeddedTexture.FreeNative, true);

            if(aiScene.NumAnimations > 0 && aiScene.Animations != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiAnimation>(aiScene.Animations, (int) aiScene.NumAnimations, Animation.FreeNative, true);

            if(freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
