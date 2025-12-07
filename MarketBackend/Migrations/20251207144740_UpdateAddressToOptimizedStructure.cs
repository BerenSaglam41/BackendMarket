using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddressToOptimizedStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Addresses",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "Addresses",
                newName: "FullAddress");

            migrationBuilder.RenameColumn(
                name: "IsDefaultAddress",
                table: "Addresses",
                newName: "IsDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Addresses",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "Addresses",
                newName: "IsDefaultAddress");

            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "Addresses",
                newName: "StreetAddress");

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "Addresses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
