using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddLitterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Litter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BreedId = table.Column<int>(type: "int", nullable: false),
                    MotherPetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FatherPetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Available = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Litter_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Litter_PetBreed_BreedId",
                        column: x => x.BreedId,
                        principalTable: "PetBreed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Litter_PetProfile_FatherPetId",
                        column: x => x.FatherPetId,
                        principalTable: "PetProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Litter_PetProfile_MotherPetId",
                        column: x => x.MotherPetId,
                        principalTable: "PetProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Litter_Available",
                table: "Litter",
                column: "Available");

            migrationBuilder.CreateIndex(
                name: "IX_Litter_BreedId",
                table: "Litter",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_Litter_CreationDate",
                table: "Litter",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Litter_FatherPetId",
                table: "Litter",
                column: "FatherPetId");

            migrationBuilder.CreateIndex(
                name: "IX_Litter_MotherPetId",
                table: "Litter",
                column: "MotherPetId");

            migrationBuilder.CreateIndex(
                name: "IX_Litter_OwnerId",
                table: "Litter",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Litter");
        }
    }
}
