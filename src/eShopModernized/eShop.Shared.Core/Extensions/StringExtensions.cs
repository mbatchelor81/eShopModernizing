namespace eShop.Shared.Core.Extensions;

public static class StringExtensions
{
    public static string ToSlug(this string value)
    {
        return value
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "-");
    }

    public static bool IsNullOrWhiteSpace(this string? value) =>
        string.IsNullOrWhiteSpace(value);
}
