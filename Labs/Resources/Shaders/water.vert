#version 400 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;
layout(location = 3) in float aAO;

flat out vec2 TexCoord;
flat out float AO;
flat out vec3 Normal;
out vec3 WorldPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform float time;

uniform vec3 Scale;

uniform float waveAmplitude = 1;
uniform float waveFrequency = 0.05;
uniform vec2 waveDirection = vec2(1.0, 1.0);

void main() {
	float wave = sin(dot(aPosition.xz, waveDirection) * waveFrequency + time) * waveAmplitude;

	vec3 newPos = aPosition + vec3(0.0, wave, 0.0);

	TexCoord = aTexCoord;
	AO = aAO;

	Normal = normalize(mat3(transpose(inverse(model))) * aNormal);

	vec4 position = vec4(newPos, 1.0) * model;
	WorldPos = position.xyz;

	gl_Position = position * view * projection;
}