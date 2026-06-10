using System.IO;
using System.Text.Json;

namespace eShopLegacy.Utilities
{
    public class Serializing
    {
        private static readonly JsonSerializerOptions s_options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = false
        };

        public Stream SerializeBinary(object input)
        {
            var stream = new MemoryStream();
            JsonSerializer.Serialize(stream, input, input.GetType(), s_options);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public object? DeserializeBinary(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.Deserialize<JsonElement>(stream, s_options);
        }
    }
}
