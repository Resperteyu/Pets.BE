using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnersIdColumnsToMateRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PetMateOwnerId",
                table: "MateRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PetOwnerId",
                table: "MateRequest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetMateOwnerId",
                table: "MateRequest",
                column: "PetMateOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetOwnerId",
                table: "MateRequest",
                column: "PetOwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MateRequest_PetMateOwnerId",
                table: "MateRequest");

            migrationBuilder.DropIndex(
                name: "IX_MateRequest_PetOwnerId",
                table: "MateRequest");

            migrationBuilder.DropColumn(
                name: "PetMateOwnerId",
                table: "MateRequest");

            migrationBuilder.DropColumn(
                name: "PetOwnerId",
                table: "MateRequest");
        }
    }
}
