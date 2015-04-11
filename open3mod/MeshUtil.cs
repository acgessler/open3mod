using System.Collections.Generic;
using Assimp;
using Assimp.Unmanaged;

namespace open3mod
{
    /// <summary>
    /// Utilities supporting deep and shallow copies of meshes.
    /// 
    /// Generally, we avoid creating new Mesh instances. Many places in the application
    /// retain references to meshes, so replacing Mesh instances is dangerous. It's
    /// preferred to swap out the contents of the mesh.
    /// </summary>
    public static class MeshUtil
    {       
        public static void DeepCopy(Mesh dest, Mesh src)
        {
            ShallowCopy(dest, src);
            dest.Vertices = new List<Vector3D>(src.Vertices);
            dest.Normals = new List<Vector3D>(src.Normals);
            dest.Tangents = new List<Vector3D>(src.Tangents);
            dest.BiTangents = new List<Vector3D>(src.BiTangents);
            dest.TextureCoordinateChannels = new List<Vector3D>[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];
            for (int i = 0; i < AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS; ++i)
            {
                dest.TextureCoordinateChannels[i] = new List<Vector3D>(src.TextureCoordinateChannels[i]);
            }
            dest.VertexColorChannels = new List<Color4D>[AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS];
            for (int i = 0; i < AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS; ++i)
            {
                dest.VertexColorChannels[i] = new List<Color4D>(src.VertexColorChannels[i]);
            }
            dest.Faces = new List<Face>(src.Faces);
            for (int i = 0; i < dest.Faces.Count; ++i)
            {
                dest.Faces[i] = new Face(dest.Faces[i].Indices.ToArray());
            }
            // TODO(acgessler): Handle bones.
        }

        public static void ShallowCopy(Mesh dest, Mesh src)
        {
            GenericShallowCopy.Copy(dest, src);
        }

        public static Mesh DeepCopy(Mesh src)
        {
            var mesh = new Mesh();
            DeepCopy(mesh, src);
            return mesh;
        }

        public static Mesh ShallowCopy(Mesh src)
        {
            var mesh = new Mesh();
            ShallowCopy(mesh, src);
            return mesh;
        }

        public static void ClearMesh(Mesh mesh)
        {
            mesh.Vertices.Clear();
            mesh.Normals.Clear();
            mesh.Tangents.Clear();
            mesh.BiTangents.Clear();
            for (int i = 0; i < AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS; ++i)
            {
                mesh.TextureCoordinateChannels[i].Clear();
            }
            for (int i = 0; i < AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS; ++i)
            {
                mesh.VertexColorChannels[i].Clear();
            }
            mesh.Faces.Clear();
            mesh.PrimitiveType = 0;
        }
    }
}
