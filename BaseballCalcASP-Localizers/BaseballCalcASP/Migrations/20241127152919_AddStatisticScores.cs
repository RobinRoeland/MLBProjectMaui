using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseballCalcASP.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "330f145d-d59d-4c24-b42a-1a77e5df146a", "8fcd1d02-b6a8-4bb2-9904-5d11c60d8b22" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "330f145d-d59d-4c24-b42a-1a77e5df146a", "c3231a02-e79f-4cf9-a471-586e7f1346e9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "fc8e4299-acd7-4856-b4ec-d6555efff42f", "f0c95914-f150-49b8-8951-0e204a55dcd1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "330f145d-d59d-4c24-b42a-1a77e5df146a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fc8e4299-acd7-4856-b4ec-d6555efff42f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8fcd1d02-b6a8-4bb2-9904-5d11c60d8b22");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c3231a02-e79f-4cf9-a471-586e7f1346e9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f0c95914-f150-49b8-8951-0e204a55dcd1");

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Inning = table.Column<int>(type: "int", nullable: false),
                    InningTop = table.Column<int>(type: "int", nullable: false),
                    PersonMLBId = table.Column<int>(type: "int", nullable: false),
                    ScoreIdInInning = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => new { x.GameId, x.Inning, x.InningTop, x.PersonMLBId, x.ScoreIdInInning });
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1397b04c-fc40-425e-a84d-7710747d6444", null, "user", "user" },
                    { "58308036-c9ee-45f7-ab08-ac79836c3837", null, "admin", "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "deleted" },
                values: new object[,]
                {
                    { "7b67422e-23d4-42bf-8442-8e9b927e16d7", 0, "347e4106-8ba4-4bc4-b60f-95a376c73cf3", "AppUser", "user2@testemail.com", true, "User2", "AppUser2", false, null, "USER2@TESTEMAIL.COM", "USER2@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEGC9a3Jr+heSmDpzOIxiDF6XS4JSbORq4HTsvxfyMUzLC8fZf3tp3LI2scR5T2Totw==", null, true, "7a54dd7d-373e-4b70-b23e-b2eb20a276c3", false, "user2@testemail.com", false },
                    { "896f9a66-8150-4203-bb63-8df62c2b0648", 0, "44a9100b-87d5-411d-a0a8-f5f0878ab195", "AppUser", "admin@testemail.com", true, "System", "Administrator", false, null, "ADMIN@TESTEMAIL.COM", "ADMIN@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEEhF0CkB5rKTDdskkh1ZfSIOR05NvpGFwpf6Y0x90XfolUoJF5YVSwvb773InyXVTw==", null, true, "b28c06ad-d4b4-415d-8936-433cda967001", false, "admin@testemail.com", false },
                    { "f44a23c4-f598-4d74-8f80-d1345fff8755", 0, "447eac11-f28c-406f-b808-88bb468227cb", "AppUser", "user1@testemail.com", true, "User1", "AppUser1", false, null, "USER1@TESTEMAIL.COM", "USER1@TESTEMAIL.COM", "AQAAAAIAAYagAAAAECKi7ywaYQTTOCxFjuaLhvcGS3/GR5ZG/n2mU4N1HbSeHVyODWwpnQYtL8ToZvNffQ==", null, true, "7825eba9-b9f4-4281-baae-3dc38f0f5e91", false, "user1@testemail.com", false }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1397b04c-fc40-425e-a84d-7710747d6444", "7b67422e-23d4-42bf-8442-8e9b927e16d7" },
                    { "58308036-c9ee-45f7-ab08-ac79836c3837", "896f9a66-8150-4203-bb63-8df62c2b0648" },
                    { "1397b04c-fc40-425e-a84d-7710747d6444", "f44a23c4-f598-4d74-8f80-d1345fff8755" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1397b04c-fc40-425e-a84d-7710747d6444", "7b67422e-23d4-42bf-8442-8e9b927e16d7" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "58308036-c9ee-45f7-ab08-ac79836c3837", "896f9a66-8150-4203-bb63-8df62c2b0648" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1397b04c-fc40-425e-a84d-7710747d6444", "f44a23c4-f598-4d74-8f80-d1345fff8755" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1397b04c-fc40-425e-a84d-7710747d6444");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58308036-c9ee-45f7-ab08-ac79836c3837");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7b67422e-23d4-42bf-8442-8e9b927e16d7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "896f9a66-8150-4203-bb63-8df62c2b0648");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f44a23c4-f598-4d74-8f80-d1345fff8755");

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
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "330f145d-d59d-4c24-b42a-1a77e5df146a", "8fcd1d02-b6a8-4bb2-9904-5d11c60d8b22" },
                    { "330f145d-d59d-4c24-b42a-1a77e5df146a", "c3231a02-e79f-4cf9-a471-586e7f1346e9" },
                    { "fc8e4299-acd7-4856-b4ec-d6555efff42f", "f0c95914-f150-49b8-8951-0e204a55dcd1" }
                });
        }
    }
}
