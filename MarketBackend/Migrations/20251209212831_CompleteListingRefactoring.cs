using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class CompleteListingRefactoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_SellerProducts_ListingId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_SellerProducts_ListingId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductViewHistories_Products_ProductId",
                table: "ProductViewHistories");

            migrationBuilder.DropTable(
                name: "SellerProducts");

            migrationBuilder.DropIndex(
                name: "IX_ProductViewHistories_UserId",
                table: "ProductViewHistories");

            migrationBuilder.RenameColumn(
                name: "ViewedAt",
                table: "ProductViewHistories",
                newName: "LastViewedAt");

            migrationBuilder.RenameColumn(
                name: "ViewDuration",
                table: "ProductViewHistories",
                newName: "ViewCount");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "ProductViewHistories",
                newName: "DeviceType");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ProductViewHistories",
                newName: "ListingId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductViewHistories_ProductId",
                table: "ProductViewHistories",
                newName: "IX_ProductViewHistories_ListingId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstViewedAt",
                table: "ProductViewHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SellerId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    ShippingTimeInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    SellerNote = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_Listings_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Listings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductViewHistories_UserId_ListingId",
                table: "ProductViewHistories",
                columns: new[] { "UserId", "ListingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ProductId",
                table: "Listings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_SellerId",
                table: "Listings",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Listings_ListingId",
                table: "CartItems",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Listings_ListingId",
                table: "OrderItems",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductViewHistories_Listings_ListingId",
                table: "ProductViewHistories",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Listings_ListingId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Listings_ListingId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductViewHistories_Listings_ListingId",
                table: "ProductViewHistories");

            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropIndex(
                name: "IX_ProductViewHistories_UserId_ListingId",
                table: "ProductViewHistories");

            migrationBuilder.DropColumn(
                name: "FirstViewedAt",
                table: "ProductViewHistories");

            migrationBuilder.RenameColumn(
                name: "ViewCount",
                table: "ProductViewHistories",
                newName: "ViewDuration");

            migrationBuilder.RenameColumn(
                name: "ListingId",
                table: "ProductViewHistories",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "LastViewedAt",
                table: "ProductViewHistories",
                newName: "ViewedAt");

            migrationBuilder.RenameColumn(
                name: "DeviceType",
                table: "ProductViewHistories",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductViewHistories_ListingId",
                table: "ProductViewHistories",
                newName: "IX_ProductViewHistories_ProductId");

            migrationBuilder.CreateTable(
                name: "SellerProducts",
                columns: table => new
                {
                    ListingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SellerId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    SellerNote = table.Column<string>(type: "TEXT", nullable: true),
                    ShippingCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    ShippingTimeInDays = table.Column<int>(type: "INTEGER", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerProducts", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_SellerProducts_AspNetUsers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellerProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductViewHistories_UserId",
                table: "ProductViewHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProducts_ProductId",
                table: "SellerProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProducts_SellerId",
                table: "SellerProducts",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_SellerProducts_ListingId",
                table: "CartItems",
                column: "ListingId",
                principalTable: "SellerProducts",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_SellerProducts_ListingId",
                table: "OrderItems",
                column: "ListingId",
                principalTable: "SellerProducts",
                principalColumn: "ListingId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductViewHistories_Products_ProductId",
                table: "ProductViewHistories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
