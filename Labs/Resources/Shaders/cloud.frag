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

in vec3 Normal;
in vec3 WorldPos;

out vec4 FragColor;

uniform vec4 Color;
uniform vec4 fogColor;
uniform float renderDistance;

uniform vec3 Camera;

uniform Light lights[32];
uniform int lightCount;

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

void main()
{
    if (Color.a < 0.01)
        discard;

	vec4 light = vec4(0.0);

	for (int i = 0; i < lightCount; i++) {
		if (lights[i].Position.y >= 0) {
			if (lights[i].Type == 0)
				light += calculateDirectionalLight(lights[i], Normal);
			else if (lights[i].Type == 1)
				light += calculatePointLight(lights[i], WorldPos, Normal);
		}
	}

	vec3 color = fogColor.rgb + light.rgb;

    FragColor = vec4(color.rgb, 0.9);
}
