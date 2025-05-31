using OpenTK.Mathematics;

namespace TerraWeaver.TerraWeaverGraphics.Model
{
    public struct Material
    {
        public Vector3 Diffuse;
        public Vector3 Specular;
        public Vector3 Emissive;
        public float Shininess;

        public Material(Vector3 diffuse, Vector3 specular, Vector3 emissive, float shininess)
        {
            this.Diffuse = diffuse;
            this.Specular = specular;
            this.Emissive = emissive;
            this.Shininess = shininess;
        }
    }
}
