using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LugaStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameNewsletterToNewsletterSubscriber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_CreatorId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Newsletters");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreatorId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "PartnerId",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NewsletterSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsSubscribed = table.Column<bool>(type: "boolean", nullable: false),
                    UnsubscribeToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscribers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PartnerId",
                table: "Categories",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubscribers_Email",
                table: "NewsletterSubscribers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsletterSubscribers_UnsubscribeToken",
                table: "NewsletterSubscribers",
                column: "UnsubscribeToken",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_PartnerId",
                table: "Categories",
                column: "PartnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_PartnerId",
                table: "Categories");

            migrationBuilder.DropTable(
                name: "NewsletterSubscribers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PartnerId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Newsletters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSubscribed = table.Column<bool>(type: "boolean", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    UnsubscribeToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Newsletters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatorId",
                table: "Products",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_Email",
                table: "Newsletters",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Newsletters_UnsubscribeToken",
                table: "Newsletters",
                column: "UnsubscribeToken",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_CreatorId",
                table: "Products",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
