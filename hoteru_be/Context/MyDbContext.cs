using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<Room>(t =>
            {
                t.HasData(
                    new Room { IdRoom = 1, Number = "101", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 2, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 3, Number = "203", Capacity = 3, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 4, Number = "305", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 5, Number = "101", Capacity = 5, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 6, Number = "201", Capacity = 6, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 7, Number = "203", Capacity = 7, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 8, Number = "305", Capacity = 8, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 9, Number = "101", Capacity = 9, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 10, Number = "201", Capacity = 10, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 11, Number = "203", Capacity = 11, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 12, Number = "305", Capacity = 12, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 13, Number = "101", Capacity = 13, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 14, Number = "201", Capacity = 14, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 15, Number = "203", Capacity = 15, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 16, Number = "305", Capacity = 16, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 17, Number = "101", Capacity = 17, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 18, Number = "201", Capacity = 18, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 19, Number = "203", Capacity = 19, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 20, Number = "305", Capacity = 20, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 21, Number = "101", Capacity = 21, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 22, Number = "201", Capacity = 22, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 23, Number = "203", Capacity = 23, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 24, Number = "305", Capacity = 24, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 25, Number = "203", Capacity = 25, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 26, Number = "305", Capacity = 26, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 27, Number = "101", Capacity = 27, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 28, Number = "201", Capacity = 28, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 29, Number = "203", Capacity = 29, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 30, Number = "305", Capacity = 30, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 31, Number = "101", Capacity = 31, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 32, Number = "201", Capacity = 32, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 33, Number = "203", Capacity = 33, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 34, Number = "305", Capacity = 34, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 });

            });
        }
    }
}
