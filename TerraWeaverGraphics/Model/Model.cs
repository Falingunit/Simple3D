using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaverGraphics;

namespace TerraWeaverGraphics.Model
{
	public abstract class Model
	{
		protected float[] _vertices;
		protected uint[] _indices;
		
		protected Shader _shader;
		protected int _vao, _vbo, _ebo;

		public Model() { }

		public Model(float[] vertices, uint[] indices, Shader shader)
		{
			_vertices = vertices;
			_indices = indices;
			_shader = shader;
		}

		public abstract void OnLoad();

		public virtual void UnloadModel()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.DeleteBuffer(_vbo);
			GL.DeleteVertexArray(_vao);
			GL.DeleteBuffer(_ebo);
		}

		public virtual void Draw(Matrix4 model)
		{
			_shader.Use();
			GL.UniformMatrix4(_shader.GetUniformLocation("modelMatrix"), true, ref model);
			GL.BindVertexArray(_vao);
			GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);
		}
	}

}
