#version 330

uniform sampler2D texture;
uniform float id;

in vec2 fUV;

out vec4 fragID;

void main()
{
	int idv = int(floor(id + 0.5));
    fragID = vec4(float(idv % 256) / 255.0, float(idv >> 8) / 255.0, 0, 0);
    if (texture2D(texture, fUV.xy).a < 0.5) discard;
}
