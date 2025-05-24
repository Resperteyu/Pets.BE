using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AmendServiceOfferDecimalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceOffer_UserId",
                table: "ServiceOffer");

            migrationBuilder.AlterColumn<int>(
                name: "Rate",
                table: "ServiceOffer",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "PeakRate",
                table: "ServiceOffer",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "HourlyRate",
                table: "ServiceOffer",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "AdditionalPetRate",
                table: "ServiceOffer",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 5, 20, 2, 16, 96, DateTimeKind.Utc).AddTicks(6348),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 5, 19, 21, 59, 338, DateTimeKind.Utc).AddTicks(3576));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_UserId_ServiceTypeId",
                table: "ServiceOffer",
                columns: new[] { "UserId", "ServiceTypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceOffer_UserId_ServiceTypeId",
                table: "ServiceOffer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "ServiceOffer",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "PeakRate",
                table: "ServiceOffer",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "HourlyRate",
                table: "ServiceOffer",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "AdditionalPetRate",
                table: "ServiceOffer",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 5, 19, 21, 59, 338, DateTimeKind.Utc).AddTicks(3576),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 5, 20, 2, 16, 96, DateTimeKind.Utc).AddTicks(6348));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOffer_UserId",
                table: "ServiceOffer",
                column: "UserId");
        }
    }
}
