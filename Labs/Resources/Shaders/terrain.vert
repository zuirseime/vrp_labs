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

uniform vec3 Scale;

void main() {
	TexCoord = aTexCoord;
	AO = aAO;

	Normal = normalize(mat3(transpose(inverse(model))) * aNormal);

	vec4 position = vec4(aPosition, 1.0) * model;
	WorldPos = position.xyz;

	gl_Position = position * view * projection;
}