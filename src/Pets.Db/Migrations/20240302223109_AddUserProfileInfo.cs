using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 3, 2, 22, 31, 9, 275, DateTimeKind.Utc).AddTicks(7834),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 2, 18, 9, 48, 32, 642, DateTimeKind.Utc).AddTicks(6722));

            migrationBuilder.CreateTable(
                name: "UserProfileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AboutMe = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProfilePhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfileInfo_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileInfo_ApplicationUserId",
                table: "UserProfileInfo",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfileInfo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationDate",
                table: "PetProfile",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(2024, 2, 18, 9, 48, 32, 642, DateTimeKind.Utc).AddTicks(6722),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldDefaultValue: new DateTime(2024, 3, 2, 22, 31, 9, 275, DateTimeKind.Utc).AddTicks(7834));
        }
    }
}
