using hoteru_be.Entities;
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
        public virtual DbSet<ReservationService> ReservationService { get; set; }
        public virtual DbSet<GuestReservation> GuestReservations { get; set; }
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
                    new Service { IdService = 1, Title = "Breakfast", Sum = 355.5f, Description = "None"},
                    new Service { IdService = 2, Title = "Spa", Sum = 120.5f, Description = "None" },
                    new Service { IdService = 3, Title = "Assistent", Sum = 248.5f, Description = "None" });

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
                    new Address { IdAddress = 1, City = "Warsaw", Country = "Poland", Street = "Koszykowa 86", Postcode = "02-913" });

            });

            modelBuilder.Entity<Hotel>(t =>
            {
                t.HasData(
                    new Hotel { IdHotel = 1, Title = "Nobu", IdAddress = 1 });

            });

            modelBuilder.Entity<GuestStatus>(t =>
            {
                t.HasData(
                    new GuestStatus { IdGuestStatus = 1, Title = "VIP"});

            });



            modelBuilder.Entity<Person>(t =>
            {
                t.HasData(
                    new Person { IdPerson = 1, Name = "Gleb", Surname = "Ivanov", Email = "helli@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 2, Name = "Artur", Surname = "Morgan", Email = "test@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 3, Name = "Mikolaj", Surname = "Sluzalek", Email = "password@gmail.com", IdHotel = 1 },
                    new Person { IdPerson = 4, Name = "Jack", Surname = "Marston", Email = "marston@gmail.com", IdHotel = 1 });

            });

            modelBuilder.Entity<Guest>(t =>
            {
                t.HasData(
                    new Guest { IdPerson = 1, Passport = "FV124124", TelNumber = "123123123", IdGuestStatus = 1},
                    new Guest { IdPerson = 2, Passport = "BG121231", TelNumber = "567567567", IdGuestStatus = 1});

            });

            modelBuilder.Entity<User>(t =>
            {
                t.HasData(
                    new User { IdPerson = 3, LoginName = "asd", Password = "asd", IdUserType = 3 },
                    new User { IdPerson = 4, LoginName = "qwe", Password = "qwe", IdUserType = 3 });

            });




            modelBuilder.Entity<Room>(t =>
            {
                t.HasData(
                    new Room { IdRoom = 1, Number = "101", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 2, Number = "102", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 3, Number = "103", Capacity = 3, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 4, Number = "104", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 5, Number = "105", Capacity = 5, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 6, Number = "106", Capacity = 6, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 7, Number = "107", Capacity = 7, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 8, Number = "108", Capacity = 8, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 9, Number = "109", Capacity = 9, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 10, Number = "201", Capacity = 10, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 11, Number = "202", Capacity = 11, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 12, Number = "203", Capacity = 12, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 13, Number = "204", Capacity = 13, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 14, Number = "205", Capacity = 14, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 15, Number = "206", Capacity = 15, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 16, Number = "207", Capacity = 16, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 17, Number = "208", Capacity = 17, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 18, Number = "209", Capacity = 18, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 19, Number = "301", Capacity = 19, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 20, Number = "302", Capacity = 20, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 21, Number = "303", Capacity = 21, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 22, Number = "304", Capacity = 22, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 23, Number = "305", Capacity = 23, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 24, Number = "306", Capacity = 24, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 25, Number = "307", Capacity = 25, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 26, Number = "308", Capacity = 26, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 27, Number = "309", Capacity = 27, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 28, Number = "401", Capacity = 28, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 29, Number = "402", Capacity = 29, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 30, Number = "403", Capacity = 30, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 31, Number = "404", Capacity = 31, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 32, Number = "405", Capacity = 32, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 33, Number = "406", Capacity = 33, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 },
                    new Room { IdRoom = 34, Number = "407", Capacity = 34, Price = 3.5f, IdRoomType = 2, IdRoomStatus = 3 });

            });
            modelBuilder.Entity<Deposit>(t =>
            {
                t.HasData(
                    new Deposit { IdDeposit = 1, Sum = 200, IdDepositType = 1 },
                    new Deposit { IdDeposit = 2, Sum = 300, IdDepositType = 1 });

            });


            modelBuilder.Entity<Reservation>(t =>
            {
                t.HasData(
                    new Reservation { IdReservation = 1, Capacity = 2, Price = 1.0f, In = new DateTime(2023, 12, 6), Out = new DateTime(2023, 12, 12), Confirmed = true, IdRoom = 1, IdUser = 3, IdDeposit = 1 },
                    new Reservation { IdReservation = 2, Capacity = 3, Price = 2.0f, In = new DateTime(2023, 12, 6), Out = new DateTime(2023, 12, 18), Confirmed = true, IdRoom = 2, IdUser = 4, IdDeposit = 2, IdBill = 1 },
                    new Reservation { IdReservation = 3, Capacity = 2, Price = 1.0f, In = new DateTime(2023, 12, 6), Out = new DateTime(2023, 12, 12), Confirmed = false, IdRoom = 3, IdUser = 3});

            });

            modelBuilder.Entity<GuestReservation>(t =>
            {
                t.HasData(
                    new GuestReservation { IdGuestReservation = 1, IdReservation = 1, IdGuest = 1 },
                    new GuestReservation { IdGuestReservation = 2, IdReservation = 2, IdGuest = 2 },
                    new GuestReservation { IdGuestReservation = 3, IdReservation = 3, IdGuest = 1 });

            });

            modelBuilder.Entity<Bill>(t =>
            {
                t.HasData(
                    new Bill { IdBill = 1, Created = new DateTime(2023, 12, 06), Sum = 300 });
            });

            modelBuilder.Entity<ReservationService>(t =>
            {
                t.HasData(
                    new ReservationService { IdReservationService = 1, IdReservation = 2, IdService = 1, Date = new DateTime(2023, 11, 10) },
                    new ReservationService { IdReservationService = 2, IdReservation = 2, IdService = 2, Date = new DateTime(2023, 11, 12) },
                    new ReservationService { IdReservationService = 3, IdReservation = 2, IdService = 3, Date = new DateTime(2023, 11, 13) },
                    new ReservationService { IdReservationService = 4, IdReservation = 3, IdService = 3, Date = new DateTime(2023, 11, 13) });

            });

        }
    }
}
