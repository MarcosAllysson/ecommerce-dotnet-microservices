using Microsoft.EntityFrameworkCore;
using SalesService.Models;

namespace SalesService.Data;

public class SalesContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public SalesContext(DbContextOptions<SalesContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                ProductId = 1,
                Quantity = 2,
                TotalPrice = 59.80M,
                OrderDate = DateTime.Now.AddDays(-1),
                Status = "Confirmed"
            }
        );
    }
}