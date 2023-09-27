using E_Commerce_API.Helpers;
using E_Commerce_API.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_API.Data
{
    public class ECommerceContext : DbContext
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Loggs> Loggs { get; set; }
        public DbSet<BankModel> BankModels { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseModel> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Email = "Admin123@gmail.com",
                    UserName = "Admin123",
                    Password = HashSettings.HashPassword("Admin123"),
                    Role = "Admin",
                    Age = 28,
                    ShippingAddress = "Unknown"
                });
        }
    }
}