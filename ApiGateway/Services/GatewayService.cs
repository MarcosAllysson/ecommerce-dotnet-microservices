using RestSharp;
using ApiGateway.Models;
using System.Text.Json;

namespace ApiGateway.Services;

public class GatewayService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GatewayService> _logger;

    public GatewayService(IConfiguration configuration, ILogger<GatewayService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // Roteia a requisição para o serviço apropriado
    public async Task<Response> RouteRequestAsync(Request request, string token)
    {
        var serviceUrl = request.Service.ToLower() switch
        {
            "stock" => _configuration["ServiceUrls:StockService"],
            "sales" => _configuration["ServiceUrls:SalesService"],
            _ => throw new ArgumentException("Invalid service specified.")
        };

        if (string.IsNullOrEmpty(serviceUrl))
            return new Response { Success = false, ErrorMessage = "Service URL not configured." };

        var client = new RestClient(serviceUrl);
        var fullEndpoint = $"{serviceUrl}/api/{request.Endpoint}";
        var restRequest = new RestRequest(fullEndpoint, Method.Post); // Ajuste o método conforme necessário

        if (token != null)
            restRequest.AddHeader("Authorization", $"Bearer {token}");

        if (request.Data != null)
            restRequest.AddJsonBody(request.Data);

        try
        {
            var response = await client.ExecuteAsync(restRequest);

            if (response.IsSuccessful)
                return new Response
                {
                    Success = true,
                    Data = JsonSerializer.Deserialize<object>(response.Content)
                };

            return new Response
            {
                Success = false,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error routing request to {Service}", request.Service);

            return new Response
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}