using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class fsdfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_IdUserType",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "GuestStatusIdGuestStatus",
                table: "Guests",
                type: "int",
                nullable: true);

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

            migrationBuilder.InsertData(
                table: "DepositTypes",
                columns: new[] { "IdDepositType", "Title" },
                values: new object[] { 1, "Card" });

            migrationBuilder.InsertData(
                table: "DepositTypes",
                columns: new[] { "IdDepositType", "Title" },
                values: new object[] { 2, "Cash" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUserType",
                table: "Users",
                column: "IdUserType");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_GuestStatusIdGuestStatus",
                table: "Guests",
                column: "GuestStatusIdGuestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Deposits_IdDepositType",
                table: "Deposits",
                column: "IdDepositType");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_GuestStatuses_GuestStatusIdGuestStatus",
                table: "Guests",
                column: "GuestStatusIdGuestStatus",
                principalTable: "GuestStatuses",
                principalColumn: "IdGuestStatus",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_GuestStatuses_GuestStatusIdGuestStatus",
                table: "Guests");

            migrationBuilder.DropTable(
                name: "Deposits");

            migrationBuilder.DropTable(
                name: "GuestStatuses");

            migrationBuilder.DropTable(
                name: "DepositTypes");

            migrationBuilder.DropIndex(
                name: "IX_Users_IdUserType",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Guests_GuestStatusIdGuestStatus",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "GuestStatusIdGuestStatus",
                table: "Guests");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdUserType",
                table: "Users",
                column: "IdUserType",
                unique: true);
        }
    }
}
