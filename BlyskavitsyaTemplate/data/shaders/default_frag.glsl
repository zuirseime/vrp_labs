#version 400 core

in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

out vec4 Color;

struct Map {
	sampler2D tex;
	vec4 color;
};

struct Material {
	Map diffuse;
	Map specular;
	Map emission;
	float shininess;
};

uniform Material material;
uniform vec3 viewPos;

void main() {
	vec4 diffuse = material.diffuse.color * vec4(texture(material.diffuse.tex, TexCoord));

	vec3 viewDirection = normalize(viewPos - FragPos);
	vec3 reflectDirection = reflect(-viewDirection, normalize(Normal));
	float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), material.shininess);
	vec4 specular = material.specular.color * spec * vec4(texture(material.specular.tex, TexCoord));

	vec4 emission = material.emission.color * vec4(texture(material.emission.tex, TexCoord));

	Color = diffuse + specular + emission;
	//Color = diffuse;
}