﻿using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace hoteru_be.Context
{
    public class MyDbContext : DbContext
    {
        
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<RoomStatus> RoomStatuses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<DepositType> DepositTypes { get; set; }
        public virtual DbSet<Deposit> Deposits { get; set; }
        public virtual DbSet<GuestStatus> GuestStatuses { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ReservationService> ReservationServices { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }



        public MyDbContext()
        {

        }

        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<RoomStatus>(t =>
            {
                t.HasData(
                    new RoomStatus { IdRoomStatus = 1, Title = "Out of service" },
                    new RoomStatus { IdRoomStatus = 2, Title = "Occupied" },
                    new RoomStatus { IdRoomStatus = 3, Title = "Ready" });

            });
            modelBuilder.Entity<UserType>(t =>
            {
                t.HasData(
                    new UserType { IdUserType = 1, Title = "Superadmin" },
                    new UserType { IdUserType = 2, Title = "Admin" },
                    new UserType { IdUserType = 3, Title = "Employee" });

            });

            modelBuilder.Entity<DepositType>(t =>
            {
                t.HasData(
                    new DepositType { IdDepositType = 1, Title = "Card" },
                    new DepositType { IdDepositType = 2, Title = "Cash" });
                   

            });

            modelBuilder.Entity<Service>(t =>
            {
                t.HasData(
                    new Service { IdService = 1, Title = "Breakfast", Sum = 355.5f, Description = "None", IdUser = 2 },
                    new Service { IdService = 2, Title = "Spa", Sum = 120.5f, Description = "None", IdUser = 2 },
                    new Service { IdService = 3, Title = "Assistent", Sum = 248.5f, Description = "None", IdUser = 2 },
                    new Service { IdService = 4, Title = "Laundry", Sum = 40.0f, Description = "Daily laundry service", IdUser = 2 },
                    new Service { IdService = 5, Title = "Parking", Sum = 75.0f, Description = "Underground parking", IdUser = 2 },
                    new Service { IdService = 6, Title = "Room Cleaning", Sum = 30.0f, Description = "Extra room cleaning", IdUser = 2 },
                    new Service { IdService = 7, Title = "Mini Bar", Sum = 60.0f, Description = "Mini bar refill", IdUser = 2 },
                    new Service { IdService = 8, Title = "City Tour", Sum = 150.0f, Description = "2-hour guided tour", IdUser = 2 },
                    new Service { IdService = 9, Title = "Gym Access", Sum = 45.0f, Description = "24/7 access", IdUser = 2 },
                    new Service { IdService = 10, Title = "Pool Access", Sum = 55.0f, Description = "Outdoor pool", IdUser = 2 },
                    new Service { IdService = 11, Title = "Sauna", Sum = 65.0f, Description = "Private sauna session", IdUser = 2 },
                    new Service { IdService = 12, Title = "Massage", Sum = 95.0f, Description = "1-hour massage", IdUser = 2 },
                    new Service { IdService = 13, Title = "Dinner", Sum = 300.0f, Description = "Buffet dinner", IdUser = 2 },
                    new Service { IdService = 14, Title = "Late Checkout", Sum = 80.0f, Description = "Until 18:00", IdUser = 2 },
                    new Service { IdService = 15, Title = "Early Check-in", Sum = 70.0f, Description = "From 6:00 AM", IdUser = 2 },
                    new Service { IdService = 16, Title = "Pet Stay", Sum = 100.0f, Description = "Pet-friendly room", IdUser = 2 },
                    new Service { IdService = 17, Title = "Airport Pickup", Sum = 200.0f, Description = "Luxury car", IdUser = 2 },
                    new Service { IdService = 18, Title = "Baggage Service", Sum = 20.0f, Description = "Porter assistance", IdUser = 2 },
                    new Service { IdService = 19, Title = "WiFi", Sum = 15.0f, Description = "Premium internet", IdUser = 2 },
                    new Service { IdService = 20, Title = "Business Center", Sum = 50.0f, Description = "Printing & scanning", IdUser = 2 },
                    new Service { IdService = 21, Title = "Conference Room", Sum = 250.0f, Description = "Per hour", IdUser = 2 },
                    new Service { IdService = 22, Title = "Translation", Sum = 180.0f, Description = "On-site translator", IdUser = 2 },
                    new Service { IdService = 23, Title = "Courier Service", Sum = 90.0f, Description = "Express delivery", IdUser = 2 },
                    new Service { IdService = 24, Title = "Photo Session", Sum = 300.0f, Description = "In-hotel shoot", IdUser = 15 },
                    new Service { IdService = 25, Title = "Birthday Package", Sum = 400.0f, Description = "Cake, balloons", IdUser = 15 },
                    new Service { IdService = 26, Title = "Anniversary Package", Sum = 500.0f, Description = "Romantic setup", IdUser = 15 },
                    new Service { IdService = 27, Title = "Wine Tasting", Sum = 280.0f, Description = "Local wines", IdUser = 15 },
                    new Service { IdService = 28, Title = "Babysitter", Sum = 220.0f, Description = "Certified staff", IdUser = 15 },
                    new Service { IdService = 29, Title = "Car Rental", Sum = 600.0f, Description = "Luxury car", IdUser = 15 },
                    new Service { IdService = 30, Title = "Bike Rental", Sum = 60.0f, Description = "Per day", IdUser = 15 }
                );
            });


            modelBuilder.Entity<RoomType>(t =>
            {
                t.HasData(
                    new RoomType { IdRoomType = 1, Title = "Regular" },
                    new RoomType { IdRoomType = 2, Title = "Luxury" });

            });

            modelBuilder.Entity<Address>(t =>
            {
                t.HasData(
                    new Address { IdAddress = 1, City = "Warsaw", Country = "Poland", Street = "Koszykowa 86", Postcode = "02-913" },
                    new Address { IdAddress = 2, City = "Krakow", Country = "Poland", Street = "Main Square 1", Postcode = "31-042" });


            });

            modelBuilder.Entity<Hotel>(t =>
            {
                t.HasData(
                    new Hotel { IdHotel = 1, Title = "Nobu", IdAddress = 1 },
                    new Hotel { IdHotel = 2, Title = "Grand Krakow", IdAddress = 2 });

            });

            modelBuilder.Entity<GuestStatus>(t =>
            {
                t.HasData(
                    new GuestStatus { IdGuestStatus = 1, Title = "VIP"},
                    new GuestStatus { IdGuestStatus = 2, Title = "Regular"});

            });



            modelBuilder.Entity<Person>(t =>
            {
                t.HasData(
                    new Person { IdPerson = 1, Name = "Gleb", Surname = "Ivanov", Email = "helli@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 2, Name = "Artur", Surname = "Morgan", Email = "test@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 3, Name = "Mikolaj", Surname = "Sluzalek", Email = "password@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 4, Name = "Jack", Surname = "Marston", Email = "marston@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 5, Name = "Anna", Surname = "Nowak", Email = "anna.nowak@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 6, Name = "John", Surname = "Doe", Email = "john.doe@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 7, Name = "Emma", Surname = "Smith", Email = "emma.smith@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 8, Name = "Liam", Surname = "Brown", Email = "liam.brown@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 9, Name = "Olivia", Surname = "Taylor", Email = "olivia.taylor@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 10, Name = "Noah", Surname = "Wilson", Email = "noah.wilson@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 11, Name = "Sophia", Surname = "Martinez", Email = "sophia.martinez@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 12, Name = "James", Surname = "Anderson", Email = "james.anderson@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 13, Name = "Isabella", Surname = "Thomas", Email = "isabella.thomas@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 14, Name = "William", Surname = "Lee", Email = "william.lee@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 15, Name = "Jan", Surname = "Kowalski", Email = "jan.kowalski@grandkrakow.pl", IdHotel = 2 });

            });

            modelBuilder.Entity<Guest>(t =>
            {
                t.HasData(
                    new Guest { IdPerson = 5, Passport = "PL111111", TelNumber = "500100100", IdGuestStatus = 1 },
                    new Guest { IdPerson = 6, Passport = "PL222222", TelNumber = "500200200", IdGuestStatus = 1 },
                    new Guest { IdPerson = 7, Passport = "PL333333", TelNumber = "500300300", IdGuestStatus = 1 },
                    new Guest { IdPerson = 8, Passport = "PL444444", TelNumber = "500400400", IdGuestStatus = 1 },
                    new Guest { IdPerson = 9, Passport = "PL555555", TelNumber = "500500500", IdGuestStatus = 1 },
                    new Guest { IdPerson = 10, Passport = "PL666666", TelNumber = "500600600", IdGuestStatus = 1 },
                    new Guest { IdPerson = 11, Passport = "PL777777", TelNumber = "500700700", IdGuestStatus = 1 },
                    new Guest { IdPerson = 12, Passport = "PL888888", TelNumber = "500800800", IdGuestStatus = 1 },
                    new Guest { IdPerson = 13, Passport = "PL999999", TelNumber = "500900900", IdGuestStatus = 1 },
                    new Guest { IdPerson = 14, Passport = "PL000000", TelNumber = "501000000", IdGuestStatus = 1 }
                );
            });


            modelBuilder.Entity<User>(t =>
            {
                t.HasData(
                    new User { IdPerson = 1, LoginName = "asd", Password = "AQAAAAEAACcQAAAAEJunn01F8InkwA3s5UmJeDNm+7oc9bBue4HftYt26KB5wusHnnCHpRLynWhQeBsldA==", IdUserType = 3 }, //pass = asd
                    new User { IdPerson = 2, LoginName = "qwe", Password = "AQAAAAEAACcQAAAAEICoXzMwe+5LQpNMHJ9xEsMYZiyVq4kyrIabFuBt2/BsE6XIdwdsn7Ix3pYtLVZArQ==", IdUserType = 1 }, //pass = qwe
                    new User { IdPerson = 3, LoginName = "zxc", Password = "AQAAAAEAACcQAAAAEHlv7ELDN76gkuMFVbat1+yHM+NR013pVL1SXY4+aYORlcApNaEoX33ZXJhZU6CAVQ==", IdUserType = 2 }, //pass = zxc
                    new User { IdPerson = 4, LoginName = "qaz", Password = "AQAAAAEAACcQAAAAEE69t49gGUi3mfUZdq9NYEgY84I2oG9t9KLonzFLjeXimp3t+ZdPufmjFp/JuJP9bg==", IdUserType = 3 }, //pass = qaz
                    new User { IdPerson = 15, LoginName = "jan", Password = "AQAAAAEAACcQAAAAEOo/kz63BA27/UpNrJX44IQqU2KaOlNvYXmzbSBbwIAQ/Ark0EqXJM/rH3/HS99Gnw==", IdUserType = 1 }); //pass = jan
            });




            modelBuilder.Entity<Room>(t =>
            {
                t.HasData(
                    new Room { IdRoom = 1, Number = "101", Capacity = 1, Price = 45f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 2, Number = "102", Capacity = 2, Price = 75f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 3, Number = "103", Capacity = 3, Price = 60f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 4, Number = "104", Capacity = 4, Price = 90f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 5, Number = "105", Capacity = 5, Price = 85f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 6, Number = "106", Capacity = 6, Price = 65f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 7, Number = "107", Capacity = 7, Price = 55f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 8, Number = "108", Capacity = 8, Price = 70f, IdRoomType = 1, IdRoomStatus = 1, IdUser = 2 },
                    new Room { IdRoom = 9, Number = "109", Capacity = 9, Price = 110f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 10, Number = "201", Capacity = 10, Price = 95f, IdRoomType = 1, IdRoomStatus = 2, IdUser = 2 },
                    new Room { IdRoom = 11, Number = "202", Capacity = 11, Price = 100f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 12, Number = "203", Capacity = 12, Price = 120f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 13, Number = "204", Capacity = 13, Price = 130f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 14, Number = "205", Capacity = 14, Price = 75f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 15, Number = "206", Capacity = 15, Price = 85f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 16, Number = "207", Capacity = 16, Price = 105f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 17, Number = "208", Capacity = 17, Price = 140f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 18, Number = "209", Capacity = 18, Price = 135f, IdRoomType = 1, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 19, Number = "301", Capacity = 19, Price = 100f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 20, Number = "302", Capacity = 20, Price = 110f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 2 },
                    new Room { IdRoom = 21, Number = "303", Capacity = 21, Price = 115f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 22, Number = "304", Capacity = 22, Price = 125f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 23, Number = "305", Capacity = 23, Price = 135f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 24, Number = "306", Capacity = 24, Price = 95f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 25, Number = "307", Capacity = 25, Price = 85f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 26, Number = "308", Capacity = 26, Price = 125f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 27, Number = "309", Capacity = 27, Price = 140f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 28, Number = "401", Capacity = 28, Price = 145f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 29, Number = "402", Capacity = 29, Price = 150f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 30, Number = "403", Capacity = 30, Price = 125f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 31, Number = "404", Capacity = 31, Price = 135f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 32, Number = "405", Capacity = 32, Price = 100f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 33, Number = "406", Capacity = 33, Price = 110f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 },
                    new Room { IdRoom = 34, Number = "407", Capacity = 34, Price = 95f, IdRoomType = 2, IdRoomStatus = 3, IdUser = 15 }
    );

            });
            modelBuilder.Entity<Deposit>(t =>
            {
                t.HasData(
                    new Deposit { IdDeposit = 1, Sum = 200, IdDepositType = 1 },
                    new Deposit { IdDeposit = 2, Sum = 300, IdDepositType = 1 },
                    new Deposit { IdDeposit = 3, Sum = 500, IdDepositType = 2 },
                    new Deposit { IdDeposit = 4, Sum = 1000, IdDepositType = 2 });

            });


            modelBuilder.Entity<Reservation>(t =>
            {
                t.HasData(
                    new Reservation { IdReservation = 1, Capacity = 1, Price = 150, In = new DateTime(2025, 06, 17), Out = new DateTime(2025, 06, 19), Confirmed = true, IdRoom = 2, IdUser = 1, IdGuest = 5, IdDeposit = 1 },
                    new Reservation { IdReservation = 2, Capacity = 2, Price = 180, In = new DateTime(2025, 06, 19), Out = new DateTime(2025, 06, 22), Confirmed = true, IdRoom = 3, IdUser = 2, IdGuest = 6, IdDeposit = 2 },
                    new Reservation { IdReservation = 3, Capacity = 3, Price = 340, In = new DateTime(2025, 06, 20), Out = new DateTime(2025, 06, 24), Confirmed = true, IdRoom = 5, IdUser = 3, IdGuest = 7, IdDeposit = 3 },
                    new Reservation { IdReservation = 4, Capacity = 4, Price = 325, In = new DateTime(2025, 06, 21), Out = new DateTime(2025, 06, 26), Confirmed = true, IdRoom = 6, IdUser = 3, IdGuest = 8, IdDeposit = 4 },
                    new Reservation { IdReservation = 5, Capacity = 1, Price = 330, In = new DateTime(2025, 06, 22), Out = new DateTime(2025, 06, 28), Confirmed = true, IdRoom = 7, IdUser = 3, IdGuest = 9 },
                    new Reservation { IdReservation = 6, Capacity = 2, Price = 140, In = new DateTime(2025, 06, 23), Out = new DateTime(2025, 06, 25), Confirmed = true, IdRoom = 8, IdUser = 3, IdGuest = 10, IdBill = 1 },
                    new Reservation { IdReservation = 7, Capacity = 3, Price = 330, In = new DateTime(2025, 06, 24), Out = new DateTime(2025, 06, 27), Confirmed = false, IdRoom = 9, IdUser = 3, IdGuest = 6 },
                    new Reservation { IdReservation = 8, Capacity = 4, Price = 380, In = new DateTime(2025, 06, 25), Out = new DateTime(2025, 06, 29), Confirmed = false, IdRoom = 10, IdUser = 3, IdGuest = 11 });
            });


            modelBuilder.Entity<ReservationService>(t =>
            {
                t.HasData(
                    new ReservationService { IdReservationService = 1, IdReservation = 2, IdService = 1, Date = new DateTime(2023, 11, 10) },
                    new ReservationService { IdReservationService = 2, IdReservation = 2, IdService = 2, Date = new DateTime(2023, 11, 12) },
                    new ReservationService { IdReservationService = 3, IdReservation = 2, IdService = 3, Date = new DateTime(2023, 11, 13) },
                    new ReservationService { IdReservationService = 4, IdReservation = 3, IdService = 3, Date = new DateTime(2023, 11, 13) });
            });

            modelBuilder.Entity<Bill>(t =>
            {
                t.HasData(
                    new Bill { IdBill = 1, Created = new DateTime(2025, 06, 25), Sum = 140f, InDate = new DateTime(2025, 06, 23), OutDate = new DateTime(2025, 06, 25), GuestName = "Noah", GuestSurname = "Wilson", RoomNumber = "108", BookedBy = "zxc"
                    }
                );
            });

        }
    }
}
