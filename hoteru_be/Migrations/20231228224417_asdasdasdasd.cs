using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class asdasdasdasd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GuestReservations",
                keyColumn: "IdGuestReservation",
                keyValue: 2,
                column: "IdReservation",
                value: 2);

            migrationBuilder.InsertData(
                table: "GuestReservations",
                columns: new[] { "IdGuestReservation", "IdGuest", "IdReservation" },
                values: new object[] { 3, 1, 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GuestReservations",
                keyColumn: "IdGuestReservation",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "GuestReservations",
                keyColumn: "IdGuestReservation",
                keyValue: 2,
                column: "IdReservation",
                value: 1);
        }
    }
}
