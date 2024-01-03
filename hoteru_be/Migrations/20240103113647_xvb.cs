using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class xvb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GuestStatuses",
                columns: new[] { "IdGuestStatus", "Title" },
                values: new object[] { 2, "Regular" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GuestStatuses",
                keyColumn: "IdGuestStatus",
                keyValue: 2);
        }
    }
}
