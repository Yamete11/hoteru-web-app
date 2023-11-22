using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class dfgdfg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GuestReservation_IdGuest",
                table: "GuestReservation",
                column: "IdGuest");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservation_Guests_IdGuest",
                table: "GuestReservation",
                column: "IdGuest",
                principalTable: "Guests",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservation_Guests_IdGuest",
                table: "GuestReservation");

            migrationBuilder.DropIndex(
                name: "IX_GuestReservation_IdGuest",
                table: "GuestReservation");
        }
    }
}
