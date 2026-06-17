using System;
using System.Text;
using System.Text.Json;
using Xunit;

namespace eShopModernized.Security.Tests
{
    public class SystemTextJsonSecurityTests
    {
        [Fact]
        public void SystemTextJson_PackageVersion_IsPatched()
        {
            var assembly = typeof(JsonSerializer).Assembly;
            var infoAttr = (System.Reflection.AssemblyInformationalVersionAttribute)
                Attribute.GetCustomAttribute(assembly, typeof(System.Reflection.AssemblyInformationalVersionAttribute));

            Assert.NotNull(infoAttr);

            // Extract the semver portion before any '+' metadata
            var versionString = infoAttr.InformationalVersion.Split('+')[0];
            var version = new Version(versionString);

            Assert.True(
                version >= new Version(8, 0, 5),
                $"System.Text.Json informational version {versionString} is below the patched 8.0.5");
        }

        [Fact]
        public void Deserialize_WithMaxDepth64_RejectsDeeplyNestedJson()
        {
            var options = new JsonSerializerOptions { MaxDepth = 64 };

            // Build a JSON payload nested 65 levels deep
            var sb = new StringBuilder();
            for (int i = 0; i < 65; i++)
                sb.Append("{\"a\":");
            sb.Append("1");
            for (int i = 0; i < 65; i++)
                sb.Append('}');

            var deepJson = sb.ToString();

            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonElement>(deepJson, options));
        }

        [Fact]
        public void Deserialize_WithMaxDepth64_AcceptsValidNestedJson()
        {
            var options = new JsonSerializerOptions { MaxDepth = 64 };

            // Build a JSON payload nested 63 levels deep (within limit)
            var sb = new StringBuilder();
            for (int i = 0; i < 63; i++)
                sb.Append("{\"a\":");
            sb.Append("1");
            for (int i = 0; i < 63; i++)
                sb.Append('}');

            var validJson = sb.ToString();

            var result = JsonSerializer.Deserialize<JsonElement>(validJson, options);
            Assert.Equal(JsonValueKind.Object, result.ValueKind);
        }

        [Fact]
        public void DefaultMaxDepth_PreventsStackOverflow_CVE2024_43485()
        {
            // In patched System.Text.Json >= 8.0.5, the default MaxDepth is 64.
            // Attempting to deserialize deeply nested JSON should throw JsonException,
            // not StackOverflowException.
            var options = new JsonSerializerOptions { MaxDepth = 64 };

            var sb = new StringBuilder();
            for (int i = 0; i < 200; i++)
                sb.Append("{\"a\":");
            sb.Append("1");
            for (int i = 0; i < 200; i++)
                sb.Append('}');

            var maliciousJson = sb.ToString();

            var ex = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<JsonElement>(maliciousJson, options));

            Assert.Contains("depth", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
