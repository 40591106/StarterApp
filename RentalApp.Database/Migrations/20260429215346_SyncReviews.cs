using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class SyncReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "items",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerName",
                table: "items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalReviews",
                table: "items",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "items");

            migrationBuilder.DropColumn(
                name: "OwnerName",
                table: "items");

            migrationBuilder.DropColumn(
                name: "TotalReviews",
                table: "items");
        }
    }
}
