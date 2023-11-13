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
                    new Room { IdRoom = 4, Number = "305", Capacity = 1, Price = 3.5f, IdRoomType = 1, IdRoomStatus = 1 });

            });
        }
    }
}
