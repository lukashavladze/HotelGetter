using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HotelGetter
{
    public class hotelDbContext : DbContext
    {
        public hotelDbContext(DbContextOptions options) : base(options)
        {

        }
  
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
        }
    }
}
