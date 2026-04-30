using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RentalApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "items",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "items",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision"
            );

            migrationBuilder.CreateTable(
                name: "rentals",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    ItemTitle = table.Column<string>(type: "text", nullable: false),
                    BorrowerId = table.Column<int>(type: "integer", nullable: false),
                    BorrowerName = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    OwnerName = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    EndDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rentals_items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_rentals_users_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_rentals_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_items_CategoryId",
                table: "items",
                column: "CategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_rentals_BorrowerId",
                table: "rentals",
                column: "BorrowerId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_rentals_ItemId",
                table: "rentals",
                column: "ItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_rentals_OwnerId",
                table: "rentals",
                column: "OwnerId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_items_categories_CategoryId",
                table: "items",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_items_categories_CategoryId", table: "items");

            migrationBuilder.DropTable(name: "rentals");

            migrationBuilder.DropIndex(name: "IX_items_CategoryId", table: "items");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "items",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "items",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true
            );
        }
    }
}
