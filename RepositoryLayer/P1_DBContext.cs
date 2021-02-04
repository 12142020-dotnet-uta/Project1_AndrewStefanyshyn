using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;

namespace RepositoryLayer
{
    public class P1_DbContext : DbContext
    {
        public DbSet<Customer> Customers{get; set;}
        public DbSet<Product> Products{get; set;}
        public DbSet<Order> Orders{get; set;}
        public DbSet<Location> Locations{get; set;}
        public DbSet<OrderLine> OrderLines{get; set;}
        public DbSet<LocationLine> LocationLines{get; set;}

        public P1_DbContext(){}
        public P1_DbContext(DbContextOptions<P1_DbContext> options) : base(options) {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=localhost;Database=Project1;Trusted_Connection=True;");
                optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}