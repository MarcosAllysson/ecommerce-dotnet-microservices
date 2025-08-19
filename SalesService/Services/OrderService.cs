using RestSharp;
using SalesService.Data;
using SalesService.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SalesService.Services;

public class OrderService
{
    private readonly SalesContext _context;
    private readonly string _stockServiceUrl;
    private readonly IConfiguration _configuration;

    public OrderService(SalesContext context, IConfiguration configuration)
    {
        _context = context;
        _stockServiceUrl = configuration["ServiceUrls:StockService"];
        _configuration = configuration;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        if (order.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        // Validar estoque via REST
        var client = new RestClient(_stockServiceUrl);
        var request = new RestRequest($"api/product/{order.ProductId}", Method.Get);
        var response = await client.ExecuteAsync<SalesService.Models.Product>(request); // Usar o modelo local

        if (!response.IsSuccessful || response.Data == null || response.Data.StockQuantity < order.Quantity)
            throw new InvalidOperationException("Insufficient stock or product not found.");

        order.TotalPrice = order.Quantity * response.Data.Price;
        order.OrderDate = DateTime.Now;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Enviar notificação via RabbitMQ
        await NotifyStockUpdateAsync(order.ProductId, -order.Quantity);

        order.Status = "Confirmed";
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    private async Task NotifyStockUpdateAsync(int productId, int quantity)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "stock-updates",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var message = new
        {
            ProductId = productId,
            Quantity = quantity
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "stock-updates",
            body: body,
            mandatory: false
        );
    }
}