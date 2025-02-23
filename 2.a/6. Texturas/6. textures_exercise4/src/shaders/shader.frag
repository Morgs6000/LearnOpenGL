#version 330 core
out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;

uniform float mixValue;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main() {
	// interpolar linearmente entre ambas as texturas (80% container, 20% awesomeface)
	FragColor = mix(texture(texture0, TexCoord), texture(texture1, TexCoord), mixValue);
}
