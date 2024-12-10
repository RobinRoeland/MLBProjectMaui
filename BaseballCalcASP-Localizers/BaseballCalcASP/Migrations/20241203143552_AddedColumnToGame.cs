using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BaseballCalcASP.Migrations
{
    /// <inheritdoc />
    public partial class AddedColumnToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "TotalInnings",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4c3bfba1-07d8-4e0d-81a5-7f86283b553a", null, "admin", "admin" },
                    { "df5f2b62-2c08-44d4-9645-34bc4d77be31", null, "user", "user" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "deleted" },
                values: new object[,]
                {
                    { "19457cb3-d31f-4323-8f13-36c6c8ae5b9d", 0, "b497a5ab-efd3-415d-8b72-828aa7a98388", "AppUser", "admin@testemail.com", true, "System", "Administrator", false, null, "ADMIN@TESTEMAIL.COM", "ADMIN@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEEgh8pAhpRgOYEJhH569xRt2XF6Aj3pcw8C2sKOi/YolbdGn+5bXEsFJB7hBF+w9Qw==", null, true, "08d33a99-9da5-4b90-b414-537a77c5b1f5", false, "admin@testemail.com", false },
                    { "a5c49ac5-934c-473b-8ae9-099558a92717", 0, "164b811f-a37f-4d2f-8247-a45adf85d37e", "AppUser", "user1@testemail.com", true, "User1", "AppUser1", false, null, "USER1@TESTEMAIL.COM", "USER1@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEPBgXb5FDr5dO7N2daHT9ty4yUYevCDqweIpdWS4WhMbcrBFOIPFmA5YXSeheS7+zg==", null, true, "b96d728d-8ee7-4b6c-a695-2080f1a6e0b0", false, "user1@testemail.com", false },
                    { "b159c0a2-f42f-41bd-a5b5-88e146f04123", 0, "17fec767-e9d6-45be-8376-fdfe1f1bbdf1", "AppUser", "user2@testemail.com", true, "User2", "AppUser2", false, null, "USER2@TESTEMAIL.COM", "USER2@TESTEMAIL.COM", "AQAAAAIAAYagAAAAEKN4fxM9d2h2IDnkCQraji5VnWaDZ6eNBqqNU2sTlMY4v57TnJoQzDiu+/2Fm/j+eQ==", null, true, "4102a313-e848-4124-bc63-220eab5b6055", false, "user2@testemail.com", false }
                });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 1,
                column: "TotalInnings",
                value: 9);

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "4c3bfba1-07d8-4e0d-81a5-7f86283b553a", "19457cb3-d31f-4323-8f13-36c6c8ae5b9d" },
                    { "df5f2b62-2c08-44d4-9645-34bc4d77be31", "a5c49ac5-934c-473b-8ae9-099558a92717" },
                    { "df5f2b62-2c08-44d4-9645-34bc4d77be31", "b159c0a2-f42f-41bd-a5b5-88e146f04123" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4c3bfba1-07d8-4e0d-81a5-7f86283b553a", "19457cb3-d31f-4323-8f13-36c6c8ae5b9d" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "df5f2b62-2c08-44d4-9645-34bc4d77be31", "a5c49ac5-934c-473b-8ae9-099558a92717" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "df5f2b62-2c08-44d4-9645-34bc4d77be31", "b159c0a2-f42f-41bd-a5b5-88e146f04123" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4c3bfba1-07d8-4e0d-81a5-7f86283b553a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df5f2b62-2c08-44d4-9645-34bc4d77be31");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "19457cb3-d31f-4323-8f13-36c6c8ae5b9d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a5c49ac5-934c-473b-8ae9-099558a92717");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b159c0a2-f42f-41bd-a5b5-88e146f04123");

            migrationBuilder.DropColumn(
                name: "TotalInnings",
                table: "Games");

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
    }
}
