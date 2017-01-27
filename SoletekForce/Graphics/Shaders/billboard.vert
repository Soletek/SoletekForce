#version 330

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;
uniform float scaleX;
uniform float scaleY;

in vec3 vPosition;
in vec4 vColor;
in vec2 vUV;

out vec4 fColor;
out vec2 fUV;

void main()
{
	mat4 modelView = viewMatrix * modelMatrix;

	modelView[0][0] = 1.0 * scaleX;
	modelView[0][1] = 0.0;
	modelView[0][2] = 0.0;

	modelView[1][0] = 0.0;
	modelView[1][1] = 1.0 * scaleY;
	modelView[1][2] = 0.0;

	modelView[2][0] = 0.0;
	modelView[2][1] = 0.0;
	modelView[2][2] = 1.0;
	
    gl_Position = projectionMatrix * modelView * vec4(vPosition, 1.0);
    fUV = vUV;
    fColor = vColor;
}
