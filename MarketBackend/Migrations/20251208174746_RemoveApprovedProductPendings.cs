using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApprovedProductPendings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete all ProductPending records with Status = Approved (1)
            // These are legacy records from before the approval flow was changed to delete ProductPending
            migrationBuilder.Sql("DELETE FROM ProductPendings WHERE Status = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
