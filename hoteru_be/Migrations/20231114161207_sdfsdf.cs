using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class sdfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "Sum", "Title" },
                values: new object[] { 1, "None", 355.5f, "Breakfast" });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "Sum", "Title" },
                values: new object[] { 2, "None", 120.5f, "Spa" });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "Sum", "Title" },
                values: new object[] { 3, "None", 248.5f, "Assistent" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
