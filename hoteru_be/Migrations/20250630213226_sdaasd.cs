using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sdaasd : Migration
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
                    Sum = table.Column<float>(type: "real", nullable: false),
                    InDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GuestSurname = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                name: "Rooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    IdRoomStatus = table.Column<int>(type: "int", nullable: false),
                    IdRoomType = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_Rooms_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    IdService = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sum = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.IdService);
                    table.ForeignKey(
                        name: "FK_Services_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.NoAction);
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
                    IdGuest = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_Reservations_Guests_IdGuest",
                        column: x => x.IdGuest,
                        principalTable: "Guests",
                        principalColumn: "IdPerson",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ReservationServices",
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
                    table.PrimaryKey("PK_ReservationServices", x => x.IdReservationService);
                    table.ForeignKey(
                        name: "FK_ReservationServices_Reservations_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationServices_Services_IdService",
                        column: x => x.IdService,
                        principalTable: "Services",
                        principalColumn: "IdService",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "IdAddress", "City", "Country", "Postcode", "Street" },
                values: new object[,]
                {
                    { 1, "Warsaw", "Poland", "02-913", "Koszykowa 86" },
                    { 2, "Krakow", "Poland", "31-042", "Main Square 1" }
                });

            migrationBuilder.InsertData(
                table: "Bills",
                columns: new[] { "IdBill", "BookedBy", "Created", "GuestName", "GuestSurname", "InDate", "OutDate", "RoomNumber", "Sum" },
                values: new object[] { 1, "zxc", new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Noah", "Wilson", new DateTime(2025, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "108", 140f });

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
                values: new object[,]
                {
                    { 1, "VIP" },
                    { 2, "Regular" }
                });

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
                    { 2, 1, 300f },
                    { 3, 2, 500f },
                    { 4, 2, 1000f }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "IdHotel", "IdAddress", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Nobu" },
                    { 2, 2, "Grand Krakow" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "IdPerson", "Email", "IdHotel", "Name", "Surname" },
                values: new object[,]
                {
                    { 1, "helli@gmail.com", 1, "Gleb", "Ivanov" },
                    { 2, "test@gmail.com", 1, "Artur", "Morgan" },
                    { 3, "password@gmail.com", 1, "Mikolaj", "Sluzalek" },
                    { 4, "marston@gmail.com", 1, "Jack", "Marston" },
                    { 5, "anna.nowak@gmail.com", 1, "Anna", "Nowak" },
                    { 6, "john.doe@gmail.com", 1, "John", "Doe" },
                    { 7, "emma.smith@gmail.com", 1, "Emma", "Smith" },
                    { 8, "liam.brown@gmail.com", 1, "Liam", "Brown" },
                    { 9, "olivia.taylor@gmail.com", 1, "Olivia", "Taylor" },
                    { 10, "noah.wilson@gmail.com", 1, "Noah", "Wilson" },
                    { 11, "sophia.martinez@gmail.com", 1, "Sophia", "Martinez" },
                    { 12, "james.anderson@gmail.com", 1, "James", "Anderson" },
                    { 13, "isabella.thomas@gmail.com", 1, "Isabella", "Thomas" },
                    { 14, "william.lee@gmail.com", 1, "William", "Lee" },
                    { 15, "jan.kowalski@grandkrakow.pl", 2, "Jan", "Kowalski" }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "IdPerson", "IdGuestStatus", "Passport", "TelNumber" },
                values: new object[,]
                {
                    { 5, 1, "PL111111", "500100100" },
                    { 6, 1, "PL222222", "500200200" },
                    { 7, 1, "PL333333", "500300300" },
                    { 8, 1, "PL444444", "500400400" },
                    { 9, 1, "PL555555", "500500500" },
                    { 10, 1, "PL666666", "500600600" },
                    { 11, 1, "PL777777", "500700700" },
                    { 12, 1, "PL888888", "500800800" },
                    { 13, 1, "PL999999", "500900900" },
                    { 14, 1, "PL000000", "501000000" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "IdPerson", "IdUserType", "LoginName", "Password" },
                values: new object[,]
                {
                    { 1, 3, "asd", "AQAAAAEAACcQAAAAEJunn01F8InkwA3s5UmJeDNm+7oc9bBue4HftYt26KB5wusHnnCHpRLynWhQeBsldA==" },
                    { 2, 1, "qwe", "AQAAAAEAACcQAAAAEICoXzMwe+5LQpNMHJ9xEsMYZiyVq4kyrIabFuBt2/BsE6XIdwdsn7Ix3pYtLVZArQ==" },
                    { 3, 2, "zxc", "AQAAAAEAACcQAAAAEHlv7ELDN76gkuMFVbat1+yHM+NR013pVL1SXY4+aYORlcApNaEoX33ZXJhZU6CAVQ==" },
                    { 4, 3, "qaz", "AQAAAAEAACcQAAAAEE69t49gGUi3mfUZdq9NYEgY84I2oG9t9KLonzFLjeXimp3t+ZdPufmjFp/JuJP9bg==" },
                    { 15, 1, "jan", "AQAAAAEAACcQAAAAEOo/kz63BA27/UpNrJX44IQqU2KaOlNvYXmzbSBbwIAQ/Ark0EqXJM/rH3/HS99Gnw==" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "IdRoomStatus", "IdRoomType", "IdUser", "Number", "Price" },
                values: new object[,]
                {
                    { 1, 1, 3, 1, 2, "101", 45f },
                    { 28, 28, 3, 2, 15, "401", 145f },
                    { 29, 29, 3, 2, 15, "402", 150f },
                    { 30, 30, 3, 2, 15, "403", 125f },
                    { 31, 31, 3, 2, 15, "404", 135f },
                    { 32, 32, 3, 2, 15, "405", 100f },
                    { 33, 33, 3, 2, 15, "406", 110f },
                    { 34, 34, 3, 2, 15, "407", 95f },
                    { 25, 25, 3, 2, 15, "307", 85f },
                    { 24, 24, 3, 2, 15, "306", 95f },
                    { 23, 23, 3, 2, 15, "305", 135f },
                    { 22, 22, 3, 2, 15, "304", 125f },
                    { 21, 21, 3, 2, 15, "303", 115f },
                    { 20, 20, 3, 2, 2, "302", 110f },
                    { 19, 19, 3, 2, 2, "301", 100f },
                    { 18, 18, 3, 1, 2, "209", 135f },
                    { 17, 17, 3, 1, 2, "208", 140f },
                    { 16, 16, 3, 1, 2, "207", 105f },
                    { 2, 2, 2, 1, 2, "102", 75f },
                    { 3, 3, 2, 1, 2, "103", 60f },
                    { 4, 4, 3, 1, 2, "104", 90f },
                    { 5, 5, 2, 1, 2, "105", 85f },
                    { 6, 6, 2, 1, 2, "106", 65f },
                    { 7, 7, 2, 1, 2, "107", 55f },
                    { 27, 27, 3, 2, 15, "309", 140f },
                    { 8, 8, 1, 1, 2, "108", 70f },
                    { 10, 10, 2, 1, 2, "201", 95f },
                    { 11, 11, 3, 1, 2, "202", 100f },
                    { 12, 12, 3, 1, 2, "203", 120f },
                    { 13, 13, 3, 1, 2, "204", 130f },
                    { 14, 14, 3, 1, 2, "205", 75f },
                    { 15, 15, 3, 1, 2, "206", 85f },
                    { 9, 9, 2, 1, 2, "109", 110f },
                    { 26, 26, 3, 2, 15, "308", 125f }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "IdUser", "Sum", "Title" },
                values: new object[,]
                {
                    { 28, "Certified staff", 15, 220f, "Babysitter" },
                    { 25, "Cake, balloons", 15, 400f, "Birthday Package" },
                    { 26, "Romantic setup", 15, 500f, "Anniversary Package" },
                    { 27, "Local wines", 15, 280f, "Wine Tasting" },
                    { 23, "Express delivery", 2, 90f, "Courier Service" },
                    { 24, "In-hotel shoot", 15, 300f, "Photo Session" },
                    { 22, "On-site translator", 2, 180f, "Translation" },
                    { 12, "1-hour massage", 2, 95f, "Massage" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "IdUser", "Sum", "Title" },
                values: new object[,]
                {
                    { 20, "Printing & scanning", 2, 50f, "Business Center" },
                    { 1, "None", 2, 355.5f, "Breakfast" },
                    { 2, "None", 2, 120.5f, "Spa" },
                    { 3, "None", 2, 248.5f, "Assistent" },
                    { 4, "Daily laundry service", 2, 40f, "Laundry" },
                    { 5, "Underground parking", 2, 75f, "Parking" },
                    { 6, "Extra room cleaning", 2, 30f, "Room Cleaning" },
                    { 7, "Mini bar refill", 2, 60f, "Mini Bar" },
                    { 8, "2-hour guided tour", 2, 150f, "City Tour" },
                    { 9, "24/7 access", 2, 45f, "Gym Access" },
                    { 10, "Outdoor pool", 2, 55f, "Pool Access" },
                    { 11, "Private sauna session", 2, 65f, "Sauna" },
                    { 29, "Luxury car", 15, 600f, "Car Rental" },
                    { 13, "Buffet dinner", 2, 300f, "Dinner" },
                    { 14, "Until 18:00", 2, 80f, "Late Checkout" },
                    { 15, "From 6:00 AM", 2, 70f, "Early Check-in" },
                    { 16, "Pet-friendly room", 2, 100f, "Pet Stay" },
                    { 17, "Luxury car", 2, 200f, "Airport Pickup" },
                    { 18, "Porter assistance", 2, 20f, "Baggage Service" },
                    { 19, "Premium internet", 2, 15f, "WiFi" },
                    { 21, "Per hour", 2, 250f, "Conference Room" },
                    { 30, "Per day", 15, 60f, "Bike Rental" }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "IdReservation", "Capacity", "Confirmed", "IdBill", "IdDeposit", "IdGuest", "IdRoom", "IdUser", "In", "Out", "Price" },
                values: new object[,]
                {
                    { 1, 1, true, null, 1, 5, 2, 1, new DateTime(2025, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 150f },
                    { 2, 2, true, null, 2, 6, 3, 2, new DateTime(2025, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 180f },
                    { 3, 3, true, null, 3, 7, 5, 3, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 340f },
                    { 4, 4, true, null, 4, 8, 6, 3, new DateTime(2025, 6, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 325f },
                    { 5, 1, true, null, null, 9, 7, 3, new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 330f },
                    { 6, 2, true, 1, null, 10, 8, 3, new DateTime(2025, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 140f },
                    { 7, 3, false, null, null, 6, 9, 3, new DateTime(2025, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 330f },
                    { 8, 4, false, null, null, 11, 10, 3, new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 380f }
                });

            migrationBuilder.InsertData(
                table: "ReservationServices",
                columns: new[] { "IdReservationService", "Date", "IdReservation", "IdService" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 2, new DateTime(2023, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                    { 3, new DateTime(2023, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3 },
                    { 4, new DateTime(2023, 11, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_IdDepositType",
                table: "Deposits",
                column: "IdDepositType");

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
                name: "IX_Reservations_IdGuest",
                table: "Reservations",
                column: "IdGuest");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdRoom",
                table: "Reservations",
                column: "IdRoom");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdUser",
                table: "Reservations",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationServices_IdReservation",
                table: "ReservationServices",
                column: "IdReservation");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationServices_IdService",
                table: "ReservationServices",
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
                name: "IX_Rooms_IdUser",
                table: "Rooms",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Services_IdUser",
                table: "Services",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUserType",
                table: "Users",
                column: "IdUserType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationServices");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "DepositTypes");

            migrationBuilder.DropTable(
                name: "GuestStatuses");

            migrationBuilder.DropTable(
                name: "RoomStatuses");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "Users");

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
