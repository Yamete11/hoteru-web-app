using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class dfgdfgghj : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdReservation",
                table: "GuestReservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservation_IdReservation",
                table: "GuestReservation",
                column: "IdReservation");

            migrationBuilder.AddForeignKey(
                name: "FK_GuestReservation_Reservation_IdReservation",
                table: "GuestReservation",
                column: "IdReservation",
                principalTable: "Reservation",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestReservation_Reservation_IdReservation",
                table: "GuestReservation");

            migrationBuilder.DropIndex(
                name: "IX_GuestReservation_IdReservation",
                table: "GuestReservation");

            migrationBuilder.DropColumn(
                name: "IdReservation",
                table: "GuestReservation");
        }
    }
}
