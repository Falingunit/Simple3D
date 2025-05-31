#version 330 core

struct TMaterial {
    sampler2D diffuse;
    sampler2D specular;
    sampler2D emissive;
    float shininess;
};

struct PointLight {
	vec3 position;
    vec3 diffuse;
    vec3 specular;

    float constant;
	float linear;
	float quadratic;
};

struct DirectionalLight {
	vec3 direction;
    vec3 diffuse;
    vec3 specular;
};

out vec4 fragColor;

in vec4 color;
in vec3 normal;
in vec3 fragPos;
in vec2 texCoord;

uniform vec3 cameraPos;

uniform TMaterial material;

uniform PointLight pointLights[16];
uniform int pointLightCount;
uniform DirectionalLight directionalLights[4];
uniform int directionalLightCount;
uniform vec3 ambientLight;
uniform float ambientIntensity;

vec3 calcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 Mdiffuse, vec3 Mspecular)
{
        float dist = length(light.position - fragPos);
        float attenuation = 1.0 / (light.constant + light.linear * dist + light.quadratic * (dist * dist));

        vec3 lightDirection = (light.position - fragPos) / dist;
        float clampedCosine = max(dot(normal, lightDirection), 0.0);
        vec3 diffuseLight = light.diffuse * Mdiffuse * clampedCosine;
        
        vec3 reflectDir = reflect(-lightDirection, normal);
        vec3 specularLight = light.specular * Mspecular * pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

        vec3 resultantLight = diffuseLight + specularLight;

        return resultantLight * attenuation;
}

vec3 calcDirLight(DirectionalLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec3 Mdiffuse, vec3 Mspecular)
{
        float clampedCosine = max(dot(normal, -light.direction), 0.0);
        vec3 diffuseLight = light.diffuse * Mdiffuse * clampedCosine;
        
        vec3 reflectDir = reflect(light.direction, normal);
        vec3 specularLight = light.specular * Mspecular * pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

        vec3 resultantLight = diffuseLight + specularLight;

        return resultantLight;
}


void main()
{
    vec3 netLight = vec3(0.0);
    vec3 viewDir = normalize(cameraPos - fragPos);
    vec3 Nnormal = normalize(normal);

    vec3 Mdiffuse = vec3(texture(material.diffuse, texCoord));
    vec3 Mspecular = vec3(texture(material.specular, texCoord));
	vec3 Memissive = vec3(texture(material.emissive, texCoord));

    //Calculate for all point lights
    for (int i = 0; i < pointLightCount; i++) 
    {
		netLight += calcPointLight(pointLights[i], Nnormal, fragPos, viewDir, Mdiffuse, Mspecular);
    }

    //Calculate for all directional lights
    for (int i = 0; i < directionalLightCount; i++) 
    {
        netLight += calcDirLight(directionalLights[i], Nnormal, fragPos, viewDir, Mdiffuse, Mspecular);
    }

    //Ambient
	netLight += ambientLight * ambientIntensity * Mdiffuse + Memissive;

    fragColor = vec4(netLight, 1.0);
}