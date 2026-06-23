using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eShopGrpcService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBrands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItemsStock",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    AvailableStock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemsStock", x => x.StockId);
                });

            migrationBuilder.CreateTable(
                name: "CatalogTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size = table.Column<double>(type: "float", nullable: false),
                    Start = table.Column<DateTime>(type: "date", nullable: false),
                    End = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "money", precision: 19, scale: 4, nullable: false),
                    Picturefilename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatalogBrandId = table.Column<int>(type: "int", nullable: false),
                    CatalogTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogBrands_CatalogBrandId",
                        column: x => x.CatalogBrandId,
                        principalTable: "CatalogBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogTypes_CatalogTypeId",
                        column: x => x.CatalogTypeId,
                        principalTable: "CatalogTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CatalogBrands",
                columns: new[] { "Id", "Brand" },
                values: new object[,]
                {
                    { 1, "Azure" },
                    { 2, ".NET" },
                    { 3, "Visual Studio" },
                    { 4, "SQL Server" },
                    { 5, "Other" }
                });

            migrationBuilder.InsertData(
                table: "CatalogItemsStock",
                columns: new[] { "StockId", "AvailableStock", "CatalogItemId", "Date" },
                values: new object[,]
                {
                    { 1, 100, 1, new DateTime(2017, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 120, 1, new DateTime(2017, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 80, 1, new DateTime(2017, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 45, 2, new DateTime(2017, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 65, 4, new DateTime(2017, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 22, 5, new DateTime(2017, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "CatalogTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, "Mug" },
                    { 2, "T-Shirt" },
                    { 3, "Sheet" },
                    { 4, "USB Memory Stick" }
                });

            migrationBuilder.InsertData(
                table: "DiscountItems",
                columns: new[] { "Id", "End", "Size", "Start" },
                values: new object[,]
                {
                    { 1, new DateTime(2017, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.29999999999999999, new DateTime(2017, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2017, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.25, new DateTime(2017, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2017, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.10000000000000001, new DateTime(2017, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2017, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.5, new DateTime(2017, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2017, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.29999999999999999, new DateTime(2017, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2017, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.25, new DateTime(2017, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "CatalogItems",
                columns: new[] { "Id", "CatalogBrandId", "CatalogTypeId", "Description", "Name", "Picturefilename", "Price" },
                values: new object[,]
                {
                    { 1, 2, 2, ".NET Bot Black Hoodie", ".NET Bot Black Hoodie", "2.png", 19.5m },
                    { 2, 2, 1, ".NET Black & White Mug", ".NET Black & White Mug", "11.png", 8.50m },
                    { 3, 5, 2, "Prism White T-Shirt", "Prism White T-Shirt", "7.png", 12m },
                    { 4, 2, 2, ".NET Foundation T-shirt", ".NET Foundation T-shirt", "5.png", 12m },
                    { 5, 5, 3, "Roslyn Red Sheet", "Roslyn Red Sheet", "9.png", 8.5m },
                    { 6, 2, 2, ".NET Blue Hoodie", ".NET Blue Hoodie", "1.png", 12m },
                    { 7, 5, 2, "Roslyn Red T-Shirt", "Roslyn Red T-Shirt", "6.png", 12m },
                    { 8, 5, 2, "Kudu Purple Hoodie", "Kudu Purple Hoodie", "3.png", 8.5m },
                    { 9, 5, 1, "Cup<T> White Mug", "Cup<T> White Mug", "12.png", 12m },
                    { 10, 2, 3, ".NET Foundation Sheet", ".NET Foundation Sheet", "8.png", 12m },
                    { 11, 2, 3, "Cup<T> Sheet", "Cup<T> Sheet", "10.png", 8.5m },
                    { 12, 5, 2, "Cup<T> TShirt", "Cup<T> TShirt", "4.png", 12m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogBrandId",
                table: "CatalogItems",
                column: "CatalogBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogTypeId",
                table: "CatalogItems",
                column: "CatalogTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems");

            migrationBuilder.DropTable(
                name: "CatalogItemsStock");

            migrationBuilder.DropTable(
                name: "DiscountItems");

            migrationBuilder.DropTable(
                name: "CatalogBrands");

            migrationBuilder.DropTable(
                name: "CatalogTypes");
        }
    }
}
