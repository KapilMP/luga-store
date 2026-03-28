using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LugaStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailResendTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "EmailLogs",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_MessageId",
                table: "EmailLogs",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_MessageId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "EmailLogs");
        }
    }
}
