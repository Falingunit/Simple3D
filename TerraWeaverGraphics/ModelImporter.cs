using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TerraWeaver.TerraWeaverGraphics.Model;
using TerraWeaverGraphics;
using TerraWeaverGraphics.Model;

namespace TerraWeaver.TerraWeaverGraphics
{
	public class ModelImporter
	{
		private AssimpContext _context = new AssimpContext();

		public ModelImporter()
		{
			_context = new();
		}

		public VertexColorModel ImportVertexColorMesh(string filePath, Shader shader, float scale = 1)
		{
            var scene = _context.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals);

            var mesh = scene.Meshes[0];

            var vertices = new Vector3[mesh.VertexCount];
            var colors = new Color4[mesh.VertexCount];
            var normals = new Vector3[mesh.VertexCount];

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                vertices[i] = new Vector3(v.X, v.Y, v.Z);

                if (mesh.HasVertexColors(0))
                {
                    var c = mesh.VertexColorChannels[0][i];
                    colors[i] = new Color4(c.R, c.G, c.B, c.A);
                }
                else
                {
                    colors[i] = Color4.White;
                }

                var n = mesh.Normals[i];
                normals[i] = new Vector3(n.X, n.Y, n.Z);
            }

            var indices = mesh.GetIndices().Select(i => (uint)i).ToArray();

            // Extract material info
            var assimpMat = scene.Materials[mesh.MaterialIndex];

            Vector3 diffuse = assimpMat.HasColorDiffuse
                ? new Vector3(assimpMat.ColorDiffuse.R, assimpMat.ColorDiffuse.G, assimpMat.ColorDiffuse.B)
                : new Vector3(1f, 1f, 1f);

            Vector3 specular = assimpMat.HasColorSpecular
                ? new Vector3(assimpMat.ColorSpecular.R, assimpMat.ColorSpecular.G, assimpMat.ColorSpecular.B)
                : new Vector3(0f, 0f, 0f);

            Vector3 emissive = assimpMat.HasColorEmissive
                ? new Vector3(assimpMat.ColorEmissive.R, assimpMat.ColorEmissive.G, assimpMat.ColorEmissive.B)
                : new Vector3(0f, 0f, 0f);

            float roughness = assimpMat.HasReflectivity ? assimpMat.Reflectivity : 0.5f;
            float shininess = 1f / Math.Max(roughness, 0.01f);
            shininess = Math.Clamp(shininess * 32f, 1f, 256f);

            var model = new VertexColorModel(vertices, indices, colors, normals, shader)
            {
                Material = new Model.Material(diffuse, specular, emissive, shininess)
            };

            model.OnLoad();
            return model;
        }

        public TextureModel LoadTextureModel(string path, Shader shader, float scale = 1f)
        {
            var scene = _context.ImportFile(path, PostProcessSteps.Triangulate);

            var mesh = scene.Meshes[0];

            var vertices = new Vector3[mesh.VertexCount];
            var uvs = new Vector2[mesh.VertexCount];
            var normals = new Vector3[mesh.VertexCount];

            var scaleMatrix = Matrix3.CreateScale(scale);

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var v = mesh.Vertices[i];
                vertices[i] = scaleMatrix * new Vector3(v.X, v.Y, v.Z);

                if (mesh.HasTextureCoords(0))
                {
                    var uv = mesh.TextureCoordinateChannels[0][i];
                    uvs[i] = new Vector2(uv.X, uv.Y);
                }

                var n = mesh.Normals[i];
                normals[i] = new Vector3(n.X, n.Y, n.Z);
            }

            var indices = mesh.GetIndices().Select(i => (uint)i).ToArray();

            // Load material info
            var assimpMat = scene.Materials[mesh.MaterialIndex];
            string baseDir = Path.GetDirectoryName(path)!;

            Texture LoadTextureIfExists(TextureType type, TextureUnit unit)
            {
                if (assimpMat.GetMaterialTextureCount(type) > 0)
                {
                    if (assimpMat.GetMaterialTexture(type, 0, out TextureSlot slot))
                    {
                        string texPath = Path.Combine("Textures", Path.GetFileName(slot.FilePath));
                        return new Texture(texPath, unit);
                    }
                }
                return null!;
            }

            // Load diffuse texture
            Texture diffuse = LoadTextureIfExists(TextureType.Ambient, TextureUnit.Texture0)
                           ?? LoadTextureIfExists(TextureType.Diffuse, TextureUnit.Texture0);

            Texture specular = LoadTextureIfExists(TextureType.Specular, TextureUnit.Texture1) ?? diffuse;
            Texture emissive = diffuse;

            // Load material colors (optional, but can be used if blending with texture)
            Vector3 diffuseColor = assimpMat.HasColorDiffuse
                ? new Vector3(assimpMat.ColorDiffuse.R, assimpMat.ColorDiffuse.G, assimpMat.ColorDiffuse.B)
                : new Vector3(1f, 1f, 1f);

            Vector3 specularColor = assimpMat.HasColorSpecular
                ? new Vector3(assimpMat.ColorSpecular.R, assimpMat.ColorSpecular.G, assimpMat.ColorSpecular.B)
                : new Vector3(0f, 0f, 0f);

            Vector3 emissiveColor = assimpMat.HasColorEmissive
                ? new Vector3(assimpMat.ColorEmissive.R, assimpMat.ColorEmissive.G, assimpMat.ColorEmissive.B)
                : new Vector3(0f, 0f, 0f);

            // Scale shininess to match OpenGL shader expectations
            float roughness = assimpMat.HasReflectivity ? assimpMat.Reflectivity : 0.5f;
            float shininess = 1f / Math.Max(roughness, 0.01f);
            shininess = Math.Clamp(shininess * 32f, 1f, 256f);

            var textureMaterial = new TextureMaterial(diffuse, specular, emissive, shininess);

            TextureModel model = new TextureModel(vertices, indices, uvs, normals, shader);
            model.TextureMaterial = textureMaterial;

            model.OnLoad();
            return model;
        }

    }
}
