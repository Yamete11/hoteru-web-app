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
                    new RoomType { IdRoomType = 1, Title = "Regular" });

            });

            modelBuilder.Entity<Room>(t =>
            {
                t.HasData(
                    new Room { IdRoom = 1, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 2, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 3, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 4, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 5, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 6, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 7, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 8, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 9, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 10, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 11, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 12, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 13, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 14, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 15, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 16, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 17, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 18, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 19, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 20, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 21, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 22, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 23, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 24, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 25, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 26, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 27, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 28, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 29, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 30, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 31, Number = "101", Capacity = 4, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 },
                    new Room { IdRoom = 32, Number = "201", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 2 },
                    new Room { IdRoom = 33, Number = "203", Capacity = 2, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 3 },
                    new Room { IdRoom = 34, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 });

            });
        }
    }
}
