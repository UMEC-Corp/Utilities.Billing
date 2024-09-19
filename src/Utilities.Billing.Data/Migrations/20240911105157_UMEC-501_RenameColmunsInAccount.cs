using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UMEC501_RenameColmunsInAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "meter_number",
                table: "accounts",
                newName: "input_code");

            migrationBuilder.RenameColumn(
                name: "controller_serial",
                table: "accounts",
                newName: "device_serial");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "input_code",
                table: "accounts",
                newName: "meter_number");

            migrationBuilder.RenameColumn(
                name: "device_serial",
                table: "accounts",
                newName: "controller_serial");
        }
    }
}
