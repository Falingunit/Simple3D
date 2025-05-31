using TerraWeaverGraphics;

namespace TerraWeaver.TerraWeaverGraphics.Model
{
    public struct TextureMaterial
    {
        public Texture Diffuse;
        public Texture Specular;
        public Texture Emissive;
        public float Shininess;

        public TextureMaterial(Texture diffuse, Texture specular, Texture emissive, float shininess)
        {
            this.Diffuse = diffuse;
            this.Specular = specular;
            this.Emissive = emissive;
            this.Shininess = shininess;
        }
    }
}
