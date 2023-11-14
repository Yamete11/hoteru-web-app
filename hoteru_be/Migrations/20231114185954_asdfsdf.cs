using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class asdfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "IdRoomStatus", "IdRoomType", "Number", "Price" },
                values: new object[,]
                {
                    { 9, 4, 1, 1, "101", 3.5f },
                    { 10, 2, 2, 1, "201", 3.5f },
                    { 11, 2, 3, 1, "203", 3.5f },
                    { 12, 1, 1, 1, "305", 3.5f },
                    { 13, 4, 1, 1, "101", 3.5f },
                    { 14, 2, 2, 1, "201", 3.5f },
                    { 15, 2, 3, 1, "203", 3.5f },
                    { 16, 1, 1, 1, "305", 3.5f },
                    { 17, 4, 1, 1, "101", 3.5f },
                    { 18, 2, 2, 1, "201", 3.5f },
                    { 19, 2, 3, 1, "203", 3.5f },
                    { 20, 1, 1, 1, "305", 3.5f },
                    { 21, 4, 1, 1, "101", 3.5f },
                    { 22, 2, 2, 1, "201", 3.5f },
                    { 23, 2, 3, 1, "203", 3.5f },
                    { 24, 1, 1, 1, "305", 3.5f }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 24);
        }
    }
}
