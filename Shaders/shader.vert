#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

out vec4 vcol;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vcol = vec4(aColor, 1.0);
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}