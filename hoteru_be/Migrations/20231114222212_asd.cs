﻿using Microsoft.EntityFrameworkCore.Migrations;

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
                name: "Guests",
                columns: table => new
                {
                    IdGuest = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.IdGuest);
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
                name: "Rooms",
                columns: table => new
                {
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_IdUserType",
                        column: x => x.IdUserType,
                        principalTable: "UserTypes",
                        principalColumn: "IdUserType",
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
                values: new object[] { 1, "Regular" });

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
                table: "Rooms",
                columns: new[] { "IdRoom", "Capacity", "IdRoomStatus", "IdRoomType", "Number", "Price" },
                values: new object[,]
                {
                    { 1, 4, 1, 1, "101", 3.5f },
                    { 32, 2, 2, 1, "201", 3.5f },
                    { 31, 4, 1, 1, "101", 3.5f },
                    { 30, 1, 1, 1, "305", 3.5f },
                    { 29, 2, 3, 1, "203", 3.5f },
                    { 28, 2, 2, 1, "201", 3.5f },
                    { 27, 4, 1, 1, "101", 3.5f },
                    { 26, 1, 1, 1, "305", 3.5f },
                    { 25, 2, 3, 1, "203", 3.5f },
                    { 24, 1, 1, 1, "305", 3.5f },
                    { 23, 2, 3, 1, "203", 3.5f },
                    { 22, 2, 2, 1, "201", 3.5f },
                    { 21, 4, 1, 1, "101", 3.5f },
                    { 20, 1, 1, 1, "305", 3.5f },
                    { 19, 2, 3, 1, "203", 3.5f },
                    { 18, 2, 2, 1, "201", 3.5f },
                    { 17, 4, 1, 1, "101", 3.5f },
                    { 16, 1, 1, 1, "305", 3.5f },
                    { 2, 2, 2, 1, "201", 3.5f },
                    { 3, 2, 3, 1, "203", 3.5f },
                    { 4, 1, 1, 1, "305", 3.5f },
                    { 5, 4, 1, 1, "101", 3.5f },
                    { 6, 2, 2, 1, "201", 3.5f },
                    { 7, 2, 3, 1, "203", 3.5f },
                    { 33, 2, 3, 1, "203", 3.5f },
                    { 8, 1, 1, 1, "305", 3.5f },
                    { 10, 2, 2, 1, "201", 3.5f },
                    { 11, 2, 3, 1, "203", 3.5f },
                    { 12, 1, 1, 1, "305", 3.5f },
                    { 13, 4, 1, 1, "101", 3.5f },
                    { 14, 2, 2, 1, "201", 3.5f },
                    { 15, 2, 3, 1, "203", 3.5f },
                    { 9, 4, 1, 1, "101", 3.5f },
                    { 34, 1, 1, 1, "305", 3.5f }
                });

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
                column: "IdUserType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "RoomStatuses");

            migrationBuilder.DropTable(
                name: "RoomTypes");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}