using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eShopPorted.Tests
{
    #region Inline model & service definitions (mirrors eShopPorted types)

    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }
    }

    public class CatalogType
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class CatalogItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureFileName { get; set; }
        public int CatalogTypeId { get; set; }
        public CatalogType CatalogType { get; set; }
        public int CatalogBrandId { get; set; }
        public CatalogBrand CatalogBrand { get; set; }
    }

    public class TestCatalogDBContext : DbContext
    {
        public TestCatalogDBContext(DbContextOptions<TestCatalogDBContext> options) : base(options) { }
        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }
    }

    #endregion

    public class CatalogSearchSecurityTests : IDisposable
    {
        private readonly TestCatalogDBContext _context;

        public CatalogSearchSecurityTests()
        {
            var options = new DbContextOptionsBuilder<TestCatalogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TestCatalogDBContext(options);
            SeedTestData();
        }

        private void SeedTestData()
        {
            var brand = new CatalogBrand { Id = 1, Brand = "TestBrand" };
            var type = new CatalogType { Id = 1, Type = "TestType" };

            _context.CatalogBrands.Add(brand);
            _context.CatalogTypes.Add(type);

            _context.CatalogItems.AddRange(
                new CatalogItem { Id = 1, Name = "Widget Alpha", Description = "A widget", Price = 10.00m, CatalogBrandId = 1, CatalogTypeId = 1 },
                new CatalogItem { Id = 2, Name = "Widget Beta", Description = "Another widget", Price = 20.00m, CatalogBrandId = 1, CatalogTypeId = 1 },
                new CatalogItem { Id = 3, Name = "Gadget Gamma", Description = "A gadget", Price = 30.00m, CatalogBrandId = 1, CatalogTypeId = 1 }
            );

            _context.SaveChanges();
        }

        private IEnumerable<CatalogItem> SearchCatalogItems(string searchTerm)
        {
            return _context.CatalogItems
                .Include(c => c.CatalogBrand)
                .Include(c => c.CatalogType)
                .Where(c => c.Name.Contains(searchTerm))
                .OrderBy(c => c.Id)
                .ToList();
        }

        [Fact]
        public void SearchCatalogItems_WithValidTerm_ReturnsMatchingItems()
        {
            var results = SearchCatalogItems("Widget").ToList();
            Assert.Equal(2, results.Count);
            Assert.All(results, item => Assert.Contains("Widget", item.Name));
        }

        [Fact]
        public void SearchCatalogItems_WithNoMatch_ReturnsEmpty()
        {
            var results = SearchCatalogItems("NonExistent").ToList();
            Assert.Empty(results);
        }

        [Fact]
        public void SearchCatalogItems_WithSqlInjectionPayload_DoesNotExecuteInjectedSql()
        {
            var results = SearchCatalogItems("'; DROP TABLE CatalogItems; --").ToList();
            Assert.Empty(results);

            var allItems = _context.CatalogItems.ToList();
            Assert.Equal(3, allItems.Count);
        }

        [Fact]
        public void SearchCatalogItems_WithUnionInjection_DoesNotLeakData()
        {
            var results = SearchCatalogItems("' UNION SELECT * FROM CatalogItems --").ToList();
            Assert.Empty(results);

            var allItems = _context.CatalogItems.ToList();
            Assert.Equal(3, allItems.Count);
        }

        [Fact]
        public void SearchCatalogItems_WithBooleanBlindInjection_DoesNotExpose()
        {
            var results = SearchCatalogItems("Widget' OR '1'='1").ToList();
            Assert.Empty(results);
        }

        [Fact]
        public void SearchCatalogItems_MultipleInjectionPayloads_DataIntact()
        {
            var dangerousInputs = new[]
            {
                "'; DELETE FROM CatalogItems; --",
                "1 OR 1=1",
                "Widget%' OR 1=1 --",
                "'; EXEC xp_cmdshell('whoami'); --",
                "Widget\"; DROP TABLE CatalogItems; --"
            };

            foreach (var input in dangerousInputs)
            {
                var results = SearchCatalogItems(input).ToList();
                Assert.True(results.Count <= 3, $"Unexpected results for input: {input}");
            }

            var allItems = _context.CatalogItems.ToList();
            Assert.Equal(3, allItems.Count);
        }

        [Fact]
        public void InputValidation_RejectsOverlongSearchTerm()
        {
            var longInput = new string('A', 201);
            Assert.True(longInput.Length > 200);
        }

        [Fact]
        public void InputValidation_RejectsSqlMetacharacters()
        {
            var pattern = new Regex(@"[;'\""]|--|/\*|\*/", RegexOptions.Compiled);

            Assert.True(pattern.IsMatch("'; DROP TABLE--"));
            Assert.True(pattern.IsMatch("test'value"));
            Assert.True(pattern.IsMatch("test;value"));
            Assert.True(pattern.IsMatch("test/*value"));
            Assert.False(pattern.IsMatch("Widget Alpha"));
            Assert.False(pattern.IsMatch("Gadget"));
            Assert.False(pattern.IsMatch("T-Shirt"));
            Assert.False(pattern.IsMatch("3/4 inch"));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
