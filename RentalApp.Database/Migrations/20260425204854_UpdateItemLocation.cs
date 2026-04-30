using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Items_users_OwnerId", table: "Items");

            migrationBuilder.DropPrimaryKey(name: "PK_Items", table: "Items");

            migrationBuilder.DropColumn(name: "Location", table: "Items");

            migrationBuilder.RenameTable(name: "Items", newName: "items");

            migrationBuilder.RenameIndex(
                name: "IX_Items_OwnerId",
                table: "items",
                newName: "IX_items_OwnerId"
            );

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "items",
                type: "text",
                nullable: true
            );

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "items",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0
            );

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "items",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0
            );

            migrationBuilder.AddPrimaryKey(name: "PK_items", table: "items", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_items_users_OwnerId",
                table: "items",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_items_users_OwnerId", table: "items");

            migrationBuilder.DropPrimaryKey(name: "PK_items", table: "items");

            migrationBuilder.DropColumn(name: "Category", table: "items");

            migrationBuilder.DropColumn(name: "Latitude", table: "items");

            migrationBuilder.DropColumn(name: "Longitude", table: "items");

            migrationBuilder.RenameTable(name: "items", newName: "Items");

            migrationBuilder.RenameIndex(
                name: "IX_items_OwnerId",
                table: "Items",
                newName: "IX_Items_OwnerId"
            );

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddPrimaryKey(name: "PK_Items", table: "Items", column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_users_OwnerId",
                table: "Items",
                column: "OwnerId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
