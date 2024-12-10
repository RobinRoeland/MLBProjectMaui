using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseballCalcASP.Migrations
{
    /// <inheritdoc />
    public partial class AddStatisticScores3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "mScoreName",
                table: "Statistics",
                newName: "ScoreName");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7f24737a-7e7c-44c2-95cd-6aa4ab039ca4", null, "admin", "admin" },
                    { "887d894e-277d-48b8-a687-e438c8d39896", null, "user", "user" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "deleted" },
                values: new object[,]
                {
                    { "59fc3ad2-a0f0-45f3-8a28-94eab2716284", 0, "157b59d4-5415-4851-9b0f-a7a9d271f849", "AppUser", "user2@testemail.com", true, "User2", "AppUser2", false, null, "USER2@TESTEMAIL.COM", "USER2@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEJBKXnf3626IK4Q1cX+rOW9Y+mUbuOoot9QUmfbqVxt9bSCsegvJTHG280jC+LfiWQ==", null, true, "b8c60cbe-3e24-454d-82a1-7432aa991f57", false, "user2@testemail.com", false },
                    { "ce7f78cf-35a4-4387-a3c5-2055f2e808f2", 0, "5ed0b212-f9ca-4520-8c99-2c261ac4443d", "AppUser", "user1@testemail.com", true, "User1", "AppUser1", false, null, "USER1@TESTEMAIL.COM", "USER1@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEIE/mn0hd6H0NBjPwp+fEM8LdU0PHpE0iKTXKxNPLTizFf0nP2/i9kdBBzl63pnmtg==", null, true, "7471a1ab-cc94-4a77-b5ed-26135bb555a3", false, "user1@testemail.com", false },
                    { "fe24deb4-ee0d-439d-8f62-e4ad999f50e2", 0, "feb90567-4e01-41ba-a852-d7e7681bfa43", "AppUser", "admin@testemail.com", true, "System", "Administrator", false, null, "ADMIN@TESTEMAIL.COM", "ADMIN@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEJqJOpWodcDCTzO+C2XXJYAEi5nH0klIkkc6pGRxJrvIBQY553iWdr+vSBvSuTDTyA==", null, true, "57a64bbe-5826-4775-aea9-3db57cd2a586", false, "admin@testemail.com", false }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "887d894e-277d-48b8-a687-e438c8d39896", "59fc3ad2-a0f0-45f3-8a28-94eab2716284" },
                    { "887d894e-277d-48b8-a687-e438c8d39896", "ce7f78cf-35a4-4387-a3c5-2055f2e808f2" },
                    { "7f24737a-7e7c-44c2-95cd-6aa4ab039ca4", "fe24deb4-ee0d-439d-8f62-e4ad999f50e2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "887d894e-277d-48b8-a687-e438c8d39896", "59fc3ad2-a0f0-45f3-8a28-94eab2716284" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "887d894e-277d-48b8-a687-e438c8d39896", "ce7f78cf-35a4-4387-a3c5-2055f2e808f2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7f24737a-7e7c-44c2-95cd-6aa4ab039ca4", "fe24deb4-ee0d-439d-8f62-e4ad999f50e2" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f24737a-7e7c-44c2-95cd-6aa4ab039ca4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "887d894e-277d-48b8-a687-e438c8d39896");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "59fc3ad2-a0f0-45f3-8a28-94eab2716284");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ce7f78cf-35a4-4387-a3c5-2055f2e808f2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "fe24deb4-ee0d-439d-8f62-e4ad999f50e2");

            migrationBuilder.RenameColumn(
                name: "ScoreName",
                table: "Statistics",
                newName: "mScoreName");

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
    }
}
