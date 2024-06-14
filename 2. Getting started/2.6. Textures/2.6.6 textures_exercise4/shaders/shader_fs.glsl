#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

uniform float mixValue;

// amostradores de textura
uniform sampler2D texture1;
uniform sampler2D texture2;

void main() {
	// interpolar linearmente entre ambas as texturas
    FragColor = mix(texture(texture1, TexCoord), texture(texture2, TexCoord), mixValue);
}
