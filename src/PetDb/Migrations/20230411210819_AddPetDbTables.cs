using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetDb.Migrations
{
    /// <inheritdoc />
    public partial class AddPetDbTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    DialCode = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.CountryCode);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Name = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sex",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sex", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    PhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profile_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Country",
                        principalColumn: "CountryCode");
                    table.ForeignKey(
                        name: "FK_Profile_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetBreed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetBreed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetBreed_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PetType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PetProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SexId = table.Column<byte>(type: "tinyint", nullable: false),
                    BreedId = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    AvailableForBreeding = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetProfile_BreedId",
                        column: x => x.BreedId,
                        principalTable: "PetBreed",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PetProfile_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Profile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PetProfile_SexId",
                        column: x => x.SexId,
                        principalTable: "Sex",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetBreed_TypeId",
                table: "PetBreed",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_BreedId",
                table: "PetProfile",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_OwnerId",
                table: "PetProfile",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_SexId",
                table: "PetProfile",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_CountryCode",
                table: "Profile",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Profile_LocationId",
                table: "Profile",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetProfile");

            migrationBuilder.DropTable(
                name: "PetBreed");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Sex");

            migrationBuilder.DropTable(
                name: "PetType");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
