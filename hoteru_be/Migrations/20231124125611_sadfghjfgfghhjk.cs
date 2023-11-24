using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sadfghjfgfghhjk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_DepositTypes_IdDepositType",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_GuestStatuses_IdGuestStatus",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Persons_IdPerson",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Addresses_IdAddress",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Hotels_IdHotel",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Deposits_IdDeposit",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_IdUser",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Reservations_IdReservation",
                table: "ReservationService");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Services_IdService",
                table: "ReservationService");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypes_IdRoomType",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_IdPerson",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_IdUserType",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_DepositTypes_IdDepositType",
                table: "Deposits",
                column: "IdDepositType",
                principalTable: "DepositTypes",
                principalColumn: "IdDepositType",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_GuestStatuses_IdGuestStatus",
                table: "Guests",
                column: "IdGuestStatus",
                principalTable: "GuestStatuses",
                principalColumn: "IdGuestStatus",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Persons_IdPerson",
                table: "Guests",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Addresses_IdAddress",
                table: "Hotels",
                column: "IdAddress",
                principalTable: "Addresses",
                principalColumn: "IdAddress",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Hotels_IdHotel",
                table: "Persons",
                column: "IdHotel",
                principalTable: "Hotels",
                principalColumn: "IdHotel",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Deposits_IdDeposit",
                table: "Reservations",
                column: "IdDeposit",
                principalTable: "Deposits",
                principalColumn: "IdDeposit",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "IdRoom",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_IdUser",
                table: "Reservations",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationService_Reservations_IdReservation",
                table: "ReservationService",
                column: "IdReservation",
                principalTable: "Reservations",
                principalColumn: "IdReservation",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationService_Services_IdService",
                table: "ReservationService",
                column: "IdService",
                principalTable: "Services",
                principalColumn: "IdService",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                table: "Rooms",
                column: "IdRoomStatus",
                principalTable: "RoomStatuses",
                principalColumn: "IdRoomStatus",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTypes_IdRoomType",
                table: "Rooms",
                column: "IdRoomType",
                principalTable: "RoomTypes",
                principalColumn: "IdRoomType",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_IdPerson",
                table: "Users",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_IdUserType",
                table: "Users",
                column: "IdUserType",
                principalTable: "UserTypes",
                principalColumn: "IdUserType",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deposits_DepositTypes_IdDepositType",
                table: "Deposits");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_GuestStatuses_IdGuestStatus",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Persons_IdPerson",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Addresses_IdAddress",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Hotels_IdHotel",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Deposits_IdDeposit",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_IdUser",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Reservations_IdReservation",
                table: "ReservationService");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationService_Services_IdService",
                table: "ReservationService");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypes_IdRoomType",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_IdPerson",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserTypes_IdUserType",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Deposits_DepositTypes_IdDepositType",
                table: "Deposits",
                column: "IdDepositType",
                principalTable: "DepositTypes",
                principalColumn: "IdDepositType",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_GuestStatuses_IdGuestStatus",
                table: "Guests",
                column: "IdGuestStatus",
                principalTable: "GuestStatuses",
                principalColumn: "IdGuestStatus",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Persons_IdPerson",
                table: "Guests",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Addresses_IdAddress",
                table: "Hotels",
                column: "IdAddress",
                principalTable: "Addresses",
                principalColumn: "IdAddress",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Hotels_IdHotel",
                table: "Persons",
                column: "IdHotel",
                principalTable: "Hotels",
                principalColumn: "IdHotel",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Deposits_IdDeposit",
                table: "Reservations",
                column: "IdDeposit",
                principalTable: "Deposits",
                principalColumn: "IdDeposit",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_IdRoom",
                table: "Reservations",
                column: "IdRoom",
                principalTable: "Rooms",
                principalColumn: "IdRoom",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_IdUser",
                table: "Reservations",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                table: "Rooms",
                column: "IdRoomStatus",
                principalTable: "RoomStatuses",
                principalColumn: "IdRoomStatus",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTypes_IdRoomType",
                table: "Rooms",
                column: "IdRoomType",
                principalTable: "RoomTypes",
                principalColumn: "IdRoomType",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_IdPerson",
                table: "Users",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "IdPerson",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserTypes_IdUserType",
                table: "Users",
                column: "IdUserType",
                principalTable: "UserTypes",
                principalColumn: "IdUserType",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
