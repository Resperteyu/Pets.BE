using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddEuropeanCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
