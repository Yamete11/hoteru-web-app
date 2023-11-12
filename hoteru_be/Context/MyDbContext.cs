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
        }
    }
}
