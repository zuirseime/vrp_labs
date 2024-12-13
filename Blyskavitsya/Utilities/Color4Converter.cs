using OpenTK.Mathematics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blyskavitsya.Utilities;
internal class Color4Converter : JsonConverter<Color4>
{
    public override Color4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        float r = 1, g = 1, b = 1, a = 1;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Color4(r, g, b, a);

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string? propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "R":
                        r = (float)reader.GetDouble();
                        break;
                    case "G":
                        g = (float)reader.GetDouble();
                        break;
                    case "B":
                        b = (float)reader.GetDouble();
                        break;
                    case "A":
                        a = (float)reader.GetDouble();
                        break;
                }
            }
        }

        throw new JsonException("Invalid JSON format for Color4");
    }
    public override void Write(Utf8JsonWriter writer, Color4 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("R", value.R);
        writer.WriteNumber("G", value.G);
        writer.WriteNumber("B", value.B);
        writer.WriteNumber("A", value.A);
        writer.WriteEndObject();
    }
}
