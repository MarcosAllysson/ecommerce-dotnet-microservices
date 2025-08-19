# E-commerce Microservices with .NET

This repository contains a microservices-based e-commerce system built with .NET Core, implementing stock management, sales processing, and an API Gateway. The system uses RabbitMQ for asynchronous communication and JWT for authentication.

## Services

- **StockService**: Manages product catalog and stock levels (port: 5001).
- **SalesService**: Handles order creation and stock validation (port: 5002).
- **ApiGateway**: Routes requests to StockService and SalesService (port: 5000/5006).

## Technologies

- .NET Core
- Entity Framework Core (InMemory for simplicity)
- RabbitMQ (asynchronous notifications)
- RestSharp (HTTP requests)
- JWT Authentication
- Swagger (API documentation)

## Setup

1. Ensure Docker is installed and running RabbitMQ:
   ```bash
   docker run -d --hostname rabbitmq-alpine --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.8-management-alpine
   ```
2. Navigate to each service directory and run:
   ```bash
   dotnet run
   ```
3. Access the API Gateway Swagger at `http://localhost:5000/swagger` (or adjust port).
4. Generate a JWT token and use it in requests.

## Usage

- Create an order via ApiGateway:
  ```json
  POST /api/gateway/route
  {
    "service": "sales",
    "endpoint": "order",
    "data": { "productId": 1, "quantity": 1 }
  }
  ```
- Check stock:
  ```json
  POST /api/gateway/route
  {
    "service": "stock",
    "endpoint": "product",
    "data": { "id": 1 }
  }
  ```

## Notes

- Mock data is seeded in-memory for StockService and SalesService.
- RabbitMQ handles stock updates asynchronously.
