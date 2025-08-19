using Microsoft.EntityFrameworkCore;
using StockService.Models;

namespace StockService.Data;

public class StockContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public StockContext(DbContextOptions<StockContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Camisa", Description = "Camisa azul", Price = 29.90M, StockQuantity = 100 },
            new Product { Id = 2, Name = "Calça", Description = "Calça jeans", Price = 59.90M, StockQuantity = 50 }
        );
    }
}