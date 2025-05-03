using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace TerraWeaverGraphics.Model
{
    internal class TextureModel : Model
    {
        private Texture _texture;

        public Texture Texture
        {
            get => _texture;
            set
            {
                _texture = value;
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _texture.Handle);
            }
        }   

        public TextureModel(float[] vertices, uint[] indices, Shader shader, Texture texture) : base(vertices, indices, shader)
        {
            _vertices = vertices;
            _indices = indices;
            _texture = texture;
        }

        public TextureModel(Vector3[] vertices, uint[] indices, Vector2[] textureCords, Shader shader, Texture texture) : base()
        {
            _vertices = new float[vertices.Length * 5];
            _indices = indices;
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i * 5] = vertices[i].X;
                _vertices[i * 5 + 1] = vertices[i].Y;
                _vertices[i * 5 + 2] = vertices[i].Z;
                _vertices[i * 5 + 3] = textureCords[i].X;
                _vertices[i * 5 + 4] = textureCords[i].Y;
            }
            _texture = texture;
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
                                   stride: 5 * sizeof(float),
                                   offset: 0);
            //Texture
            int texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false,
                                   stride: 5 * sizeof(float),
                                   offset: 3 * sizeof(float));

            //Setup ebo
            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                          _indices.Length * sizeof(uint),
                          _indices,
                          BufferUsageHint.StaticDraw);

            _texture.Use();
        }

        public override void Draw(Matrix4 model)
        {
            GL.BindVertexArray(_vao);

            _texture.Use();
            _shader.Use();

            GL.UniformMatrix4(_shader.GetUniformLocation("modelMatrix"), true, ref model);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}