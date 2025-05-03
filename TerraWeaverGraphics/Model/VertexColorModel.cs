using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TerraWeaverGraphics.Model
{
	public class VertexColorModel : Model
	{
		public VertexColorModel(float[] vertices, uint[] indices, Shader shader) : base(vertices, indices, shader) { }

		public VertexColorModel(Vector3[] vertices, uint[] indices, Color4[] colors, Shader shader) : base()
		{
            _vertices = new float[vertices.Length * 6];
            _indices = indices;
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i * 6] = vertices[i].X;
                _vertices[i * 6 + 1] = vertices[i].Y;
                _vertices[i * 6 + 2] = vertices[i].Z;
                _vertices[i * 6 + 3] = colors[i].R;
                _vertices[i * 6 + 4] = colors[i].G;
                _vertices[i * 6 + 5] = colors[i].B;
            }
			_shader = shader;
		}

		public override void OnLoad()
		{
			//Setup vbo
			_vbo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
			GL.BufferData(BufferTarget.ArrayBuffer,
						  _vertices.Length * sizeof(float),
						  _vertices,
						  BufferUsageHint.StaticDraw);

			//Setup vao
			_vao = GL.GenVertexArray();
			GL.BindVertexArray(_vao);

            //Position
            int posLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(posLocation);
			GL.VertexAttribPointer(posLocation, 3, VertexAttribPointerType.Float, false,
								   stride: 6 * sizeof(float),
								   offset: 0);
            //Color
            int colLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colLocation);
			GL.VertexAttribPointer(colLocation, 3, VertexAttribPointerType.Float, false,
								   stride: 6 * sizeof(float),
								   offset: 3 * sizeof(float));

			//Setup ebo
			_ebo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer,
						  _indices.Length * sizeof(uint),
						  _indices,
						  BufferUsageHint.StaticDraw);
		}
	}

}
