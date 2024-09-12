using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utilities.Billing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UMEC501_ChangePaymentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currency_amount",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "date",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "date_to",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "token_amount",
                table: "payments",
                newName: "amount");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_payments_account_id",
                table: "payments",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_asset_id",
                table: "payments",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_payments_accounts_account_id",
                table: "payments",
                column: "account_id",
                principalTable: "accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_payments_assets_asset_id",
                table: "payments",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_payments_accounts_account_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "fk_payments_assets_asset_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_account_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "ix_payments_asset_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "payments",
                newName: "token_amount");

            migrationBuilder.AddColumn<decimal>(
                name: "currency_amount",
                table: "payments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_to",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
