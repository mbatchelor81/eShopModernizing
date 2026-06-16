using eShop.Shared.Core;
using Xunit;

namespace eShop.Catalog.Tests;

public class SerializingTests
{
    private readonly Serializing _serializer = new();

    [Fact]
    public void SerializeJson_And_DeserializeJson_RoundTrip()
    {
        var input = new TestItem { Id = 1, Name = "Test" };

        using var stream = _serializer.SerializeJson(input);
        var result = _serializer.DeserializeJson<TestItem>(stream);

        Assert.NotNull(result);
        Assert.Equal(input.Id, result!.Id);
        Assert.Equal(input.Name, result.Name);
    }

    [Fact]
    public void DeserializeJson_EmptyStream_ReturnsNull()
    {
        using var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write("null");
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        var result = _serializer.DeserializeJson<TestItem>(stream);

        Assert.Null(result);
    }

    private class TestItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
