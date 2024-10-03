using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UMEC528_AddWalletForTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "wallet",
                table: "tenants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wallet",
                table: "tenants");
        }
    }
}
