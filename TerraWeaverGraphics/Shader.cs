using OpenTK.Graphics.OpenGL4;

namespace TerraWeaverGraphics
{
	public class Shader : IDisposable
	{
		private int Handle;

        private string _vertexPath, _fragmentPath;

		public Shader(string vertexPath, string fragmentPath)
		{
            _vertexPath = vertexPath;
            _fragmentPath = fragmentPath;
		}

        public void OnLoad()
        {
            string vertexShaderSource = File.ReadAllText(_vertexPath);
            string fragmentShaderSource = File.ReadAllText(_fragmentPath);

            CompileShader(vertexShaderSource, fragmentShaderSource);
        }

		private void CompileShader(string VertexShaderSource, string FragmentShaderSource)
		{
            int VertexShader, FragmentShader;
            
            //Create shaders and set source to the read shaders
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            //Compile vertex shader and check for errors
            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            //Compile fragment shader and check for errors
            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

		public void Use()
		{
			GL.UseProgram(Handle);
		}

        public void SetInt(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

		public int GetAttribLocation(string attribName)
		{
			return GL.GetAttribLocation(Handle, attribName);
		}

		public int GetUniformLocation(string uniformName)
		{
			return GL.GetUniformLocation(Handle, uniformName);
		}

		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				GL.DeleteProgram(Handle);

				disposedValue = true;
			}
		}

		~Shader()
		{
			if (disposedValue == false)
			{
				Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
    }
}
