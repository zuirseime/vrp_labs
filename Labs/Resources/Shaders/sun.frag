#version 400 core

in vec3 WorldPos;

out vec4 FragColor;

uniform vec4 Color;
uniform float Intensity;

uniform float Radius = 1;
uniform float Cutoff = 3;

uniform float intensityFactor = 0.00002;

void main() {
	float distance = length(WorldPos.xy) / Radius;

	float alpha = exp(-Cutoff * pow(distance, 2));

	vec4 color = Color * alpha;
	color.a = alpha;
	FragColor = Color * Intensity * intensityFactor;
}