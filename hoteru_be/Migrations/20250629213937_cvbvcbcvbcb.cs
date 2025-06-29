using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class cvbvcbcvbcb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "IdAddress", "City", "Country", "Postcode", "Street" },
                values: new object[] { 2, "Krakow", "Poland", "31-042", "Main Square 1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 1,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEJunn01F8InkwA3s5UmJeDNm+7oc9bBue4HftYt26KB5wusHnnCHpRLynWhQeBsldA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 2,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEICoXzMwe+5LQpNMHJ9xEsMYZiyVq4kyrIabFuBt2/BsE6XIdwdsn7Ix3pYtLVZArQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 3,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEHlv7ELDN76gkuMFVbat1+yHM+NR013pVL1SXY4+aYORlcApNaEoX33ZXJhZU6CAVQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                column: "Password",
                value: "AQAAAAEAACcQAAAAEE69t49gGUi3mfUZdq9NYEgY84I2oG9t9KLonzFLjeXimp3t+ZdPufmjFp/JuJP9bg==");

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "IdHotel", "IdAddress", "Title" },
                values: new object[] { 2, 2, "Grand Krakow" });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "IdPerson", "Email", "IdHotel", "Name", "Surname" },
                values: new object[] { 15, "jan.kowalski@grandkrakow.pl", 2, "Jan", "Kowalski" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "IdPerson", "IdUserType", "LoginName", "Password" },
                values: new object[] { 15, 1, "jan", "AQAAAAEAACcQAAAAEOo/kz63BA27/UpNrJX44IQqU2KaOlNvYXmzbSBbwIAQ/Ark0EqXJM/rH3/HS99Gnw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "IdPerson",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "IdHotel",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Addresses",
                keyColumn: "IdAddress",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 1,
                column: "Password",
                value: "asd");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 2,
                column: "Password",
                value: "qwe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 3,
                column: "Password",
                value: "zxc");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "IdPerson",
                keyValue: 4,
                column: "Password",
                value: "qaz");
        }
    }
}
