using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tus DbSets:
        public DbSet<User> Users { get; set; }
        // public DbSet<Product> Products { get; set; }
    }
}
