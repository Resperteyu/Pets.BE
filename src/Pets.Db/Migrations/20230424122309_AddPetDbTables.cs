using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pets.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPetDbTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Verified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordReset = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "char(2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DialCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false)
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
                    Longitude = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    GeoLocation = table.Column<Point>(type: "geography", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
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
                    Title = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sex", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.AccountId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryCode = table.Column<string>(type: "char(2)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    PhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profile_Country_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Country",
                        principalColumn: "CountryCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profile_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetBreed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeId = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
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
                name: "PetProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SexId = table.Column<byte>(type: "tinyint", nullable: false),
                    BreedId = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    AvailableForBreeding = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetProfile_PetBreed_BreedId",
                        column: x => x.BreedId,
                        principalTable: "PetBreed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetProfile_Profile_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetProfile_Sex_SexId",
                        column: x => x.SexId,
                        principalTable: "Sex",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "CountryCode", "DialCode", "Name" },
                values: new object[] { "GB", "+44", "United Kingdom" });

            migrationBuilder.InsertData(
                table: "PetType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)1, "Cats" },
                    { (byte)2, "Dogs" }
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
                    { 101, "Appenzeller Sennenhunde", (byte)2 },
                    { 102, "Australian Cattle Dog", (byte)2 },
                    { 103, "Australian Kelpie", (byte)2 },
                    { 104, "Australian Shepherd", (byte)2 },
                    { 105, "Australian Terrier", (byte)2 },
                    { 106, "Azawakh", (byte)2 },
                    { 107, "Barbet", (byte)2 },
                    { 108, "Basenji", (byte)2 },
                    { 109, "Basset Fauve de Bretagne", (byte)2 },
                    { 110, "Basset Hound", (byte)2 },
                    { 111, "Bavarian Mountain Scent Hound", (byte)2 },
                    { 112, "Beagle", (byte)2 },
                    { 113, "Bearded Collie", (byte)2 },
                    { 114, "Beauceron", (byte)2 },
                    { 115, "Bedlington Terrier", (byte)2 },
                    { 116, "Belgian Laekenois", (byte)2 },
                    { 117, "Belgian Malinois", (byte)2 },
                    { 118, "Belgian Sheepdog", (byte)2 },
                    { 119, "Belgian Tervuren", (byte)2 },
                    { 120, "Bergamasco", (byte)2 },
                    { 121, "Berger Picard", (byte)2 },
                    { 122, "Bernese Mountain Dog", (byte)2 },
                    { 123, "Bichon Frise", (byte)2 },
                    { 124, "Biewer Terrier", (byte)2 },
                    { 125, "Black and Tan Coonhound", (byte)2 },
                    { 126, "Black Russian Terrier", (byte)2 },
                    { 127, "Bloodhound", (byte)2 },
                    { 128, "Bluetick Coonhound", (byte)2 },
                    { 129, "Boerboel", (byte)2 },
                    { 130, "Bolognese", (byte)2 },
                    { 131, "Border Collie", (byte)2 },
                    { 132, "Border Terrier", (byte)2 },
                    { 133, "Borzoi", (byte)2 },
                    { 134, "Boston Terrier", (byte)2 },
                    { 135, "Bouvier des Flandres", (byte)2 },
                    { 136, "Boxer", (byte)2 },
                    { 137, "Boykin Spaniel", (byte)2 },
                    { 138, "Bracco Italiano", (byte)2 },
                    { 139, "Braque du Bourbonnais", (byte)2 },
                    { 140, "Briard", (byte)2 },
                    { 141, "Brittany", (byte)2 },
                    { 142, "Broholmer", (byte)2 },
                    { 143, "Brussels Griffon", (byte)2 },
                    { 144, "Bull Terrier", (byte)2 },
                    { 145, "Bulldog", (byte)2 },
                    { 146, "Bullmastiff", (byte)2 },
                    { 147, "Cairn Terrier", (byte)2 },
                    { 148, "Canaan Dog", (byte)2 },
                    { 149, "Cane Corso", (byte)2 },
                    { 150, "Cardigan Welsh Corgi", (byte)2 },
                    { 151, "Carolina Dog", (byte)2 },
                    { 152, "Catahoula Leopard Dog", (byte)2 },
                    { 153, "Caucasian Shepherd Dog", (byte)2 },
                    { 154, "Cavalier King Charles Spaniel", (byte)2 },
                    { 155, "Central Asian Shepherd Dog", (byte)2 },
                    { 156, "Cesky Terrier", (byte)2 },
                    { 157, "Chesapeake Bay Retriever", (byte)2 },
                    { 158, "Chihuahua", (byte)2 },
                    { 159, "Chinese Crested", (byte)2 },
                    { 160, "Chinese Shar-Pei", (byte)2 },
                    { 161, "Chinook", (byte)2 },
                    { 162, "Chow Chow", (byte)2 },
                    { 163, "Cirneco del Etna", (byte)2 },
                    { 164, "Clumber Spaniel", (byte)2 },
                    { 165, "Cocker Spaniel", (byte)2 },
                    { 166, "Collie", (byte)2 },
                    { 167, "Coton de Tulear", (byte)2 },
                    { 168, "Curly-Coated Retriever", (byte)2 },
                    { 169, "Czechoslovakian Vlcak", (byte)2 },
                    { 170, "Dachshund", (byte)2 },
                    { 171, "Dalmatian", (byte)2 },
                    { 172, "Dandie Dinmont Terrier", (byte)2 },
                    { 173, "Danish-Swedish Farmdog", (byte)2 },
                    { 174, "Deutscher Wachtelhund", (byte)2 },
                    { 175, "Doberman Pinscher", (byte)2 },
                    { 176, "Dogo Argentino", (byte)2 },
                    { 177, "Dogue de Bordeaux", (byte)2 },
                    { 178, "Drentsche Patrijshond", (byte)2 },
                    { 179, "Drever", (byte)2 },
                    { 180, "Dutch Shepherd", (byte)2 },
                    { 181, "Dutch Smoushond", (byte)2 },
                    { 182, "East Siberian Laika", (byte)2 },
                    { 183, "English Cocker Spaniel", (byte)2 },
                    { 184, "English Foxhound", (byte)2 },
                    { 185, "English Setter", (byte)2 },
                    { 186, "English Springer Spaniel", (byte)2 },
                    { 187, "English Toy Spaniel", (byte)2 },
                    { 188, "Entlebucher", (byte)2 },
                    { 189, "Estrela Mountain Dog", (byte)2 },
                    { 190, "Eurasier", (byte)2 },
                    { 191, "Field Spaniel", (byte)2 },
                    { 192, "Finnish Lapphund", (byte)2 },
                    { 193, "Finnish Spitz", (byte)2 },
                    { 194, "Flat-Coated Retriever", (byte)2 },
                    { 195, "French Bulldog", (byte)2 },
                    { 196, "French Spaniel", (byte)2 },
                    { 197, "German Longhaired Pointer", (byte)2 },
                    { 198, "German Pinscher", (byte)2 },
                    { 199, "German Shepherd Dog", (byte)2 },
                    { 200, "German Shorthaired Pointer", (byte)2 },
                    { 201, "German Spitz", (byte)2 },
                    { 202, "German Wirehaired Pointer", (byte)2 },
                    { 203, "Giant Schnauzer", (byte)2 },
                    { 204, "Glen of Imaal Terrier", (byte)2 },
                    { 205, "Golden Retriever", (byte)2 },
                    { 206, "Goldendoodle", (byte)2 },
                    { 207, "Gordon Setter", (byte)2 },
                    { 208, "Grand Basset Griffon Vendeen", (byte)2 },
                    { 209, "Grand Bleu de Gascogne", (byte)2 },
                    { 210, "Grand Griffon Vendeen", (byte)2 },
                    { 211, "Great Pyrenees", (byte)2 },
                    { 212, "Greater Swiss Mountain Dog", (byte)2 },
                    { 213, "Greyhound", (byte)2 },
                    { 214, "Hamiltonstovare", (byte)2 },
                    { 215, "Harrier", (byte)2 },
                    { 216, "Havanese", (byte)2 },
                    { 217, "Hokkaido", (byte)2 },
                    { 218, "Hovawart", (byte)2 },
                    { 219, "Ibizan Hound", (byte)2 },
                    { 220, "Icelandic Sheepdog", (byte)2 },
                    { 221, "Irish Red and White Setter", (byte)2 },
                    { 222, "Irish Setter", (byte)2 },
                    { 223, "Irish Terrier", (byte)2 },
                    { 224, "Irish Water Spaniel", (byte)2 },
                    { 225, "Great Dane", (byte)2 },
                    { 226, "Irish Wolfhound", (byte)2 },
                    { 227, "Italian Greyhound", (byte)2 },
                    { 228, "Jagdterrier", (byte)2 },
                    { 229, "Japanese Chin", (byte)2 },
                    { 230, "Japanese Spitz", (byte)2 },
                    { 231, "Jindo", (byte)2 },
                    { 232, "Kai Ken", (byte)2 },
                    { 233, "Karelian Bear Dog", (byte)2 },
                    { 234, "Keeshond", (byte)2 },
                    { 235, "Kerry Blue Terrier", (byte)2 },
                    { 236, "Kishu Ken", (byte)2 },
                    { 237, "Komondor", (byte)2 },
                    { 238, "Kooikerhondje", (byte)2 },
                    { 239, "Kromfohrlander", (byte)2 },
                    { 240, "Labradoodle", (byte)2 },
                    { 241, "Labrador Retriever", (byte)2 },
                    { 242, "Lagotto Romagnolo", (byte)2 },
                    { 243, "Lakeland Terrier", (byte)2 },
                    { 244, "Kuvasz", (byte)2 },
                    { 245, "Lancashire Heeler", (byte)2 },
                    { 246, "Landseer", (byte)2 },
                    { 247, "Large Munsterlander", (byte)2 },
                    { 248, "Leonberger", (byte)2 },
                    { 249, "Lhasa Apso", (byte)2 },
                    { 250, "Lowchen", (byte)2 },
                    { 251, "Maltese", (byte)2 },
                    { 252, "Manchester Terrier", (byte)2 },
                    { 253, "Maremma Sheepdog", (byte)2 },
                    { 254, "Mastiff", (byte)2 },
                    { 255, "Miniature American Shepherd", (byte)2 },
                    { 256, "Miniature Bull Terrier", (byte)2 },
                    { 257, "Miniature Pinscher", (byte)2 },
                    { 258, "Miniature Schnauzer", (byte)2 },
                    { 259, "Mudi", (byte)2 },
                    { 260, "Neapolitan Mastiff", (byte)2 },
                    { 261, "Newfoundland", (byte)2 },
                    { 262, "Norfolk Terrier", (byte)2 },
                    { 263, "Norwegian Buhund", (byte)2 },
                    { 264, "Norwegian Elkhound", (byte)2 },
                    { 265, "Norwegian Lundehund", (byte)2 },
                    { 266, "Norwich Terrier", (byte)2 },
                    { 267, "Nova Scotia Duck Tolling Retriever", (byte)2 },
                    { 268, "Old English Sheepdog", (byte)2 },
                    { 269, "Otterhound", (byte)2 },
                    { 270, "Papillon", (byte)2 },
                    { 271, "Parson Russell Terrier", (byte)2 },
                    { 272, "Pekingese", (byte)2 },
                    { 273, "Pembroke Welsh Corgi", (byte)2 },
                    { 274, "Perro de Presa Canario", (byte)2 },
                    { 275, "Peruvian Inca Orchid", (byte)2 },
                    { 276, "Petit Basset Griffon Vendeen", (byte)2 },
                    { 277, "Pharaoh Hound", (byte)2 },
                    { 278, "Plott Hound", (byte)2 },
                    { 279, "Pointer", (byte)2 },
                    { 280, "Polish Lowland Sheepdog", (byte)2 },
                    { 281, "Pomeranian", (byte)2 },
                    { 282, "Poodle", (byte)2 },
                    { 283, "Porcelaine", (byte)2 },
                    { 284, "Portuguese Podengo", (byte)2 },
                    { 285, "Portuguese Pointer", (byte)2 },
                    { 286, "Portuguese Water Dog", (byte)2 },
                    { 287, "Pudelpointer", (byte)2 },
                    { 288, "Pug", (byte)2 },
                    { 289, "Puli", (byte)2 },
                    { 290, "Pumi", (byte)2 },
                    { 291, "Pyrenean Mastiff", (byte)2 },
                    { 292, "Pyrenean Shepherd", (byte)2 },
                    { 293, "Rafeiro do Alentejo", (byte)2 },
                    { 294, "Rat Terrier", (byte)2 },
                    { 295, "Redbone Coonhound", (byte)2 },
                    { 296, "Rhodesian Ridgeback", (byte)2 },
                    { 297, "Romanian Mioritic Shepherd Dog", (byte)2 },
                    { 298, "Rottweiler", (byte)2 },
                    { 299, "Russell Terrier", (byte)2 },
                    { 300, "Russian Toy", (byte)2 },
                    { 301, "Russian Tsvetn", (byte)2 },
                    { 302, "Saint Bernard", (byte)2 },
                    { 303, "Saluki", (byte)2 },
                    { 304, "Samoyed", (byte)2 },
                    { 305, "Schapendoes", (byte)2 },
                    { 306, "Schipperke", (byte)2 },
                    { 307, "Scottish Deerhound", (byte)2 },
                    { 308, "Scottish Terrier", (byte)2 },
                    { 309, "Sealyham Terrier", (byte)2 },
                    { 310, "Shar Pei", (byte)2 },
                    { 311, "Shetland Sheepdog", (byte)2 },
                    { 312, "Shiba Inu", (byte)2 },
                    { 313, "Shih Tzu", (byte)2 },
                    { 314, "Shikoku", (byte)2 },
                    { 315, "Siberian Husky", (byte)2 },
                    { 316, "Silky Terrier", (byte)2 },
                    { 317, "Skye Terrier", (byte)2 },
                    { 318, "Sloughi", (byte)2 },
                    { 319, "Slovakian Wirehaired Pointer", (byte)2 },
                    { 320, "Slovensky Cuvac", (byte)2 },
                    { 321, "Slovensky Kopov", (byte)2 },
                    { 322, "Small Munsterlander", (byte)2 },
                    { 323, "Smooth Fox Terrier", (byte)2 },
                    { 324, "Soft Coated Wheaten Terrier", (byte)2 },
                    { 325, "Spanish Mastiff", (byte)2 },
                    { 326, "Spanish Water Dog", (byte)2 },
                    { 327, "Spinone Italiano", (byte)2 },
                    { 328, "Stabyhoun", (byte)2 },
                    { 329, "Staffordshire Bull Terrier", (byte)2 },
                    { 330, "Standard Schnauzer", (byte)2 },
                    { 331, "Sussex Spaniel", (byte)2 },
                    { 332, "Swedish Lapphund", (byte)2 },
                    { 333, "Swedish Vallhund", (byte)2 },
                    { 334, "Taiwan Dog", (byte)2 },
                    { 335, "Teddy Roosevelt Terrier", (byte)2 },
                    { 336, "Thai Ridgeback", (byte)2 },
                    { 337, "Tibetan Mastiff", (byte)2 },
                    { 338, "Tibetan Spaniel", (byte)2 },
                    { 339, "Tibetan Terrier", (byte)2 },
                    { 340, "Tornjak", (byte)2 },
                    { 341, "Tosa", (byte)2 },
                    { 342, "Toy American Eskimo Dog", (byte)2 },
                    { 343, "Toy Fox Terrier", (byte)2 },
                    { 344, "Toy Poodle", (byte)2 },
                    { 345, "Treeing Tennessee Brindle", (byte)2 },
                    { 346, "Treeing Walker Coonhound", (byte)2 }
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
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "PetBreed");

            migrationBuilder.DropTable(
                name: "Profile");

            migrationBuilder.DropTable(
                name: "Sex");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "PetType");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
