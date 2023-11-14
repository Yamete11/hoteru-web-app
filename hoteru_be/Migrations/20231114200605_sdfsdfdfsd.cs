using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sdfsdfdfsd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "IdRoomStatus", "IdRoomType", "Number", "Price" },
                values: new object[,]
                {
                    { 25, 2, 3, 1, "203", 3.5f },
                    { 26, 1, 1, 1, "305", 3.5f },
                    { 27, 4, 1, 1, "101", 3.5f },
                    { 28, 2, 2, 1, "201", 3.5f },
                    { 29, 2, 3, 1, "203", 3.5f },
                    { 30, 1, 1, 1, "305", 3.5f },
                    { 31, 4, 1, 1, "101", 3.5f },
                    { 32, 2, 2, 1, "201", 3.5f },
                    { 33, 2, 3, 1, "203", 3.5f },
                    { 34, 1, 1, 1, "305", 3.5f }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 34);
        }
    }
}
