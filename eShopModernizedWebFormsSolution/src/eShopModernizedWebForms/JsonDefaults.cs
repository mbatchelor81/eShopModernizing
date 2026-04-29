using System.Text.Json;

namespace eShopModernizedWebForms
{
    public static class JsonDefaults
    {
        public static JsonSerializerOptions SerializerOptions { get; } = new JsonSerializerOptions
        {
            MaxDepth = 64
        };
    }
}
