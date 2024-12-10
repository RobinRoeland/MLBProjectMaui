using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseballCalcASP.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticScores2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "DefensivePlay",
                table: "Statistics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "ScoreValue",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "mScoreName",
                table: "Statistics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af", null, "user", "user" },
                    { "f83f1dda-eac3-4e53-ab5e-7911c3db60f5", null, "admin", "admin" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "deleted" },
                values: new object[,]
                {
                    { "20a58fed-588f-44fc-b131-cd7174c083ca", 0, "9ca34626-21e9-4629-af81-de6a35592ee1", "AppUser", "user2@testemail.com", true, "User2", "AppUser2", false, null, "USER2@TESTEMAIL.COM", "USER2@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEKqyHQOtl113IQ6mmnywytZXJcrFG+gBPDBRaQPEqKjXseHfRNchf5ssSQE42nxzDA==", null, true, "ce203680-6c37-442f-a15b-f2785ed7b1e2", false, "user2@testemail.com", false },
                    { "7d42485e-b709-4be9-a868-5f63b393b2b7", 0, "4eb1ccc5-9b77-4479-bc5f-2bae0134d670", "AppUser", "admin@testemail.com", true, "System", "Administrator", false, null, "ADMIN@TESTEMAIL.COM", "ADMIN@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEOKx1SNle23JhcTl+k8kONd7V4ytWYnhlST27MZLm9T4qyZtNtYkghZTXrSsCxw8Vw==", null, true, "ec8fbcd7-50a9-47c8-b442-3ef25823cd27", false, "admin@testemail.com", false },
                    { "904519b3-14f3-4ec5-8aa5-5ca137c401da", 0, "00a00f8b-29d6-4ba0-af3b-248d4add49a7", "AppUser", "user1@testemail.com", true, "User1", "AppUser1", false, null, "USER1@TESTEMAIL.COM", "USER1@TESTEMAIL.COM", "AQAAAAIAAYagAAAAENdvJRMhHlUZoOwb/ApDHLcnsUYvCeeEqaUaWz4g9MBXORMxnKyIIObtrgmsoRUYQg==", null, true, "691d4e42-b088-4454-b02e-88df0060ac29", false, "user1@testemail.com", false }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af", "20a58fed-588f-44fc-b131-cd7174c083ca" },
                    { "f83f1dda-eac3-4e53-ab5e-7911c3db60f5", "7d42485e-b709-4be9-a868-5f63b393b2b7" },
                    { "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af", "904519b3-14f3-4ec5-8aa5-5ca137c401da" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af", "20a58fed-588f-44fc-b131-cd7174c083ca" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f83f1dda-eac3-4e53-ab5e-7911c3db60f5", "7d42485e-b709-4be9-a868-5f63b393b2b7" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af", "904519b3-14f3-4ec5-8aa5-5ca137c401da" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f00e05c3-8b6d-4e23-b7e6-ed4a3b3704af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f83f1dda-eac3-4e53-ab5e-7911c3db60f5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20a58fed-588f-44fc-b131-cd7174c083ca");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7d42485e-b709-4be9-a868-5f63b393b2b7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "904519b3-14f3-4ec5-8aa5-5ca137c401da");

            migrationBuilder.DropColumn(
                name: "DefensivePlay",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "ScoreValue",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "mScoreName",
                table: "Statistics");

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
    }
}
