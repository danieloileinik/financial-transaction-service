using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialTransactionService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "transactions",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "accounts",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_AccountId_Timestamp",
                schema: "public",
                table: "transactions",
                newName: "IX_transactions_AccountId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_AccountId",
                schema: "public",
                table: "transactions",
                newName: "IX_transactions_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactions",
                schema: "public",
                table: "transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accounts",
                schema: "public",
                table: "accounts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_transactions",
                schema: "public",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_accounts",
                schema: "public",
                table: "accounts");

            migrationBuilder.RenameTable(
                name: "transactions",
                schema: "public",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "accounts",
                schema: "public",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_AccountId_Timestamp",
                table: "Transactions",
                newName: "IX_Transactions_AccountId_Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_AccountId",
                table: "Transactions",
                newName: "IX_Transactions_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");
        }
    }
}
