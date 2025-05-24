using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPetProfileCreationDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 18, 9, 48, 32, 642, DateTimeKind.Utc).AddTicks(6722));

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_CreationDate",
                table: "PetProfile",
                column: "CreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PetProfile_CreationDate",
                table: "PetProfile");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "PetProfile");
        }
    }
}
