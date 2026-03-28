using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LugaStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "AspNetUsers");
        }
    }
}
