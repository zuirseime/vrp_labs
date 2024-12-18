#version 400 core

layout(location = 0) in vec3 aPosition;

out vec3 WorldPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 Scale;

void main() {
	vec4 position = vec4(aPosition, 1.0) * model;
	WorldPos = position.xyz;

	gl_Position = position * view * projection;
}