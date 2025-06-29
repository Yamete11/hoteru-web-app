using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class asdasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUser",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 1,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 2,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 3,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 4,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 5,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 6,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 7,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 8,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 9,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 10,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 11,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 12,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 13,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 14,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 15,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 16,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 17,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 18,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 19,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 20,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 21,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 22,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 23,
                column: "IdUser",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 24,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 25,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 26,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 27,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 28,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 29,
                column: "IdUser",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 30,
                column: "IdUser",
                value: 15);

            migrationBuilder.CreateIndex(
                name: "IX_Services_IdUser",
                table: "Services",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_IdUser",
                table: "Services",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_IdUser",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_IdUser",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IdUser",
                table: "Services");
        }
    }
}
