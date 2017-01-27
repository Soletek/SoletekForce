#version 330

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

in vec3 vPosition;
in vec4 vColor;
in vec2 vUV;

out vec4 fColor;
out vec2 fUV;

void main()
{
    gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vPosition, 1.0);
    fUV = vUV;
    fColor = vColor;
}
