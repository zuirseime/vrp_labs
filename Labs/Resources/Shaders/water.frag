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
uniform vec4 Color;

uniform Light lights[32];
uniform int lightCount;

uniform float Opacity;

uniform vec3 Camera;
uniform vec4 fogColor = vec4(0.5, 0.6, 0.8, 1);
uniform float renderDistance;
uniform float density = 2;
uniform float heightFalloff = 0.3;

float getFogFactor(float distance) {
	
	float height = WorldPos.y;

	float distRatio = distance / renderDistance;
	float distFactor = exp(-pow(distRatio * density, 2.0));

	return clamp(distFactor, 0.0, 1.0);
}

float getAlphaFade(float distance) {
	if (distance > renderDistance) {
		return clamp(1.0 - (distance - renderDistance) / (renderDistance * 0.1), 0.0, 1.0);
	}
	return 1.0;
}

void main() {
	vec4 texColor = texture(Texture, TexCoord);
	if (texColor.a == 0.0) discard;
	
	vec4 newColor = Color * texColor;
	
	newColor.a *= Opacity;
	
	float distance = length(WorldPos - Camera);
	float fogFactor = getFogFactor(distance);
	
    if (AO > 0) {
		newColor.rgb *= AO;
	}
	
	vec4 finalColor = mix(fogColor, newColor, fogFactor);
	
	FragColor = finalColor;
}