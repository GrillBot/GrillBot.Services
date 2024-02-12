# GrillBot - PointsService

Service for manage user measures in [GrillBot](https://github.com/GrillBot).

## Requirements

- PostgreSQL server (minimal recommended version is 13).
- .NET 7.0 (with ASP.NET Core 7)
- RabbitMQ 2.

If you're running service on Linux distributions, you have to install these packages: `tzdata`, `libc6-dev`.

Only debian based distros are tested. Funcionality cannot be guaranteed for other distributions.

### Development requirements

- JetBrains Rider or another IDE supports .NET (for example Microsoft Visual Studio)
- [dotnet-ef](https://learn.microsoft.com/cs-cz/ef/core/cli/dotnet) utility (for code first migrations).

## Configuration

If you starting service in development environment (require environment variable `ASPNETCORE_ENVIRONMENT=Development`), you have to fill `appsettings.Development.json`.

If you starting service in production environment (container is recommended), you have to configure environment variables.

- `ConnectionStrings:Default` - Connection string to the database. First start requires created empty database with correctly set permissions.
- `RabbitMQ:Hostname` - Hostname to your RabbitMQ instance.
- `RabbitMQ:Username` - Username to your RabbitMQ instance.
- `RabbitMQ:Password` - Password to your RabbitMQ instance.

*In RabbitMQ, queues will be created automatically.*

## Containers

Latest docker image is published in GitHub packages.

## Licence

GrillBot and any other related services are licenced as All Rights Reserved. The source code is available for reading and contribution. Owner consent is required for use in a production environment or using some part of code in your project.
