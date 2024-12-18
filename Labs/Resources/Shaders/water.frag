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

vec4 calculateDirectionalLight(Light light, vec3 normal) {
	float diff = max(dot(normal, normalize(-light.Direction)), 0.0);

	return light.Color * diff * light.Intensity;
}

vec4 calculatePointLight(Light light, vec3 fragPos, vec3 normal) {
	vec3 lightDir = normalize(light.Position - fragPos);
	float diff = max(dot(normal, lightDir), 0.0);

	float distance = length(light.Position - fragPos);
	float attenuation = 1.0 / (1.0 + 0.09 * distance + 0.032 * pow(distance, 2.0));

	return light.Color * diff * light.Intensity * attenuation;
}

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

	vec4 light = vec4(0.0);

	for (int i = 0; i < lightCount; i++) {
		if (lights[i].Type == 0)
			light += calculateDirectionalLight(lights[i], Normal);
		else if (lights[i].Type == 1)
			light += calculatePointLight(lights[i], WorldPos, Normal);
	}
	
	newColor.rgb *= light.rgb;
	
	vec4 finalColor = mix(fogColor, newColor, fogFactor);
	
	FragColor = finalColor;
}