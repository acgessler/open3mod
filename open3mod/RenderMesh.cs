///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [RenderMesh.cs]
// (c) 2012-2015, Open3Mod Contributors
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

using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace open3mod
{
    /// <summary>
    /// Mesh rendering using VBOs.
    /// 
    /// Based on http://www.opentk.com/files/T08_VBO.cs
    /// </summary>
    public class RenderMesh
    {
        private readonly Mesh _mesh;
        struct Vbo
        {
            public int VertexBufferId;
            public int ColorBufferId;
            public int TexCoordBufferId;
            public int NormalBufferId;
            public int TangentBufferId;
            public int ElementBufferId;
            public int NumIndices;
            public int BitangentBufferId;
            public bool Is32BitIndices; 
        }

        private readonly Vbo _vbo;


        /// <summary>
        /// Constructs a RenderMesh for a given assimp mesh and uploads the
        /// required data to the GPU.
        /// </summary>
        /// <param name="mesh"></param>
        /// <exception cref="Exception">When any Gl errors occur during uploading</exception>
        public RenderMesh(Mesh mesh)
        {
            Debug.Assert(mesh != null);

            _mesh = mesh;
            Upload(out _vbo);
        }


        /// <summary>
        /// Draws the mesh geometry given the current pipeline state. 
        /// 
        /// The pipeline is restored afterwards.
        /// </summary>
        /// <param name="flags">Rendering mode</param>
        public void Render(RenderFlags flags)
        {
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            Debug.Assert(_vbo.VertexBufferId != 0);
            Debug.Assert(_vbo.ElementBufferId != 0);

            // normals
            if (flags.HasFlag(RenderFlags.Shaded))
            {
                if (_vbo.NormalBufferId != 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo.NormalBufferId);
                    GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
                    GL.EnableClientState(ArrayCap.NormalArray);
                }
            }

            // vertex colors
            if (_vbo.ColorBufferId != 0)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo.ColorBufferId);
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(int), IntPtr.Zero);
                GL.EnableClientState(ArrayCap.ColorArray);
            }

            // UV coordinates
            if (flags.HasFlag(RenderFlags.Textured))
            {
                if (_vbo.TexCoordBufferId != 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo.TexCoordBufferId);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 8, IntPtr.Zero);
                    GL.EnableClientState(ArrayCap.TextureCoordArray);
                }
            }


            // vertex position
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo.VertexBufferId);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.VertexArray);


            // primitives
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vbo.ElementBufferId);
            GL.DrawElements(BeginMode.Triangles, _vbo.NumIndices /* actually, count(indices) */,
                _vbo.Is32BitIndices ? DrawElementsType.UnsignedInt : DrawElementsType.UnsignedShort,
                IntPtr.Zero);

            // Restore the state
            GL.PopClientAttrib();
        }


        /// <summary>
        /// Currently only called during construction, this method uploads the input mesh (
        /// the RenderMesh instance is bound to) to a VBO.
        /// </summary>
        /// <param name="vboToFill"></param>
        private void Upload(out Vbo vboToFill)
        {
            vboToFill = new Vbo();     
      
            UploadVertices(out vboToFill.VertexBufferId);
            if (_mesh.HasNormals)
            {
                UploadNormals(out vboToFill.NormalBufferId);
            }

            if (_mesh.HasVertexColors(0))
            {
                UploadColors(out vboToFill.ColorBufferId);
            }

            if (_mesh.HasTextureCoords(0))
            {
                UploadTextureCoords(out vboToFill.TexCoordBufferId);
            }

            if (_mesh.HasTangentBasis)
            {
                UploadTangentsAndBitangents(out vboToFill.TangentBufferId, out vboToFill.BitangentBufferId);
            }

            UploadPrimitives(out vboToFill.ElementBufferId, out vboToFill.NumIndices, out vboToFill.Is32BitIndices);
            // TODO: upload bone weights
        }


        /// <summary>
        /// Generates and populates an Gl vertex array buffer given 3D vectors as source data
        /// </summary>
        private void GenAndFillBuffer(out int outGlBufferId, List<Vector3D> dataBuffer) 
        {
            GL.GenBuffers(1, out outGlBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, outGlBufferId);

            var byteCount = dataBuffer.Count * 12;
            var temp = new float[byteCount];

            var n = 0;
            foreach(var v in dataBuffer)
            {
                temp[n++] = v.X;
                temp[n++] = v.Y;
                temp[n++] = v.Z;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (byteCount), temp, BufferUsageHint.StaticDraw);

            VerifyBufferSize(byteCount);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        /// <summary>
        /// Verifies that the size of the currently bound vertex array buffer matches
        /// a given parameter and throws if it doesn't.
        /// </summary>
// ReSharper disable UnusedParameter.Local
        private void VerifyBufferSize(int byteCount)
// ReSharper restore UnusedParameter.Local
        {
            int bufferSize;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (byteCount != bufferSize)
            {
                throw new Exception("Vertex data array not uploaded correctly - buffer size does not match upload size");
            }
        }


        /// <summary>
        /// Uploads vertex indices to a newly generated Gl vertex array
        /// </summary>
        private void UploadPrimitives(out int elementBufferId, out int indicesCount, out bool is32Bit)
        {
            Debug.Assert(_mesh.HasTextureCoords(0));

            GL.GenBuffers(1, out elementBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferId);

            var faces = _mesh.Faces;

            // TODO account for other primitives than triangles
            var triCount = 0;
            int byteCount;
            is32Bit = false;
            foreach(var face in faces)
            {
                if (face.IndexCount != 3)
                {
                    continue;
                }
                ++triCount;
                if (face.Indices.Any(idx => idx > 0xffff))
                {
                    is32Bit = true;
                }
            }

            var intCount = triCount * 3;
            if (is32Bit)
            {
                var temp = new uint[intCount];
                byteCount = intCount * sizeof(uint);
                var n = 0;
                foreach (var idx in faces.Where(face => face.IndexCount == 3).SelectMany(face => face.Indices))
                {
                    temp[n++] = (uint)idx;
                }

                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)byteCount,
                    temp, BufferUsageHint.StaticDraw);
            }
            else
            {
                var temp = new ushort[intCount];
                byteCount = intCount * sizeof(ushort);
                var n = 0;
                foreach (var idx in faces.Where(face => face.IndexCount == 3).SelectMany(face => face.Indices))
                {
                    Debug.Assert(idx <= 0xffff);
                    temp[n++] = (ushort)idx;
                }

                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)byteCount, 
                    temp, BufferUsageHint.StaticDraw);
            }

            int bufferSize;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (byteCount != bufferSize)
            {
                throw new Exception("Index data array not uploaded correctly - buffer size does not match upload size");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            indicesCount = triCount * 3;
        }


        /// <summary>
        /// Uploads UV coordinates to a newly generated Gl vertex array.
        /// </summary>
        private void UploadTextureCoords(out int texCoordBufferId)
        {
            Debug.Assert(_mesh.HasTextureCoords(0));

            GL.GenBuffers(1, out texCoordBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordBufferId);

            var uvs = _mesh.TextureCoordinateChannels[0];
            var floatCount = uvs.Count * 2;
            var temp = new float[floatCount];
            var n = 0;
            foreach (var uv in uvs)
            {
                temp[n++] = uv.X;
                temp[n++] = uv.Y;
            }

            var byteCount = floatCount*sizeof (float);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(byteCount), temp, BufferUsageHint.StaticDraw);
            VerifyBufferSize(byteCount);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }


        /// <summary>
        /// Uploads vertex positions to a newly generated Gl vertex array.
        /// </summary>
        private void UploadVertices(out int verticesBufferId)
        {
            GenAndFillBuffer(out verticesBufferId, _mesh.Vertices);
        }


        /// <summary>
        /// Uploads normal vectors to a newly generated Gl vertex array.
        /// </summary>
        private void UploadNormals(out int normalBufferId)
        {
            Debug.Assert(_mesh.HasNormals);
            GenAndFillBuffer(out normalBufferId, _mesh.Normals);
        }


        /// <summary>
        /// Uploads tangents and bitangents to newly generated Gl vertex arrays.
        /// </summary>
        private void UploadTangentsAndBitangents(out int tangentBufferId, out int bitangentBufferId)
        {
            Debug.Assert(_mesh.HasTangentBasis);

            var tangents = _mesh.Tangents;
            GenAndFillBuffer(out tangentBufferId, tangents);

            var bitangents = _mesh.BiTangents;
            Debug.Assert(bitangents.Count == tangents.Count);

            GenAndFillBuffer(out bitangentBufferId, bitangents);
        }


        /// <summary>
        /// Uploads vertex colors to a newly generated Gl vertex array.
        /// </summary>
        /// <param name="colorBufferId"></param>
        private void UploadColors(out int colorBufferId)
        {
            Debug.Assert(_mesh.HasVertexColors(0));

            GL.GenBuffers(1, out colorBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferId);

            var colors = _mesh.VertexColorChannels[0];
            // convert to 32Bit RGBA
            var byteCount = colors.Count*4;
            var byteColors = new byte[byteCount];
            var n = 0;
            foreach(var c in colors)
            {
                byteColors[n++] = (byte)(c.R * 255);
                byteColors[n++] = (byte)(c.G * 255);
                byteColors[n++] = (byte)(c.B * 255);
                byteColors[n++] = (byte)(c.A * 255);
            }

            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(byteCount), byteColors, BufferUsageHint.StaticDraw);

            int bufferSize;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (byteCount != bufferSize)
            {
                throw new Exception("Vertex array not uploaded correctly");
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }       
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 