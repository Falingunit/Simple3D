using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using TerraWeaver.TerraWeaverGraphics.Model;

namespace TerraWeaverGraphics.Model
{
    public class TextureModel : Model
    {
        public TextureMaterial TextureMaterial { get; set; }

        public TextureModel(float[] vertices, uint[] indices, Shader shader) : base(vertices, indices, shader)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public TextureModel(Vector3[] vertices, uint[] indices, Vector2[] textureCords, Vector3[] normals, Shader shader)
        {
            _vertices = new float[vertices.Length * 8];
            _indices = indices;
            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[i * 8] = vertices[i].X;
                _vertices[i * 8 + 1] = vertices[i].Y;
                _vertices[i * 8 + 2] = vertices[i].Z;
                _vertices[i * 8 + 3] = textureCords[i].X;
                _vertices[i * 8 + 4] = textureCords[i].Y;
                _vertices[i * 8 + 5] = normals[i].X;
                _vertices[i * 8 + 6] = normals[i].Y;
                _vertices[i * 8 + 7] = normals[i].Z;
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
                                   stride: 8 * sizeof(float),
                                   offset: 0);
            //Texture
            int texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false,
                                   stride: 8 * sizeof(float),
                                   offset: 3 * sizeof(float));

            int normalLocation = _shader.GetAttribLocation("aNormal");
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false,
                                   stride: 8 * sizeof(float),
                                   offset: 5 * sizeof(float));

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
            GL.BindVertexArray(_vao);

            _shader.Use();

            _shader.BindTextureUnits();
            _shader.SetTextureMaterial(TextureMaterial);
            _shader.SetModelMatrix(ref model);

            GL.UniformMatrix4(_shader.GetUniformLocation("modelMatrix"), true, ref model);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}