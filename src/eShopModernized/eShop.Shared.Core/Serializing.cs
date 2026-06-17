using System.Text.Json;

namespace eShop.Shared.Core;

/// <summary>
/// Modern replacement for the legacy BinaryFormatter-based serializer.
/// Uses System.Text.Json for safe, high-performance serialization.
/// </summary>
public class Serializing
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public Stream SerializeJson<T>(T input, JsonSerializerOptions? options = null)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, input, options ?? DefaultOptions);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public T? DeserializeJson<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        stream.Seek(0, SeekOrigin.Begin);
        return JsonSerializer.Deserialize<T>(stream, options ?? DefaultOptions);
    }
}
