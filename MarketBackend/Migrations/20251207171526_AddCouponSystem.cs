using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<string>(
                name: "AppliedCouponCode",
                table: "ShoppingCarts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MinimumPurchaseAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxUsageCount = table.Column<int>(type: "INTEGER", nullable: true),
                    CurrentUsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByAdminId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBySellerId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BrandId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                    table.UniqueConstraint("AK_Coupons_Code", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Coupons_AspNetUsers_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coupons_AspNetUsers_CreatedBySellerId",
                        column: x => x.CreatedBySellerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coupons_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_AppliedCouponCode",
                table: "ShoppingCarts",
                column: "AppliedCouponCode");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_BrandId",
                table: "Coupons",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CreatedByAdminId",
                table: "Coupons",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CreatedBySellerId",
                table: "Coupons",
                column: "CreatedBySellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Coupons_AppliedCouponCode",
                table: "ShoppingCarts",
                column: "AppliedCouponCode",
                principalTable: "Coupons",
                principalColumn: "Code",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Coupons_AppliedCouponCode",
                table: "ShoppingCarts");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_AppliedCouponCode",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "AppliedCouponCode",
                table: "ShoppingCarts");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "ShoppingCarts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
