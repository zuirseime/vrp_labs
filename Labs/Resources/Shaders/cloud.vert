#version 400 core

layout(location = 0) in vec3 aPosition;
layout(location = 2) in vec3 aNormal;

out vec3 Normal;
out vec3 WorldPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	Normal = normalize(mat3(transpose(inverse(model))) * aNormal);

    vec4 position = vec4(aPosition, 1.0) * model;
	WorldPos = aPosition.xyz;

	gl_Position = position * view * projection;
}