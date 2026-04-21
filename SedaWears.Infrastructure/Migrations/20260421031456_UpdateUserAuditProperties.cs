using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SedaWears.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAuditProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Shops",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Shops",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "ShopOwners",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ShopOwners",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "ShopManagers",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ShopManagers",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "ProductSales",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "ProductSales",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Products",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Products",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Orders",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Orders",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "NewsletterSubscribers",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "NewsletterSubscribers",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Categories",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Categories",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "AspNetUsers",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "AspNetUsers",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Addresses",
                newName: "LastModifiedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Addresses",
                newName: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedById",
                table: "AspNetUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastModifiedById",
                table: "AspNetUsers",
                column: "LastModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CreatedById",
                table: "AspNetUsers",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_LastModifiedById",
                table: "AspNetUsers",
                column: "LastModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_LastModifiedById",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LastModifiedById",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Shops",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Shops",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "ShopOwners",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ShopOwners",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "ShopManagers",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ShopManagers",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "ProductSales",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ProductSales",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Products",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Products",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Orders",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Orders",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "NewsletterSubscribers",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "NewsletterSubscribers",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Categories",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Categories",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "AspNetUsers",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "AspNetUsers",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedById",
                table: "Addresses",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Addresses",
                newName: "CreatedBy");
        }
    }
}
