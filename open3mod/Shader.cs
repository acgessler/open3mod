///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [Shader.cs]
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


using OpenTK;

using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using System.Diagnostics;


namespace open3mod
{
    /// <summary>
    /// Represents a shader program that consists of a vertex and a fragment
    /// shader stage.
    /// </summary>
    public sealed class Shader : IDisposable
    {
        private readonly int _program;
        private readonly int _vs;
        private readonly int _fs;
        private bool _disposed;
        private readonly Dictionary<string, int> _variables = new Dictionary<string, int>(); 

        private static readonly Dictionary<string, string> _textCache = new Dictionary<string, string>();
        private static int _programBound = 0;

        /// <summary>
        /// Constructs a shader program given the source code for the single stages.
        /// 
        /// An exception will be thrown if constructing the Gl program fails.
        /// </summary>
        /// <param name="vertexShaderResName">Resource location for the source code for
        ///    the vertex shader stage</param>
        /// <param name="fragmentShaderResName">Resource location for the source code for 
        ///    the fragment shader stage</param>
        /// <param name="defines">(Preprocessor) stub to prepend to both stages. This
        ///    must be valid GLSL source code.</param>
        public static Shader FromResource(string vertexShaderResName, string fragmentShaderResName, string defines)
        {
            string vs, fs;

            var assembly = Assembly.GetExecutingAssembly();

            lock (_textCache)
            {
                if (!_textCache.TryGetValue(vertexShaderResName, out vs))
                {
                    var stream = assembly.GetManifestResourceStream(vertexShaderResName);
                    if (stream == null)
                    {
                        throw new Exception("failed to locate resource containing the vertex stage source code: " +
                                            vertexShaderResName);
                    }
                    using (var reader = new StreamReader(stream))
                    {
                        vs = reader.ReadToEnd();
                        _textCache.Add(vertexShaderResName, vs);
                    }
                }

                if (!_textCache.TryGetValue(fragmentShaderResName, out fs))
                {
                    var stream = assembly.GetManifestResourceStream(fragmentShaderResName);                
                    if (stream == null)
                    {
                        throw new Exception("failed to locate resource containing the fragment stage source code " +
                                            fragmentShaderResName);
                    }
                    using (var reader = new StreamReader(stream))
                    {
                        fs = reader.ReadToEnd();
                        _textCache.Add(fragmentShaderResName, fs);
                    }
                }
            }
            return new Shader(vs, fs, defines);
        }


        /// <summary>
        /// Constructs a shader program given the source code for the single stages.
        /// 
        /// An exception will be thrown if constructing the Gl program fails.
        /// </summary>
        /// <param name="vertexShader">Source code for the vertex shader stage</param>
        /// <param name="fragmentShader">Source code for the fragment shader stage</param>
        /// <param name="defines">(Preprocessor) stub to prepend to both stages. This
        ///    must be valid GLSL source code.</param>
        public Shader(string vertexShader, string fragmentShader, string defines)
        {
            _program = GL.CreateProgram();
            int result;

            _vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vs, string.Format("#version 130\n{0}\n{1}", defines, vertexShader));
            GL.CompileShader(_vs);
            GL.GetShader(_vs, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Debug.WriteLine(GL.GetShaderInfoLog(_vs));

                Dispose();
                throw new Exception("failed to compile vertex shader");
            }

            _fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fs, string.Format("#version 130\n{0}\n{1}", defines, fragmentShader));
            GL.CompileShader(_fs);
            GL.GetShader(_fs, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Debug.WriteLine(GL.GetShaderInfoLog(_fs));
                Dispose();
                throw new Exception("failed to compile fragment shader");
            }

            GL.AttachShader(Program, _vs);
            GL.AttachShader(Program, _fs);

            GL.LinkProgram(Program);
            GL.GetProgram(Program, ProgramParameter.LinkStatus, out result);
            if (result == 0)
            {
                Debug.WriteLine(GL.GetProgramInfoLog(Program));
                Dispose();
                throw new Exception("failed to link shader program");
            }
        }



        public int Program
        {
            get { return _program; }
        }



#if DEBUG
        ~Shader()
        {
            // OpenTk is unsafe from here, explicit Dispose() is required.
            Debug.Assert(false);
        }
#endif


        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (_program != 0)
            {
                GL.DeleteProgram(_program);
            }
            if (_vs != 0)
            {
                GL.DeleteShader(_vs);
            }
            if (_fs != 0)
            {
                GL.DeleteShader(_fs);
            }
            GC.SuppressFinalize(this);
        }

        public void SetVec3(string name, Vector3 v)
        {
            BindIfNecessary();
            GL.Uniform3(GetVariableLocation(name), v);
        }

        public void SetVec4(string name, Vector4 v)
        {
            BindIfNecessary();
            GL.Uniform4(GetVariableLocation(name), v);
        }

        public void SetMat4(string name, Matrix4 v)
        {
            BindIfNecessary();
            GL.UniformMatrix4(GetVariableLocation(name), false, ref v);
        }

        public void BindIfNecessary()
        {
            if (_programBound == _program)
            {
                return;
            }
            GL.UseProgram(_program);
            _programBound = _program;
        }

        public static void Unbind()
        {
            GL.UseProgram(0);
            _programBound = 0;
        }

        private int GetVariableLocation(string name)
        {
            if (!_variables.ContainsKey(name))
            {
                int location = GL.GetUniformLocation(_program, name);
                if (location == -1)
                {
                    throw new Exception("Failed to lookup variable: " + name);
                }
                _variables[name] = location;
            }
            return _variables[name];
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 