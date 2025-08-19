using Microsoft.EntityFrameworkCore;
using StockService.Data;
using StockService.Models;

namespace StockService.Service;

public class ProductService
{
    private readonly StockContext _context;

    public ProductService(StockContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        if (string.IsNullOrEmpty(product.Name))
            throw new ArgumentException("Product's name is mandatory!");

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task UpdateStockAsync(int ProductId, int quantity)
    {
        var Product = await _context.Products.FindAsync(ProductId);

        if (Product == null)
            throw new KeyNotFoundException("Product not found.");

        // Pode ser negativo para redução
        Product.StockQuantity += quantity;

        if (Product.StockQuantity < 0)
            throw new InvalidOperationException("Insuficient stock.");

        await _context.SaveChangesAsync();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }
}