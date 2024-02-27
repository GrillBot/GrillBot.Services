# GrillBot - AuditLogService

Service to manage all logging functionality in [GrillBot](https://github.com/GrillBot).

## Requirements

- [PostgreSQL](https://www.postgresql.org/) server (minimal recommended version is 13) [Docker image](https://hub.docker.com/_/postgres)
- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) (with ASP.NET Core 7)
- [RabbitMQ](https://rabbitmq.com/) (minimal recommended version is 2) [Docker image](https://hub.docker.com/_/rabbitmq)

If you're running service on Linux distributions, you have to install these packages: `tzdata`, `libc6-dev`.

Only debian based distros are tested. Funcionality cannot be guaranteed for other distributions.

### Development requirements

- JetBrains Rider or another IDE supports .NET (for example Microsoft Visual Studio)
- [dotnet-ef](https://learn.microsoft.com/cs-cz/ef/core/cli/dotnet) utility (for code first migrations).
- Generate [personal access token (Classic)](https://docs.github.com/en/enterprise-server@3.4/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token) with `read:packages` permission.
- Add new NuGet source with URL `https://nuget.pkg.github.com/GrillBot/index.json`.
  - You can do it from your IDE or via CLI `dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u {Username} -p {PersonalAccessToken}`
  - On Linux systems add to previous command parameter `--store-password-in-clear-text`.

## Configuration

If you starting service in development environment (require environment variable `ASPNETCORE_ENVIRONMENT=Development`), you have to fill `appsettings.Development.json`.

If you starting service in production environment (container is recommended), you have to configure environment variables.

- `ConnectionStrings:Default` - Connection string to the database. First start requires created empty database with correctly set permissions.
- `ConnectionStrings:BotToken` - Discord bot authentication token.
- `RabbitMQ:Hostname`, `RabbitMQ:Username`, `RabbitMQ:Password` - Credentials for your RabbitMQ instance.

## Containers

Latest docker image is published in GitHub packages.

## Licence

GrillBot and any other related services are licenced as All Rights Reserved. The source code is available for reading and contribution. Owner consent is required for use in a production environment or using some part of code in your project.
