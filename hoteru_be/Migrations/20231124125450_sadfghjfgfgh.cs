using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sadfghjfgfgh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservations_Guests_Id_Guest",
                table: "GuestReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservations_Reservations_Id_Reservation",
                table: "GuestReservations");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservations_Guests_Id_Guest",
                table: "GuestReservations",
                column: "Id_Guest",
                principalTable: "Guests",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservations_Reservations_Id_Reservation",
                table: "GuestReservations",
                column: "Id_Reservation",
                principalTable: "Reservations",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservations_Guests_Id_Guest",
                table: "GuestReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservations_Reservations_Id_Reservation",
                table: "GuestReservations");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservations_Guests_Id_Guest",
                table: "GuestReservations",
                column: "Id_Guest",
                principalTable: "Guests",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservations_Reservations_Id_Reservation",
                table: "GuestReservations",
                column: "Id_Reservation",
                principalTable: "Reservations",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
