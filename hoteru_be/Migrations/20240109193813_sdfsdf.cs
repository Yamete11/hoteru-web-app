using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sdfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Reservations_IdReservation",
                table: "ReservationService");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Services_IdService",
                table: "ReservationService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationService",
                table: "ReservationService");

            migrationBuilder.RenameTable(
                name: "ReservationService",
                newName: "ReservationServices");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationService_IdService",
                table: "ReservationServices",
                newName: "IX_ReservationServices_IdService");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationService_IdReservation",
                table: "ReservationServices",
                newName: "IX_ReservationServices_IdReservation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationServices",
                table: "ReservationServices",
                column: "IdReservationService");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                column: "IdUserType",
                value: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationServices_Reservations_IdReservation",
                table: "ReservationServices",
                column: "IdReservation",
                principalTable: "Reservations",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationServices_Services_IdService",
                table: "ReservationServices",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "IdService",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationServices_Reservations_IdReservation",
                table: "ReservationServices");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationServices_Services_IdService",
                table: "ReservationServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationServices",
                table: "ReservationServices");

            migrationBuilder.RenameTable(
                name: "ReservationServices",
                newName: "ReservationService");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationServices_IdService",
                table: "ReservationService",
                newName: "IX_ReservationService_IdService");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationServices_IdReservation",
                table: "ReservationService",
                newName: "IX_ReservationService_IdReservation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationService",
                table: "ReservationService",
                column: "IdReservationService");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                column: "IdUserType",
                value: 3);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationService_Reservations_IdReservation",
                table: "ReservationService",
                column: "IdReservation",
                principalTable: "Reservations",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationService_Services_IdService",
                table: "ReservationService",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "IdService",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
