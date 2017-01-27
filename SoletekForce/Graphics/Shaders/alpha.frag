#version 330

uniform sampler2D texture;
uniform float alpha;

in vec4 fColor; // must match name in vertex shader
in vec2 fUV;

out vec4 fragColor; // first out variable is automatically written to the screen

void main()
{
    fragColor = texture2D(texture, fUV.xy) * fColor * vec4(1, 1, 1, 1 - alpha);
    if (fragColor.a < 0.003) discard;
}
