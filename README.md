# SE121 ECommerce Backend

ASP.NET Core backend for an e-commerce platform with JWT authentication, SQL Server, Docker, and structured layering for controllers, services, repositories, DTOs, entities, and mappers.

## Requirements

- .NET 10 SDK
- SQL Server
- Docker and Docker Compose if you want the containerized stack

## Run Locally

1. Update the connection string in [ECommerceApp/appsettings.json](ECommerceApp/appsettings.json).
2. Restore and build the app from the [ECommerceApp](ECommerceApp) project folder.
3. Run the API with `dotnet run`.

## Docker

The repository includes [docker-compose.yml](docker-compose.yml) and a project [Dockerfile](ECommerceApp/Dockerfile) for container-based execution.

## Database

The application uses Entity Framework Core with seed data in `ApplicationDbContext`. Apply migrations before first run and then use `dotnet ef database update`.

## Configuration

Key settings are defined in [ECommerceApp/appsettings.json](ECommerceApp/appsettings.json) and [ECommerceApp/appsettings.Development.json](ECommerceApp/appsettings.Development.json):

- `ConnectionStrings:EFCoreDBConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `SERILOG_*` settings for logging

## API Surface

Current controllers include customers, addresses, carts, categories, and products. The domain also includes entities and mapper registrations for order, payment, cancellation, refund, and feedback features that are in progress.