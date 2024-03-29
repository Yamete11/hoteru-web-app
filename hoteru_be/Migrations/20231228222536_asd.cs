﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class asd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    IdAddress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.IdAddress);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    IdBill = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sum = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.IdBill);
                });

            migrationBuilder.CreateTable(
                name: "DepositTypes",
                columns: table => new
                {
                    IdDepositType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositTypes", x => x.IdDepositType);
                });

            migrationBuilder.CreateTable(
                name: "GuestStatuses",
                columns: table => new
                {
                    IdGuestStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestStatuses", x => x.IdGuestStatus);
                });

            migrationBuilder.CreateTable(
                name: "RoomStatuses",
                columns: table => new
                {
                    IdRoomStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomStatuses", x => x.IdRoomStatus);
                });

            migrationBuilder.CreateTable(
                name: "RoomTypes",
                columns: table => new
                {
                    IdRoomType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomTypes", x => x.IdRoomType);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    IdService = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sum = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.IdService);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    IdUserType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.IdUserType);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    IdHotel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdAddress = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.IdHotel);
                    table.ForeignKey(
                        name: "FK_Hotels_Addresses_IdAddress",
                        column: x => x.IdAddress,
                        principalTable: "Addresses",
                        principalColumn: "IdAddress",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deposits",
                columns: table => new
                {
                    IdDeposit = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sum = table.Column<float>(type: "real", nullable: false),
                    IdDepositType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposits", x => x.IdDeposit);
                    table.ForeignKey(
                        name: "FK_Deposits_DepositTypes_IdDepositType",
                        column: x => x.IdDepositType,
                        principalTable: "DepositTypes",
                        principalColumn: "IdDepositType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    IdRoomStatus = table.Column<int>(type: "int", nullable: false),
                    IdRoomType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.IdRoom);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                        column: x => x.IdRoomStatus,
                        principalTable: "RoomStatuses",
                        principalColumn: "IdRoomStatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rooms_RoomTypes_IdRoomType",
                        column: x => x.IdRoomType,
                        principalTable: "RoomTypes",
                        principalColumn: "IdRoomType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    IdPerson = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdHotel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.IdPerson);
                    table.ForeignKey(
                        name: "FK_Persons_Hotels_IdHotel",
                        column: x => x.IdHotel,
                        principalTable: "Hotels",
                        principalColumn: "IdHotel",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    Passport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdGuestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.IdPerson);
                    table.ForeignKey(
                        name: "FK_Guests_GuestStatuses_IdGuestStatus",
                        column: x => x.IdGuestStatus,
                        principalTable: "GuestStatuses",
                        principalColumn: "IdGuestStatus",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Guests_Persons_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Persons",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    LoginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdPerson);
                    table.ForeignKey(
                        name: "FK_Users_Persons_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Persons",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_IdUserType",
                        column: x => x.IdUserType,
                        principalTable: "UserTypes",
                        principalColumn: "IdUserType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    IdReservation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    In = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Out = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false),
                    IdDeposit = table.Column<int>(type: "int", nullable: true),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    IdBill = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.IdReservation);
                    table.ForeignKey(
                        name: "FK_Reservations_Bills_IdBill",
                        column: x => x.IdBill,
                        principalTable: "Bills",
                        principalColumn: "IdBill",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Deposits_IdDeposit",
                        column: x => x.IdDeposit,
                        principalTable: "Deposits",
                        principalColumn: "IdDeposit",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Rooms_IdRoom",
                        column: x => x.IdRoom,
                        principalTable: "Rooms",
                        principalColumn: "IdRoom",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuestReservations",
                columns: table => new
                {
                    IdGuestReservation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReservation = table.Column<int>(type: "int", nullable: false),
                    IdGuest = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestReservations", x => x.IdGuestReservation);
                    table.ForeignKey(
                        name: "FK_GuestReservations_Guests_IdGuest",
                        column: x => x.IdGuest,
                        principalTable: "Guests",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuestReservations_Reservations_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ReservationService",
                columns: table => new
                {
                    IdReservationService = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdService = table.Column<int>(type: "int", nullable: false),
                    IdReservation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationService", x => x.IdReservationService);
                    table.ForeignKey(
                        name: "FK_ReservationService_Reservations_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationService_Services_IdService",
                        column: x => x.IdService,
                        principalTable: "Services",
                        principalColumn: "IdService",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "IdAddress", "City", "Country", "Postcode", "Street" },
                values: new object[] { 1, "Warsaw", "Poland", "02-913", "Koszykowa 86" });

            migrationBuilder.InsertData(
                table: "Bills",
                columns: new[] { "IdBill", "Created", "Sum" },
                values: new object[] { 1, new DateTime(2023, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 300f });

            migrationBuilder.InsertData(
                table: "DepositTypes",
                columns: new[] { "IdDepositType", "Title" },
                values: new object[,]
                {
                    { 1, "Card" },
                    { 2, "Cash" }
                });

            migrationBuilder.InsertData(
                table: "GuestStatuses",
                columns: new[] { "IdGuestStatus", "Title" },
                values: new object[] { 1, "VIP" });

            migrationBuilder.InsertData(
                table: "RoomStatuses",
                columns: new[] { "IdRoomStatus", "Title" },
                values: new object[,]
                {
                    { 1, "Out of service" },
                    { 2, "Occupied" },
                    { 3, "Ready" }
                });

            migrationBuilder.InsertData(
                table: "RoomTypes",
                columns: new[] { "IdRoomType", "Title" },
                values: new object[,]
                {
                    { 1, "Regular" },
                    { 2, "Luxury" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "Sum", "Title" },
                values: new object[,]
                {
                    { 1, "None", 355.5f, "Breakfast" },
                    { 2, "None", 120.5f, "Spa" },
                    { 3, "None", 248.5f, "Assistent" }
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "IdUserType", "Title" },
                values: new object[,]
                {
                    { 1, "Superadmin" },
                    { 2, "Admin" },
                    { 3, "Employee" }
                });

            migrationBuilder.InsertData(
                table: "Deposits",
                columns: new[] { "IdDeposit", "IdDepositType", "Sum" },
                values: new object[,]
                {
                    { 1, 1, 200f },
                    { 2, 1, 300f }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "IdHotel", "IdAddress", "Title" },
                values: new object[] { 1, 1, "Nobu" });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "IdRoomStatus", "IdRoomType", "Number", "Price" },
                values: new object[,]
                {
                    { 19, 19, 3, 1, "301", 3.5f },
                    { 20, 20, 1, 1, "302", 3.5f },
                    { 21, 21, 1, 1, "303", 3.5f },
                    { 22, 22, 2, 1, "304", 3.5f },
                    { 23, 23, 3, 1, "305", 3.5f },
                    { 24, 24, 1, 1, "306", 3.5f },
                    { 25, 25, 3, 1, "307", 3.5f },
                    { 26, 26, 1, 1, "308", 3.5f },
                    { 27, 27, 1, 1, "309", 3.5f },
                    { 28, 28, 2, 1, "401", 3.5f },
                    { 29, 29, 3, 1, "402", 3.5f },
                    { 30, 30, 1, 1, "403", 3.5f },
                    { 31, 31, 1, 1, "404", 3.5f },
                    { 32, 32, 2, 1, "405", 3.5f },
                    { 18, 18, 2, 1, "209", 3.5f },
                    { 17, 17, 1, 1, "208", 3.5f },
                    { 16, 16, 1, 1, "207", 3.5f },
                    { 15, 15, 3, 1, "206", 3.5f },
                    { 1, 1, 1, 1, "101", 3.5f },
                    { 2, 2, 2, 1, "102", 3.5f },
                    { 3, 3, 3, 1, "103", 3.5f },
                    { 4, 4, 1, 1, "104", 3.5f },
                    { 5, 5, 1, 1, "105", 3.5f },
                    { 6, 6, 2, 1, "106", 3.5f },
                    { 33, 33, 3, 1, "406", 3.5f },
                    { 7, 7, 3, 1, "107", 3.5f },
                    { 9, 9, 1, 1, "109", 3.5f },
                    { 10, 10, 2, 1, "201", 3.5f },
                    { 11, 11, 3, 1, "202", 3.5f },
                    { 12, 12, 1, 1, "203", 3.5f },
                    { 13, 13, 1, 1, "204", 3.5f },
                    { 14, 14, 2, 1, "205", 3.5f },
                    { 8, 8, 1, 1, "108", 3.5f },
                    { 34, 34, 1, 1, "407", 3.5f }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "IdPerson", "Email", "IdHotel", "Name", "Surname" },
                values: new object[,]
                {
                    { 1, "helli@gmail.com", 1, "Gleb", "Ivanov" },
                    { 2, "test@gmail.com", 1, "Artur", "Morgan" },
                    { 3, "password@gmail.com", 1, "Mikolaj", "Sluzalek" },
                    { 4, "marston@gmail.com", 1, "Jack", "Marston" }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "IdPerson", "IdGuestStatus", "Passport", "TelNumber" },
                values: new object[,]
                {
                    { 1, 1, "FV124124", "123123123" },
                    { 2, 1, "BG121231", "567567567" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "IdPerson", "IdUserType", "LoginName", "Password" },
                values: new object[,]
                {
                    { 3, 3, "asd", "asd" },
                    { 4, 3, "qwe", "qwe" }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "IdReservation", "Capacity", "Confirmed", "IdBill", "IdDeposit", "IdRoom", "IdUser", "In", "Out", "Price" },
                values: new object[] { 1, 2, true, null, 1, 1, 3, new DateTime(2023, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1f });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "IdReservation", "Capacity", "Confirmed", "IdBill", "IdDeposit", "IdRoom", "IdUser", "In", "Out", "Price" },
                values: new object[] { 3, 2, false, null, null, 1, 3, new DateTime(2023, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1f });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "IdReservation", "Capacity", "Confirmed", "IdBill", "IdDeposit", "IdRoom", "IdUser", "In", "Out", "Price" },
                values: new object[] { 2, 3, true, 1, 2, 2, 4, new DateTime(2023, 12, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 12, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 2f });

            migrationBuilder.InsertData(
                table: "GuestReservations",
                columns: new[] { "IdGuestReservation", "IdGuest", "IdReservation" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "ReservationService",
                columns: new[] { "IdReservationService", "Date", "IdReservation", "IdService" },
                values: new object[,]
                {
                    { 4, new DateTime(2023, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3 },
                    { 1, new DateTime(2023, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 2, new DateTime(2023, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                    { 3, new DateTime(2023, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_IdDepositType",
                table: "Deposits",
                column: "IdDepositType");

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservations_IdGuest",
                table: "GuestReservations",
                column: "IdGuest");

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservations_IdReservation",
                table: "GuestReservations",
                column: "IdReservation");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_IdGuestStatus",
                table: "Guests",
                column: "IdGuestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_IdAddress",
                table: "Hotels",
                column: "IdAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_IdHotel",
                table: "Persons",
                column: "IdHotel");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdBill",
                table: "Reservations",
                column: "IdBill",
                unique: true,
                filter: "[IdBill] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdDeposit",
                table: "Reservations",
                column: "IdDeposit",
                unique: true,
                filter: "[IdDeposit] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdRoom",
                table: "Reservations",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdUser",
                table: "Reservations",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationService_IdReservation",
                table: "ReservationService",
                column: "IdReservation");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationService_IdService",
                table: "ReservationService",
                column: "IdService");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdRoomStatus",
                table: "Rooms",
                column: "IdRoomStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_IdRoomType",
                table: "Rooms",
                column: "IdRoomType");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUserType",
                table: "Users",
                column: "IdUserType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestReservations");

            migrationBuilder.DropTable(
                name: "ReservationService");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "GuestStatuses");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DepositTypes");

            migrationBuilder.DropTable(
                name: "RoomStatuses");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
