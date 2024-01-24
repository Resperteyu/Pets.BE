using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedingAndAddFlagsToPetProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForAdoption",
                table: "PetProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ForSale",
                table: "PetProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Missing",
                table: "PetProfile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "PetProfile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("af3dd336-0bbb-4c52-b8cc-b45f90517155"), "ada39352-2b42-4a6c-a073-fe0a60f30107", "Administrator", "ADMINISTRATOR" },
                    { new Guid("b82f0af8-26e8-4c3d-80d9-f9d85db6af10"), "b0411996-0450-4fb5-aa0f-ea0f1841c6aaba94c8a5-0198-4edf-9b2e-e2128a46457a", "PetOwner", "PETOWNER" },
                    { new Guid("ba94c8a5-0198-4edf-9b2e-e2128a46457a"), "fe74e7b7-3ae7-4e32-9402-39545953b20a", "Shelter", "SHELTER" },
                    { new Guid("d2631f51-a5a5-4413-afeb-8104e779b886"), "163b55d7-5be1-447b-b00a-4314f3e17b2c", "Professional", "PROFESSIONAL" }
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Code", "DialCode", "Name" },
                values: new object[,]
                {
                    { "AT", "+43", "Austria" },
                    { "BE", "+32", "Belgium" },
                    { "BG", "+359", "Bulgaria" },
                    { "CY", "+357", "Cyprus" },
                    { "CZ", "+420", "Czech Republic" },
                    { "DE", "+49", "Germany" },
                    { "DK", "+45", "Denmark" },
                    { "EE", "+372", "Estonia" },
                    { "ES", "+34", "Spain" },
                    { "FI", "+358", "Finland" },
                    { "FR", "+33", "France" },
                    { "GR", "+30", "Greece" },
                    { "HR", "+385", "Croatia" },
                    { "HU", "+36", "Hungary" },
                    { "IE", "+353", "Ireland" },
                    { "IT", "+39", "Italy" },
                    { "LT", "+370", "Lithuania" },
                    { "LU", "+352", "Luxembourg" },
                    { "LV", "+371", "Latvia" },
                    { "MT", "+356", "Malta" },
                    { "NL", "+31", "Netherlands" },
                    { "PL", "+48", "Poland" },
                    { "PT", "+351", "Portugal" },
                    { "RO", "+40", "Romania" },
                    { "SE", "+46", "Sweden" },
                    { "SI", "+386", "Slovenia" },
                    { "SK", "+421", "Slovakia" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("af3dd336-0bbb-4c52-b8cc-b45f90517155"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b82f0af8-26e8-4c3d-80d9-f9d85db6af10"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("ba94c8a5-0198-4edf-9b2e-e2128a46457a"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d2631f51-a5a5-4413-afeb-8104e779b886"));

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "AT");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "BE");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "BG");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "CY");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "CZ");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "DE");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "DK");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "EE");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "ES");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "FI");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "FR");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "GR");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "HR");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "HU");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "IE");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "IT");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "LT");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "LU");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "LV");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "MT");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "NL");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "PL");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "PT");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "RO");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "SE");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "SI");

            migrationBuilder.DeleteData(
                table: "Country",
                keyColumn: "Code",
                keyValue: "SK");

            migrationBuilder.DropColumn(
                name: "ForAdoption",
                table: "PetProfile");

            migrationBuilder.DropColumn(
                name: "ForSale",
                table: "PetProfile");

            migrationBuilder.DropColumn(
                name: "Missing",
                table: "PetProfile");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PetProfile");
        }
    }
}
