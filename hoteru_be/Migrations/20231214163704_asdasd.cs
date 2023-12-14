using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class asdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 3,
                columns: new[] { "LoginName", "Password" },
                values: new object[] { "asd", "asd" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                columns: new[] { "LoginName", "Password" },
                values: new object[] { "qwe", "qwe" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 3,
                columns: new[] { "LoginName", "Password" },
                values: new object[] { "Login1", "123123123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                columns: new[] { "LoginName", "Password" },
                values: new object[] { "Login2", "567567567" });
        }
    }
}
