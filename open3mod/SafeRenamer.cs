using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assimp;

namespace open3mod
{
    /// <summary>
    /// Rename nodes, meshes, materials and animations while keeping name-based references
    /// within the scene intact. Does not itself provide UI to ask the user for a new name
    /// (this is RenameDialog.cs) and does not update any spots in the UI where the name
    /// of an item occurs (this is the responsibility of the caller).
    /// </summary>
    public class SafeRenamer
    {
        private readonly Scene _scene;

        public SafeRenamer(Scene scene)
        {
            _scene = scene;
        }

        public HashSet<string> GetAllNodeNames()
        {
            HashSet<string> names = new HashSet<string>();
            Action<Node> visitor = null;
            visitor = node =>
            {
                names.Add(node.Name);
                foreach (var c in node.Children)
                {
                    visitor(c);
                }
            };
            visitor(_scene.Raw.RootNode);
            return names;
        }

        public HashSet<string> GetAllMeshNames()
        {
            return new HashSet<string>(_scene.Raw.Meshes.Select(mesh => mesh.Name));
        }

        public HashSet<string> GetAllMaterialNames()
        {
            return new HashSet<string>(_scene.Raw.Materials.Select(material => material.Name));
        }

        public HashSet<string> GetAllAnimationNames()
        {
            return new HashSet<string>(_scene.Raw.Animations.Select(anim => anim.Name));
        }

        /// <summary>
        /// Rename node to the given new name.
        /// 
        /// Does not create UndoStack entries itself, caller must do this.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newName"></param>
        public void RenameNode(Node node, string newName)
        {
            if (newName == node.Name)
            {
                return;
            }

            var oldName = node.Name;
            node.Name = newName;

            // Update references from bones
            foreach (var mesh in _scene.Raw.Meshes)
            {
                foreach (var bone in mesh.Bones)
                {
                    if (bone.Name == oldName)
                    {
                        bone.Name = newName;
                    }
                }
            }

            // Update references from node animation channels
            foreach (var anim in _scene.Raw.Animations)
            {
                foreach (var channel in anim.NodeAnimationChannels)
                {
                    if (channel.NodeName == oldName)
                    {
                        channel.NodeName = newName;
                    }
                }
            }

            // SceneAnimator is - unfortunately - name based.
            _scene.SceneAnimator.RenameNode(oldName, newName);
        }

        /// <summary>
        /// Rename a mesh.
        /// 
        /// Does not create UndoStack entries itself, caller must do this.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="newName"></param>
        public void RenameMesh(Mesh mesh, string newName)
        {
            mesh.Name = newName;
        }

        /// <summary>
        /// Rename an animation.
        /// 
        /// Does not create UndoStack entries itself, caller must do this.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="newName"></param>
        public void RenameAnimation(Animation animation, string newName)
        {
            animation.Name = newName;
        }


        /// <summary>
        /// Rename a texture on disk, preserving the file extension.
        /// 
        /// Does not create UndoStack entries itself, caller must do this.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="newName"></param>
        public void RenameTexture(Texture texture, string newName)
        {
            string oldId = texture.OriginalTextureId;
            string oldLocation = texture.ActualLocation;
            string oldExt = Path.GetExtension(oldLocation);
            string newExt = Path.GetExtension(newName);

            if (oldExt != newExt)
            {
                newName = newName + oldExt;
            }

            newName = Path.GetDirectoryName(oldLocation) + Path.DirectorySeparatorChar + Path.GetFileName(newName);

            if (Path.GetFullPath(newName) == Path.GetFullPath(oldLocation))
            {
                return;
            }
            try
            {
                texture.Move(newName);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw ex;
            }
            catch (IOException ex)
            {
                // Just don't rename the texture. Likeliest case is the destination file already exists.
                // TODO(acgessler): Figure out how to best propagate this error.
            }
            string newId = texture.OriginalTextureId;
            _scene.TextureSet.Replace(oldId, newId);
            foreach (var mat in _scene.Raw.Materials)
            {
                foreach (var prop in mat.GetAllProperties())
                {
                    if (prop.GetStringValue() == oldId)
                    {
                        prop.SetStringValue(newId);
                    }
                }
            }
        }
    }
}
