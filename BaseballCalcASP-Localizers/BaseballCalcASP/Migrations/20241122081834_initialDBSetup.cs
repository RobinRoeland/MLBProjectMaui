using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseballCalcASP.Migrations
{
    /// <inheritdoc />
    public partial class initialDBSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    HomeTeamId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamId = table.Column<int>(type: "int", nullable: false),
                    GameDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeStartingPitcherId = table.Column<int>(type: "int", nullable: false),
                    AwayStartingPitcherId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PlayerKey = table.Column<int>(type: "int", nullable: false),
                    GamesPlayed = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PlateAppearences = table.Column<int>(type: "int", nullable: false),
                    HStrikeOuts = table.Column<int>(type: "int", nullable: false),
                    Hits = table.Column<int>(type: "int", nullable: false),
                    Singles = table.Column<int>(type: "int", nullable: false),
                    Doubles = table.Column<int>(type: "int", nullable: false),
                    Triples = table.Column<int>(type: "int", nullable: false),
                    HomeRuns = table.Column<int>(type: "int", nullable: false),
                    BaseOnBalls = table.Column<int>(type: "int", nullable: false),
                    HitByPitch = table.Column<int>(type: "int", nullable: false),
                    SacrificeFlies = table.Column<int>(type: "int", nullable: false),
                    SacrificeHits = table.Column<int>(type: "int", nullable: false),
                    CaughtStealing = table.Column<int>(type: "int", nullable: false),
                    StolenBases = table.Column<int>(type: "int", nullable: false),
                    Runs = table.Column<int>(type: "int", nullable: false),
                    Errors = table.Column<int>(type: "int", nullable: false),
                    DoublePlays = table.Column<int>(type: "int", nullable: false),
                    TriplePlays = table.Column<int>(type: "int", nullable: false),
                    PassedBalls = table.Column<int>(type: "int", nullable: false),
                    PStrikeOuts = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPlayers = table.Column<int>(type: "int", nullable: false),
                    MLB_Org_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VenueName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeagueName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDisplayBrief = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FranchiseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rugnummer = table.Column<int>(type: "int", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    APILink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MLBPersonId = table.Column<int>(type: "int", nullable: true),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "330f145d-d59d-4c24-b42a-1a77e5df146a", null, "user", "user" },
                    { "fc8e4299-acd7-4856-b4ec-d6555efff42f", null, "admin", "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "deleted" },
                values: new object[,]
                {
                    { "8fcd1d02-b6a8-4bb2-9904-5d11c60d8b22", 0, "5c013b5f-310e-465c-a374-174f1f48b3b0", "AppUser", "user1@testemail.com", true, "User1", "AppUser1", false, null, "USER1@TESTEMAIL.COM", "USER1@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEGsT1jfGM4G4Es6ndEoQ6XqHpd3GMhNWqnoEFlm5y5zwpRHO0uXKqRFDj6HDHjp1Zw==", null, true, "2c30ce7c-52de-4066-b917-f90b30a512d7", false, "user1@testemail.com", false },
                    { "c3231a02-e79f-4cf9-a471-586e7f1346e9", 0, "6a06927e-1069-4c94-bef0-da077e7afd68", "AppUser", "user2@testemail.com", true, "User2", "AppUser2", false, null, "USER2@TESTEMAIL.COM", "USER2@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEFS0rhWUMqcRobj0LWgDWUaQZugmH09KsY+NW6mg03PxTeNeO3eHvsIHpawczCL5bg==", null, true, "9705a69a-795d-4ca8-92bb-d65edb3d493e", false, "user2@testemail.com", false },
                    { "f0c95914-f150-49b8-8951-0e204a55dcd1", 0, "5d1fd256-ac69-4978-a2d1-96db426cef75", "AppUser", "admin@testemail.com", true, "System", "Administrator", false, null, "ADMIN@TESTEMAIL.COM", "ADMIN@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEMoLOkfwjNLjPViIxHzJpov9sALZacTT2IJif4zS3Kob4T+UATO6XxJdGJyPWsxlyQ==", null, true, "66818887-4249-44cc-80dd-4e86b4eb3c7e", false, "admin@testemail.com", false }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AwayStartingPitcherId", "AwayTeamId", "GameDate", "GameTime", "HomeStartingPitcherId", "HomeTeamId" },
                values: new object[] { 1, 3, 4, "2008-12-04", "13:00", 1, 1 });

            migrationBuilder.InsertData(
                table: "Seasons",
                columns: new[] { "Id", "BaseOnBalls", "CaughtStealing", "Deleted", "DoublePlays", "Doubles", "Errors", "GamesPlayed", "HStrikeOuts", "HitByPitch", "Hits", "HomeRuns", "PStrikeOuts", "PassedBalls", "PlateAppearences", "PlayerKey", "Runs", "SacrificeFlies", "SacrificeHits", "Singles", "StolenBases", "TriplePlays", "Triples", "Year" },
                values: new object[,]
                {
                    { 1, 1, 2, false, 0, 2, 1, 3, 2, 1, 3, 0, 0, 0, 7, 8, 3, 0, 0, 1, 4, 0, 0, 2023 },
                    { 2, 1, 2, false, 0, 2, 1, 4, 4, 1, 2, 0, 0, 0, 7, 6, 3, 0, 0, 1, 1, 0, 0, 2023 }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "City", "Deleted", "FranchiseCode", "LeagueName", "MLB_Org_ID", "Name", "NameDisplayBrief", "TotalPlayers", "VenueName" },
                values: new object[,]
                {
                    { 1, null, false, null, null, null, "Waldos", null, 2, null },
                    { 2, null, false, null, null, null, "Bebops", null, 2, null },
                    { 3, null, false, null, null, null, "Foxes", null, 2, null },
                    { 4, null, false, null, null, null, "Kangeroos", null, 1, null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "330f145d-d59d-4c24-b42a-1a77e5df146a", "8fcd1d02-b6a8-4bb2-9904-5d11c60d8b22" },
                    { "330f145d-d59d-4c24-b42a-1a77e5df146a", "c3231a02-e79f-4cf9-a471-586e7f1346e9" },
                    { "fc8e4299-acd7-4856-b4ec-d6555efff42f", "f0c95914-f150-49b8-8951-0e204a55dcd1" }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "APILink", "DOB", "Deleted", "MLBPersonId", "Name", "Position", "Rugnummer", "TeamId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2004, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Waldo", null, null, 1 },
                    { 2, null, new DateTime(2004, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Waldo2", null, null, 1 },
                    { 3, null, new DateTime(2003, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Kangoeroe", null, null, 4 },
                    { 4, null, new DateTime(2006, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Test", null, null, 2 },
                    { 5, null, new DateTime(2006, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Test2", null, null, 2 },
                    { 6, null, new DateTime(2005, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "A1", null, null, 2 },
                    { 7, null, new DateTime(2006, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "A2", null, null, 3 },
                    { 8, null, new DateTime(2008, 12, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "A3", null, null, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");
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
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
