using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                        { Guid.NewGuid().ToString(), "Administrator", "ADMINISTRATOR", Guid.NewGuid().ToString() },
                        { Guid.NewGuid().ToString(), "PetOwner", "PETOWNER", Guid.NewGuid().ToString() },
                        { Guid.NewGuid().ToString(), "Shelter", "SHELTER", Guid.NewGuid().ToString() },
                        { Guid.NewGuid().ToString(), "Professional", "PROFESSIONAL", Guid.NewGuid().ToString() },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValues: new object[] { "Administrator", "PetOwner", "Shelter", "Professional" });
        }
    }
}
