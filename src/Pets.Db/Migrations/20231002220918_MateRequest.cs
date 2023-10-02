using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class MateRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MateRequestState",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateRequestState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetMateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    AmountAgreement = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LitterSplitAgreement = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    BreedingPlaceAgreement = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Response = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MateRequestStateId = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateRequest_MateRequestState_MateRequestStateId",
                        column: x => x.MateRequestStateId,
                        principalTable: "MateRequestState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MateRequest_PetProfile_PetMateProfileId",
                        column: x => x.PetMateProfileId,
                        principalTable: "PetProfile",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MateRequest_PetProfile_PetProfileId",
                        column: x => x.PetProfileId,
                        principalTable: "PetProfile",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "MateRequestState",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { (byte)1, "Sent" },
                    { (byte)2, "Changes requested" },
                    { (byte)3, "Accepted" },
                    { (byte)4, "Breeding" },
                    { (byte)5, "Rejected" },
                    { (byte)6, "Completed" },
                    { (byte)7, "Failed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_MateRequestStateId",
                table: "MateRequest",
                column: "MateRequestStateId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetMateProfileId",
                table: "MateRequest",
                column: "PetMateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetProfileId",
                table: "MateRequest",
                column: "PetProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MateRequest");

            migrationBuilder.DropTable(
                name: "MateRequestState");
        }
    }
}
