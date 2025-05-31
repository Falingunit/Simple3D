using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaver.TerraWeaverGraphics.Model;

namespace TerraWeaverGraphics.Model
{
	public class VertexColorModel : Model
	{
		public Material Material { get; set; }

		public VertexColorModel(float[] vertices, uint[] indices, Shader shader) : base(vertices, indices, shader) { }

		public VertexColorModel(Vector3[] vertices, uint[] indices, Color4[] colors, Vector3[] normals, Shader shader) : base()
		{
            _vertices = new float[vertices.Length * 9];
            _indices = indices;
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i * 9] = vertices[i].X;
                _vertices[i * 9 + 1] = vertices[i].Y;
                _vertices[i * 9 + 2] = vertices[i].Z;
                _vertices[i * 9 + 3] = colors[i].R;
                _vertices[i * 9 + 4] = colors[i].G;
                _vertices[i * 9 + 5] = colors[i].B;

				var normal = normals[i].Normalized();
				_vertices[i * 9 + 6] = normal.X;
				_vertices[i * 9 + 7] = normal.Y;
                _vertices[i * 9 + 8] = normal.Z;
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
								   stride: 9 * sizeof(float),
								   offset: 0);
            //Color
            int colLocation = _shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(colLocation);
			GL.VertexAttribPointer(colLocation, 3, VertexAttribPointerType.Float, false,
								   stride: 9 * sizeof(float),
								   offset: 3 * sizeof(float));

			int normalLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false,
                                      stride: 9 * sizeof(float),
                                      offset: 6 * sizeof(float));

            //Setup ebo
            _ebo = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer,
						  _indices.Length * sizeof(uint),
						  _indices,
						  BufferUsageHint.StaticDraw);
		}

        public override void Draw(Matrix4 model)
        {
            _shader.SetMaterial(Material);
            _shader.SetModelMatrix(ref model);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }

}
