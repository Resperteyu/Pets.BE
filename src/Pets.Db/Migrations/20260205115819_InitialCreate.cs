using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LastName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character(2)", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DialCode = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "MateRequestState",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateRequestState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sex",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sex", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    JwtId = table.Column<string>(type: "text", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AboutMe = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ProfilePhotoUrl = table.Column<string>(type: "text", nullable: true),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Line1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Line2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Postcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CountryCode = table.Column<string>(type: "character(2)", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Address_Country_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Country",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PetBreed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeId = table.Column<byte>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetBreed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetBreed_PetType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceTypeId = table.Column<byte>(type: "smallint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    ForCats = table.Column<bool>(type: "boolean", nullable: false),
                    ForDogs = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Rate = table.Column<int>(type: "integer", nullable: false),
                    PeakRate = table.Column<int>(type: "integer", nullable: false),
                    HourlyRate = table.Column<int>(type: "integer", nullable: false),
                    AdditionalPetRate = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    GeoLocation = table.Column<Point>(type: "geometry", nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Location_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SexId = table.Column<byte>(type: "smallint", nullable: false),
                    BreedId = table.Column<int>(type: "integer", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    AvailableForBreeding = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ForSale = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    ForAdoption = table.Column<bool>(type: "boolean", nullable: false),
                    Missing = table.Column<bool>(type: "boolean", nullable: false),
                    Private = table.Column<bool>(type: "boolean", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetProfile_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetProfile_PetBreed_BreedId",
                        column: x => x.BreedId,
                        principalTable: "PetBreed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetProfile_Sex_SexId",
                        column: x => x.SexId,
                        principalTable: "Sex",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Litter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BreedId = table.Column<int>(type: "integer", nullable: false),
                    MotherPetId = table.Column<Guid>(type: "uuid", nullable: false),
                    FatherPetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "MateRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PetProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetMateProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetMateOwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    AmountAgreement = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LitterSplitAgreement = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BreedingPlaceAgreement = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Response = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    MateRequestStateId = table.Column<byte>(type: "smallint", nullable: false)
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
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("af3dd336-0bbb-4c52-b8cc-b45f90517155"), "ada39352-2b42-4a6c-a073-fe0a60f30107", "Administrator", "ADMINISTRATOR" },
                    { new Guid("b82f0af8-26e8-4c3d-80d9-f9d85db6af10"), "b0411996-0450-4fb5-aa0f-ea0f1841c6aa", "PetOwner", "PETOWNER" },
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
                    { "GB", "+44", "United Kingdom" },
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

            migrationBuilder.InsertData(
                table: "MateRequestState",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { (byte)1, "Pending" },
                    { (byte)2, "Accepted" },
                    { (byte)3, "Declined" },
                    { (byte)4, "Cancelled" },
                    { (byte)5, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "PetType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Cats" },
                    { (byte)2, "Dogs" }
                });

            migrationBuilder.InsertData(
                table: "ServiceType",
                columns: new[] { "Id", "Title", "Url" },
                values: new object[,]
                {
                    { (byte)1, "Pet Sitting", "pet-sitting" },
                    { (byte)2, "Pet Hosting", "pet-hosting" },
                    { (byte)3, "Pet Visit", "pet-visit" },
                    { (byte)4, "Dog Walking", "dog-walking" }
                });

            migrationBuilder.InsertData(
                table: "Sex",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { (byte)1, "Male" },
                    { (byte)2, "Female" }
                });

            migrationBuilder.InsertData(
                table: "PetBreed",
                columns: new[] { "Id", "Title", "TypeId" },
                values: new object[,]
                {
                    { 1, "Abyssinian", (byte)1 },
                    { 2, "Aegean", (byte)1 },
                    { 3, "American Bobtail", (byte)1 },
                    { 4, "American Curl", (byte)1 },
                    { 5, "American Shorthair", (byte)1 },
                    { 6, "American Wirehair", (byte)1 },
                    { 7, "Arabian Mau", (byte)1 },
                    { 8, "Asian", (byte)1 },
                    { 9, "Australian Mist", (byte)1 },
                    { 10, "Balinese", (byte)1 },
                    { 11, "Bambino", (byte)1 },
                    { 12, "Bengal", (byte)1 },
                    { 13, "Birman", (byte)1 },
                    { 14, "Bombay", (byte)1 },
                    { 15, "Brazilian Shorthair", (byte)1 },
                    { 16, "British Longhair", (byte)1 },
                    { 17, "British Shorthair", (byte)1 },
                    { 18, "Burmese", (byte)1 },
                    { 19, "Burmilla", (byte)1 },
                    { 20, "California Spangled", (byte)1 },
                    { 21, "Ceylon", (byte)1 },
                    { 22, "Chantilly-Tiffany", (byte)1 },
                    { 23, "Chartreux", (byte)1 },
                    { 24, "Chausie", (byte)1 },
                    { 25, "Colorpoint Shorthair", (byte)1 },
                    { 26, "Cornish Rex", (byte)1 },
                    { 27, "Cymric", (byte)1 },
                    { 28, "Devon Rex", (byte)1 },
                    { 29, "Donskoy", (byte)1 },
                    { 30, "Dragon Li", (byte)1 },
                    { 31, "Dwelf", (byte)1 },
                    { 32, "Egyptian Mau", (byte)1 },
                    { 33, "European Shorthair", (byte)1 },
                    { 34, "Exotic Shorthair", (byte)1 },
                    { 35, "Foldex", (byte)1 },
                    { 36, "German Rex", (byte)1 },
                    { 37, "Havana Brown", (byte)1 },
                    { 38, "Highlander", (byte)1 },
                    { 39, "Himalayan", (byte)1 },
                    { 40, "Japanese Bobtail", (byte)1 },
                    { 41, "Javanese", (byte)1 },
                    { 42, "Khao Manee", (byte)1 },
                    { 43, "Korat", (byte)1 },
                    { 44, "Kurilian Bobtail", (byte)1 },
                    { 45, "LaPerm", (byte)1 },
                    { 46, "Maine Coon", (byte)1 },
                    { 47, "Manx", (byte)1 },
                    { 48, "Mekong Bobtail", (byte)1 },
                    { 49, "Minskin", (byte)1 },
                    { 50, "Munchkin", (byte)1 },
                    { 51, "Nebelung", (byte)1 },
                    { 52, "Norwegian Forest Cat", (byte)1 },
                    { 53, "Ocicat", (byte)1 },
                    { 54, "Ojos Azules", (byte)1 },
                    { 55, "Oriental Bicolor", (byte)1 },
                    { 56, "Oriental Longhair", (byte)1 },
                    { 57, "Oriental Shorthair", (byte)1 },
                    { 58, "Persian", (byte)1 },
                    { 59, "Peterbald", (byte)1 },
                    { 60, "Pixie-Bob", (byte)1 },
                    { 61, "Raas", (byte)1 },
                    { 62, "Ragamuffin", (byte)1 },
                    { 63, "Ragdoll", (byte)1 },
                    { 64, "Russian Blue", (byte)1 },
                    { 65, "Russian White", (byte)1 },
                    { 66, "Russian Black", (byte)1 },
                    { 67, "Savannah", (byte)1 },
                    { 68, "Scottish Fold", (byte)1 },
                    { 69, "Selkirk Rex", (byte)1 },
                    { 70, "Serengeti", (byte)1 },
                    { 71, "Serrade Petit", (byte)1 },
                    { 72, "Siamese", (byte)1 },
                    { 73, "Siberian", (byte)1 },
                    { 74, "Singapura", (byte)1 },
                    { 75, "Snowshoe", (byte)1 },
                    { 76, "Sokoke", (byte)1 },
                    { 77, "Somali", (byte)1 },
                    { 78, "Sphynx", (byte)1 },
                    { 79, "Suphalak", (byte)1 },
                    { 80, "Thai", (byte)1 },
                    { 81, "Thai Lilac", (byte)1 },
                    { 82, "Tonkinese", (byte)1 },
                    { 83, "Toyger", (byte)1 },
                    { 84, "Turkish Angora", (byte)1 },
                    { 85, "Turkish Van", (byte)1 },
                    { 86, "Ukrainian Levkoy", (byte)1 },
                    { 87, "York Chocolate", (byte)1 },
                    { 88, "Affenpinscher", (byte)2 },
                    { 89, "Afghan Hound", (byte)2 },
                    { 90, "Airedale Terrier", (byte)2 },
                    { 91, "Akita", (byte)2 },
                    { 92, "Alaskan Malamute", (byte)2 },
                    { 93, "American English Coonhound", (byte)2 },
                    { 94, "American Eskimo Dog", (byte)2 },
                    { 95, "American Foxhound", (byte)2 },
                    { 96, "American Hairless Terrier", (byte)2 },
                    { 97, "American Leopard Hound", (byte)2 },
                    { 98, "American Staffordshire Terrier", (byte)2 },
                    { 99, "American Water Spaniel", (byte)2 },
                    { 100, "Anatolian Shepherd Dog", (byte)2 },
                    { 101, "Australian Cattle Dog", (byte)2 },
                    { 102, "Australian Shepherd", (byte)2 },
                    { 103, "Australian Terrier", (byte)2 },
                    { 104, "Basenji", (byte)2 },
                    { 105, "Basset Hound", (byte)2 },
                    { 106, "Beagle", (byte)2 },
                    { 107, "Bearded Collie", (byte)2 },
                    { 108, "Beauceron", (byte)2 },
                    { 109, "Belgian Malinois", (byte)2 },
                    { 110, "Belgian Sheepdog", (byte)2 },
                    { 111, "Belgian Tervuren", (byte)2 },
                    { 112, "Bernese Mountain Dog", (byte)2 },
                    { 113, "Bichon Frise", (byte)2 },
                    { 114, "Bloodhound", (byte)2 },
                    { 115, "Border Collie", (byte)2 },
                    { 116, "Border Terrier", (byte)2 },
                    { 117, "Borzoi", (byte)2 },
                    { 118, "Boston Terrier", (byte)2 },
                    { 119, "Boxer", (byte)2 },
                    { 120, "Brittany", (byte)2 },
                    { 121, "Brussels Griffon", (byte)2 },
                    { 122, "Bull Terrier", (byte)2 },
                    { 123, "Bulldog", (byte)2 },
                    { 124, "Bullmastiff", (byte)2 },
                    { 125, "Cairn Terrier", (byte)2 },
                    { 126, "Cane Corso", (byte)2 },
                    { 127, "Cardigan Welsh Corgi", (byte)2 },
                    { 128, "Cavalier King Charles Spaniel", (byte)2 },
                    { 129, "Chesapeake Bay Retriever", (byte)2 },
                    { 130, "Chihuahua", (byte)2 },
                    { 131, "Chinese Crested", (byte)2 },
                    { 132, "Chinese Shar-Pei", (byte)2 },
                    { 133, "Chow Chow", (byte)2 },
                    { 134, "Cocker Spaniel", (byte)2 },
                    { 135, "Collie", (byte)2 },
                    { 136, "Coton de Tulear", (byte)2 },
                    { 137, "Dachshund", (byte)2 },
                    { 138, "Dalmatian", (byte)2 },
                    { 139, "Doberman Pinscher", (byte)2 },
                    { 140, "English Cocker Spaniel", (byte)2 },
                    { 141, "English Setter", (byte)2 },
                    { 142, "English Springer Spaniel", (byte)2 },
                    { 143, "French Bulldog", (byte)2 },
                    { 144, "German Pinscher", (byte)2 },
                    { 145, "German Shepherd Dog", (byte)2 },
                    { 146, "German Shorthaired Pointer", (byte)2 },
                    { 147, "Giant Schnauzer", (byte)2 },
                    { 148, "Golden Retriever", (byte)2 },
                    { 149, "Gordon Setter", (byte)2 },
                    { 150, "Great Dane", (byte)2 },
                    { 151, "Great Pyrenees", (byte)2 },
                    { 152, "Greyhound", (byte)2 },
                    { 153, "Havanese", (byte)2 },
                    { 154, "Irish Setter", (byte)2 },
                    { 155, "Irish Wolfhound", (byte)2 },
                    { 156, "Italian Greyhound", (byte)2 },
                    { 157, "Jack Russell Terrier", (byte)2 },
                    { 158, "Japanese Chin", (byte)2 },
                    { 159, "Keeshond", (byte)2 },
                    { 160, "Kerry Blue Terrier", (byte)2 },
                    { 161, "Komondor", (byte)2 },
                    { 162, "Labrador Retriever", (byte)2 },
                    { 163, "Lhasa Apso", (byte)2 },
                    { 164, "Maltese", (byte)2 },
                    { 165, "Mastiff", (byte)2 },
                    { 166, "Miniature Pinscher", (byte)2 },
                    { 167, "Miniature Schnauzer", (byte)2 },
                    { 168, "Newfoundland", (byte)2 },
                    { 169, "Norfolk Terrier", (byte)2 },
                    { 170, "Norwegian Elkhound", (byte)2 },
                    { 171, "Norwich Terrier", (byte)2 },
                    { 172, "Old English Sheepdog", (byte)2 },
                    { 173, "Papillon", (byte)2 },
                    { 174, "Pekingese", (byte)2 },
                    { 175, "Pembroke Welsh Corgi", (byte)2 },
                    { 176, "Pointer", (byte)2 },
                    { 177, "Pomeranian", (byte)2 },
                    { 178, "Poodle", (byte)2 },
                    { 179, "Portuguese Water Dog", (byte)2 },
                    { 180, "Pug", (byte)2 },
                    { 181, "Puli", (byte)2 },
                    { 182, "Rhodesian Ridgeback", (byte)2 },
                    { 183, "Rottweiler", (byte)2 },
                    { 184, "Saint Bernard", (byte)2 },
                    { 185, "Saluki", (byte)2 },
                    { 186, "Samoyed", (byte)2 },
                    { 187, "Schipperke", (byte)2 },
                    { 188, "Scottish Deerhound", (byte)2 },
                    { 189, "Scottish Terrier", (byte)2 },
                    { 190, "Shetland Sheepdog", (byte)2 },
                    { 191, "Shiba Inu", (byte)2 },
                    { 192, "Shih Tzu", (byte)2 },
                    { 193, "Siberian Husky", (byte)2 },
                    { 194, "Silky Terrier", (byte)2 },
                    { 195, "Soft Coated Wheaten Terrier", (byte)2 },
                    { 196, "Staffordshire Bull Terrier", (byte)2 },
                    { 197, "Standard Schnauzer", (byte)2 },
                    { 198, "Tibetan Mastiff", (byte)2 },
                    { 199, "Tibetan Spaniel", (byte)2 },
                    { 200, "Tibetan Terrier", (byte)2 },
                    { 201, "Vizsla", (byte)2 },
                    { 202, "Weimaraner", (byte)2 },
                    { 203, "Welsh Terrier", (byte)2 },
                    { 204, "West Highland White Terrier", (byte)2 },
                    { 205, "Whippet", (byte)2 },
                    { 206, "Wire Fox Terrier", (byte)2 },
                    { 207, "Yorkshire Terrier", (byte)2 },
                    { 208, "Mixed Breed", (byte)2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_ApplicationUserId",
                table: "Address",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryCode",
                table: "Address",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Location_AddressId",
                table: "Location",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_MateRequestStateId",
                table: "MateRequest",
                column: "MateRequestStateId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetMateOwnerId",
                table: "MateRequest",
                column: "PetMateOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetMateProfileId",
                table: "MateRequest",
                column: "PetMateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetOwnerId",
                table: "MateRequest",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MateRequest_PetProfileId",
                table: "MateRequest",
                column: "PetProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PetBreed_TypeId",
                table: "PetBreed",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_BreedId",
                table: "PetProfile",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_CreationDate",
                table: "PetProfile",
                column: "CreationDate");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_OwnerId",
                table: "PetProfile",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_Private",
                table: "PetProfile",
                column: "Private");

            migrationBuilder.CreateIndex(
                name: "IX_PetProfile_SexId",
                table: "PetProfile",
                column: "SexId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

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
                name: "IX_ServiceOffer_UserId_ServiceTypeId",
                table: "ServiceOffer",
                columns: new[] { "UserId", "ServiceTypeId" },
                unique: true);

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Litter");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "MateRequest");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ServiceOffer");

            migrationBuilder.DropTable(
                name: "UserProfileInfo");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "MateRequestState");

            migrationBuilder.DropTable(
                name: "PetProfile");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PetBreed");

            migrationBuilder.DropTable(
                name: "Sex");

            migrationBuilder.DropTable(
                name: "PetType");
        }
    }
}
