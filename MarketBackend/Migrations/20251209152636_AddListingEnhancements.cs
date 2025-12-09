using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddListingEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "CartItems",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "SellerProducts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SellerProducts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductPendings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Coupons",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductViewHistories",
                columns: table => new
                {
                    ProductViewHistoryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", nullable: true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ViewDuration = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductViewHistories", x => x.ProductViewHistoryId);
                    table.ForeignKey(
                        name: "FK_ProductViewHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductViewHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductViewHistories_ProductId",
                table: "ProductViewHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductViewHistories_UserId",
                table: "ProductViewHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductViewHistories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "SellerProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SellerProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductPendings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Coupons");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "CartItems",
                newName: "LastUpdated");
        }
    }
}
