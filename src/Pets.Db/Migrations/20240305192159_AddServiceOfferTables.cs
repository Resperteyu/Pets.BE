using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceOfferTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 5, 19, 21, 59, 338, DateTimeKind.Utc).AddTicks(3576),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 2, 22, 31, 9, 275, DateTimeKind.Utc).AddTicks(7834));

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ForCats = table.Column<bool>(type: "bit", nullable: false),
                    ForDogs = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeakRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalPetRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOffer_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceOffer_ServiceType_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_Active",
                table: "ServiceOffer",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_CreationDate",
                table: "ServiceOffer",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_ServiceTypeId",
                table: "ServiceOffer",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_UserId",
                table: "ServiceOffer",
                column: "UserId");

            migrationBuilder.InsertData(
                table: "ServiceType",
                columns: new[] { "Id", "Title", "Url" },
                values: new object[,]
                {
                    { (byte)1, "Pet Sitting", "PetSitting" },
                    { (byte)2, "Pet Hosting", "PetHosting"},
                    { (byte)3, "Pet Visit", "PetVisit" },
                    { (byte)4, "Dog Walking", "DogWalking" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOffer");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 2, 22, 31, 9, 275, DateTimeKind.Utc).AddTicks(7834),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 5, 19, 21, 59, 338, DateTimeKind.Utc).AddTicks(3576));
        }
    }
}
