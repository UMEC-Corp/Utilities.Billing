using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class umec462_AddTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "assets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_assets_tenant_id",
                table: "assets",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_assets_tenants_tenant_id",
                table: "assets",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_assets_tenants_tenant_id",
                table: "assets");

            migrationBuilder.DropIndex(
                name: "ix_assets_tenant_id",
                table: "assets");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "assets");
        }
    }
}
