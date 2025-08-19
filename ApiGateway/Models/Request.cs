namespace ApiGateway.Models;

public class Request
{
    public string Service { get; set; } = string.Empty; // "Stock" ou "Sales"
    public string Endpoint { get; set; } = string.Empty;
    public object Data { get; set; }
}