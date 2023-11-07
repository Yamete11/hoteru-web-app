using hoteru_be.Entities;
using Microsoft.EntityFrameworkCore;

namespace hoteru_be.Context
{
    public class MyDbContext : DbContext
    {

        public virtual DbSet<Person> Persons { get; set; }

        public MyDbContext()
        {

        }

        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
