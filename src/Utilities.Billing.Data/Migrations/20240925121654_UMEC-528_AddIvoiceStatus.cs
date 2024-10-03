using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UMEC528_AddIvoiceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "invoices");
        }
    }
}
