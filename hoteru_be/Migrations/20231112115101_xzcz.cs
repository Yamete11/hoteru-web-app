using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class xzcz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomStatus_IdRoomStatus",
                table: "Room");

            migrationBuilder.DropForeignKey(
                name: "FK_Room_RoomType_IdRoomType",
                table: "Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomType",
                table: "RoomType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomStatus",
                table: "RoomStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Room",
                table: "Room");

            migrationBuilder.RenameTable(
                name: "RoomType",
                newName: "RoomTypes");

            migrationBuilder.RenameTable(
                name: "RoomStatus",
                newName: "RoomStatuses");

            migrationBuilder.RenameTable(
                name: "Room",
                newName: "Rooms");

            migrationBuilder.RenameIndex(
                name: "IX_Room_IdRoomType",
                table: "Rooms",
                newName: "IX_Rooms_IdRoomType");

            migrationBuilder.RenameIndex(
                name: "IX_Room_IdRoomStatus",
                table: "Rooms",
                newName: "IX_Rooms_IdRoomStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomTypes",
                table: "RoomTypes",
                column: "IdRoomType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomStatuses",
                table: "RoomStatuses",
                column: "IdRoomStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "IdRoom");

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
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
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

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "IdUserType", "Title" },
                values: new object[] { 1, "Superadmin" });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "IdUserType", "Title" },
                values: new object[] { 2, "Admin" });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "IdUserType", "Title" },
                values: new object[] { 3, "Employee" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUserType",
                table: "Users",
                column: "IdUserType",
                unique: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomStatuses_IdRoomStatus",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypes_IdRoomType",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomTypes",
                table: "RoomTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomStatuses",
                table: "RoomStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "RoomTypes",
                newName: "RoomType");

            migrationBuilder.RenameTable(
                name: "RoomStatuses",
                newName: "RoomStatus");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Room");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_IdRoomType",
                table: "Room",
                newName: "IX_Room_IdRoomType");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_IdRoomStatus",
                table: "Room",
                newName: "IX_Room_IdRoomStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomType",
                table: "RoomType",
                column: "IdRoomType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomStatus",
                table: "RoomStatus",
                column: "IdRoomStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Room",
                table: "Room",
                column: "IdRoom");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomStatus_IdRoomStatus",
                table: "Room",
                column: "IdRoomStatus",
                principalTable: "RoomStatus",
                principalColumn: "IdRoomStatus",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_RoomType_IdRoomType",
                table: "Room",
                column: "IdRoomType",
                principalTable: "RoomType",
                principalColumn: "IdRoomType",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
