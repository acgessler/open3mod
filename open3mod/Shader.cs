using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        private static readonly Dictionary<string, string> _textCache = new Dictionary<string, string>(); 


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
            GL.ShaderSource(_vs, string.Format("{0}\n{1}", defines, vertexShader));
            GL.CompileShader(_vs);
            GL.GetShader(_vs, ShaderParameter.CompileStatus, out result);
            if (result == 0)
            {
                Debug.WriteLine(GL.GetShaderInfoLog(_vs));

                Dispose();
                throw new Exception("failed to compile vertex shader");
            }

            _fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fs, string.Format("{0}\n{1}", defines, fragmentShader));
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
    }
}
