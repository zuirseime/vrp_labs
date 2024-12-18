#version 400 core

in vec3 WorldPos;

out vec4 FragColor;

uniform vec4 Top;
uniform vec4 Bottom;

void main() {
	float t = (normalize(WorldPos).y) / 1.5;
	t = t * 0.95 + 0.05;

	vec4 color = mix(Bottom, Top, t);

	FragColor = color;
}