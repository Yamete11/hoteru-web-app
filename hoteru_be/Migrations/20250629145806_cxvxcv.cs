using Microsoft.EntityFrameworkCore.Migrations;

namespace hoteru_be.Migrations
{
    public partial class cxvxcv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "IdService", "Description", "Sum", "Title" },
                values: new object[,]
                {
                    { 4, "Daily laundry service", 40f, "Laundry" },
                    { 28, "Certified staff", 220f, "Babysitter" },
                    { 27, "Local wines", 280f, "Wine Tasting" },
                    { 26, "Romantic setup", 500f, "Anniversary Package" },
                    { 25, "Cake, balloons", 400f, "Birthday Package" },
                    { 24, "In-hotel shoot", 300f, "Photo Session" },
                    { 23, "Express delivery", 90f, "Courier Service" },
                    { 22, "On-site translator", 180f, "Translation" },
                    { 21, "Per hour", 250f, "Conference Room" },
                    { 20, "Printing & scanning", 50f, "Business Center" },
                    { 19, "Premium internet", 15f, "WiFi" },
                    { 18, "Porter assistance", 20f, "Baggage Service" },
                    { 29, "Luxury car", 600f, "Car Rental" },
                    { 17, "Luxury car", 200f, "Airport Pickup" },
                    { 15, "From 6:00 AM", 70f, "Early Check-in" },
                    { 14, "Until 18:00", 80f, "Late Checkout" },
                    { 13, "Buffet dinner", 300f, "Dinner" },
                    { 12, "1-hour massage", 95f, "Massage" },
                    { 11, "Private sauna session", 65f, "Sauna" },
                    { 10, "Outdoor pool", 55f, "Pool Access" },
                    { 9, "24/7 access", 45f, "Gym Access" },
                    { 8, "2-hour guided tour", 150f, "City Tour" },
                    { 7, "Mini bar refill", 60f, "Mini Bar" },
                    { 6, "Extra room cleaning", 30f, "Room Cleaning" },
                    { 5, "Underground parking", 75f, "Parking" },
                    { 16, "Pet-friendly room", 100f, "Pet Stay" },
                    { 30, "Per day", 60f, "Bike Rental" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "IdService",
                keyValue: 30);
        }
    }
}
