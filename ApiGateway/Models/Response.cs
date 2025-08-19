namespace ApiGateway.Models;

public class Response
{
    public bool Success { get; set; }
    public object Data { get; set; }
    public string ErrorMessage { get; set; }
}