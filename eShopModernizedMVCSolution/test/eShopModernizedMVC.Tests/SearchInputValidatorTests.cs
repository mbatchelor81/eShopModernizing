using eShopModernizedMVC.Services;
using Xunit;

namespace eShopModernizedMVC.Tests
{
    public class SearchInputValidatorTests
    {
        [Theory]
        [InlineData("shirt", true)]
        [InlineData("Azure T-Shirt", false)]
        [InlineData("laptop bag", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValid_BasicInputs(string searchTerm, bool expected)
        {
            Assert.Equal(expected, SearchInputValidator.IsValid(searchTerm));
        }

        [Theory]
        [InlineData("'; DROP TABLE CatalogItems; --")]
        [InlineData("' OR '1'='1")]
        [InlineData("1; DELETE FROM Catalog --")]
        [InlineData("' UNION SELECT * FROM users --")]
        [InlineData("'; EXEC xp_cmdshell('dir') --")]
        [InlineData("1' OR 1=1 --")]
        [InlineData("admin'--")]
        public void IsValid_RejectsSqlInjectionPayloads(string payload)
        {
            Assert.False(SearchInputValidator.IsValid(payload));
        }

        [Theory]
        [InlineData("'; DROP TABLE CatalogItems; --", " DROP TABLE CatalogItems ")]
        [InlineData("' OR '1'='1", " OR 1=1")]
        [InlineData("normal search", "normal search")]
        [InlineData("test*item", "testitem")]
        [InlineData("hello\\world", "helloworld")]
        public void Sanitize_RemovesSqlMetaCharacters(string input, string expected)
        {
            Assert.Equal(expected, SearchInputValidator.Sanitize(input));
        }

        [Fact]
        public void Sanitize_TruncatesLongInput()
        {
            var longInput = new string('a', 300);
            var result = SearchInputValidator.Sanitize(longInput);
            Assert.Equal(SearchInputValidator.MaxSearchTermLength, result.Length);
        }

        [Fact]
        public void Sanitize_ReturnsEmptyForNullOrWhitespace()
        {
            Assert.Equal(string.Empty, SearchInputValidator.Sanitize(null));
            Assert.Equal(string.Empty, SearchInputValidator.Sanitize(""));
            Assert.Equal(string.Empty, SearchInputValidator.Sanitize("   "));
        }

        [Fact]
        public void Sanitize_TrimsWhitespace()
        {
            Assert.Equal("test", SearchInputValidator.Sanitize("  test  "));
        }

        [Fact]
        public void IsValid_RejectsInputExceedingMaxLength()
        {
            var longInput = new string('a', 201);
            Assert.False(SearchInputValidator.IsValid(longInput));
        }

        [Fact]
        public void IsValid_AcceptsInputAtMaxLength()
        {
            var maxInput = new string('a', 200);
            Assert.True(SearchInputValidator.IsValid(maxInput));
        }

        [Fact]
        public void MaxSearchTermLength_Is200()
        {
            Assert.Equal(200, SearchInputValidator.MaxSearchTermLength);
        }

        [Theory]
        [InlineData("SELECT")]
        [InlineData("DROP")]
        [InlineData("INSERT")]
        [InlineData("UPDATE")]
        [InlineData("DELETE")]
        public void Sanitize_PreservesSqlKeywordsWithoutMetaCharacters(string keyword)
        {
            var result = SearchInputValidator.Sanitize(keyword);
            Assert.Equal(keyword, result);
        }

        [Theory]
        [InlineData("'; DROP TABLE CatalogItems; --")]
        [InlineData("1 OR 1=1")]
        [InlineData("' UNION SELECT password FROM users --")]
        [InlineData("%' AND 1=1 AND '%'='")]
        public void CatalogSearch_SqlInjectionPayloadsAreSafelyHandled(string injectionPayload)
        {
            var sanitized = SearchInputValidator.Sanitize(injectionPayload);
            Assert.DoesNotContain("'", sanitized);
            Assert.DoesNotContain(";", sanitized);
            Assert.DoesNotContain("--", sanitized);
        }
    }
}
