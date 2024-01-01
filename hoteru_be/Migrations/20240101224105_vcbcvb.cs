using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class vcbcvb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "IdReservation",
                keyValue: 3,
                column: "IdRoom",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 1,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 2,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 3,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 4,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 5,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 6,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 8,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 9,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 10,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 12,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 13,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 14,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 16,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 17,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 18,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 19,
                column: "IdRoomType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 20,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 21,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 22,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 23,
                column: "IdRoomType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 24,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 25,
                column: "IdRoomType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 26,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 27,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 28,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 29,
                column: "IdRoomType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 30,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 31,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 32,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 33,
                column: "IdRoomType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 34,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 3, 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "IdReservation",
                keyValue: 3,
                column: "IdRoom",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 1,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 2,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 3,
                column: "IdRoomStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 4,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 5,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 6,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 8,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 9,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 10,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 12,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 13,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 14,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 16,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 17,
                column: "IdRoomStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 18,
                column: "IdRoomStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 19,
                column: "IdRoomType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 20,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 21,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 22,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 23,
                column: "IdRoomType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 24,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 25,
                column: "IdRoomType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 26,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 27,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 28,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 29,
                column: "IdRoomType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 30,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 31,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 32,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 33,
                column: "IdRoomType",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "IdRoom",
                keyValue: 34,
                columns: new[] { "IdRoomStatus", "IdRoomType" },
                values: new object[] { 1, 1 });
        }
    }
}
