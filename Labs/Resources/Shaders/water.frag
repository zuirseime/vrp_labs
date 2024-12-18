#version 400 core

struct Light {
	int Type;
	vec3 Position;
	vec3 Direction;
	vec4 Color;
	float Intensity;
	float Radius;
	float Cutoff;
};

flat in vec2 TexCoord;
flat in float AO;
flat in vec3 Normal;
in vec3 WorldPos;

out vec4 FragColor;

uniform sampler2D Texture;

void main() {
	vec4 texColor = texture(Texture, TexCoord);
	if (texColor.a == 0.0) discard;
	
	FragColor = texColor;
}