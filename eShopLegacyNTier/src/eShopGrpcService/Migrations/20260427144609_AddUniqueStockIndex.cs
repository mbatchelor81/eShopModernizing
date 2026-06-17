using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopGrpcService.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueStockIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemsStock_CatalogItemId_Date",
                table: "CatalogItemsStock",
                columns: new[] { "CatalogItemId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogItemsStock_CatalogItemId_Date",
                table: "CatalogItemsStock");
        }
    }
}
