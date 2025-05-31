using OpenTK.Mathematics;

namespace TerraWeaver.TerraWeaverGraphics
{
    public struct DirectionalLight
    {
        public Vector3 Direction;
        public Vector3 Diffuse;
        public Vector3 Specular;

        public DirectionalLight(Vector3 direction, Vector3 diffuse, Vector3 specular)
        {
            Direction = direction;
            Diffuse = diffuse;
            Specular = specular;
        }
    }
}
