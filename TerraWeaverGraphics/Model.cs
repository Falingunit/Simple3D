using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaverGraphics;

namespace TerraWeaver.TerraWeaverGraphics
{
	public class Model
	{
		public float[] Vertices;
		public uint[] Indices;

		private int _vao, _vbo, _ebo;

		public Model(float[] vertices, uint[] indices)
		{
			Vertices = vertices;
			Indices = indices;
		}

		public void OnLoad()
		{
			//Setup vbo
			_vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
			GL.BufferData(BufferTarget.ArrayBuffer,
						  Vertices.Length * sizeof(float),
						  Vertices,
						  BufferUsageHint.StaticDraw);


			//Setup vao
			_vao = GL.GenVertexArray();
			GL.BindVertexArray(_vao);
			//Position
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
								   stride: 6 * sizeof(float),
								   offset: 0);
			//Color
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false,
								   stride: 6 * sizeof(float),
								   offset: 3 * sizeof(float));

			//Setup ebo
			_ebo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer,
						  Indices.Length * sizeof(uint),
						  Indices,
						  BufferUsageHint.StaticDraw);
		}

		public void UnloadModel()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.DeleteBuffer(_vbo);
			GL.DeleteVertexArray(_vao);
			GL.DeleteBuffer(_ebo);
		}

		public void Draw(Shader shader, Matrix4 model)
		{
			shader.Use();
			GL.UniformMatrix4(shader.GetUniformLocation("model"), true, ref model);
			GL.BindVertexArray(_vao);
			GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);
		}
	}

}
