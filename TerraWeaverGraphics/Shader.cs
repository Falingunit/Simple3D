using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaver.TerraWeaverGraphics;
using TerraWeaver.TerraWeaverGraphics.Model;
using Material = TerraWeaver.TerraWeaverGraphics.Model.Material;

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

        public void SetMaterial(Material material)
        {
            Use();
            GL.Uniform3(GetUniformLocation("material.diffuse"), material.Diffuse);
            GL.Uniform3(GetUniformLocation("material.specular"), material.Specular);
            GL.Uniform3(GetUniformLocation("material.emissive"), material.Emissive);
            GL.Uniform1(GetUniformLocation("material.shininess"), material.Shininess);
        }

        public void SetVector(string name, Vector3 vector)
        {
            Use();
            GL.Uniform3(GetUniformLocation(name), vector);
        }

        public void SetFloat(string name, float value)
        {
            Use();
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

        public void SetModelMatrix(ref Matrix4 model)
        {
            Use();
            GL.UniformMatrix4(GetUniformLocation("modelMatrix"), true, ref model);
        }

        public void SetPointLight(int i, PointLight pointLight)
        {
            Use();
            GL.Uniform3(GetUniformLocation($"pointLights[{i}].position"), pointLight.Position);
            GL.Uniform3(GetUniformLocation($"pointLights[{i}].diffuse"), pointLight.Diffuse);
            GL.Uniform3(GetUniformLocation($"pointLights[{i}].specular"), pointLight.Specular);
            GL.Uniform1(GetUniformLocation($"pointLights[{i}].constant"), pointLight.Constant);
            GL.Uniform1(GetUniformLocation($"pointLights[{i}].linear"), pointLight.Linear);
            GL.Uniform1(GetUniformLocation($"pointLights[{i}].quadratic"), pointLight.Quadratic);
        }
        public void SetDirectionalLight(int i, DirectionalLight directionalLight)
        {
            Use();
            GL.Uniform3(GetUniformLocation($"directionalLights[{i}].direction"), directionalLight.Direction);
            GL.Uniform3(GetUniformLocation($"directionalLights[{i}].diffuse"), directionalLight.Diffuse);
            GL.Uniform3(GetUniformLocation($"directionalLights[{i}].specular"), directionalLight.Specular);
        }


        public void SetTextureMaterial(TextureMaterial textureMaterial)
        {
            Use();
            textureMaterial.Diffuse.Use(TextureUnit.Texture0);
            textureMaterial.Specular.Use(TextureUnit.Texture1);
            textureMaterial.Emissive.Use(TextureUnit.Texture2);
            GL.Uniform1(GetUniformLocation("material.shininess"), textureMaterial.Shininess);
        }

        public void BindTextureUnits()
        {
            Use();
            GL.Uniform1(GetUniformLocation("material.diffuse"), 0);
            GL.Uniform1(GetUniformLocation("material.specular"), 1);
            GL.Uniform1(GetUniformLocation("material.emissive"), 2);
        }
    }
}
