using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnPrivateToPetProfileDbTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 5, 20, 2, 16, 96, DateTimeKind.Utc).AddTicks(6348));

            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "PetProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_Private",
                table: "PetProfile",
                column: "Private");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PetProfile_Private",
                table: "PetProfile");

            migrationBuilder.DropColumn(
                name: "Private",
                table: "PetProfile");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 5, 20, 2, 16, 96, DateTimeKind.Utc).AddTicks(6348),
                oldClrType: typeof(DateTime),
                oldType: "date");
        }
    }
}
