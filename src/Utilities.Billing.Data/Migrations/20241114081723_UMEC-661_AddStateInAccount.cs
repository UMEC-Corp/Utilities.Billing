using Microsoft.EntityFrameworkCore.Migrations;
using Utilities.Billing.Data.Entities;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UMEC661_AddStateInAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql($"update accounts set state = {(int)AccountState.Ok};");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "state",
                table: "accounts");
        }
    }
}
