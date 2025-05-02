#version 330 core
out vec4 FragColor;
in vec4 vcol;

void main()
{
    FragColor = vcol;
}