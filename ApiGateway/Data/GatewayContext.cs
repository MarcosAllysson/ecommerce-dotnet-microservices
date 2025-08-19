using Microsoft.EntityFrameworkCore;
using ApiGateway.Models;

namespace ApiGateway.Data;

public class GatewayContext : DbContext
{
    public DbSet<Request> Requests { get; set; }
    public DbSet<Response> Responses { get; set; }

    public GatewayContext(DbContextOptions<GatewayContext> options) : base(options) { }
}