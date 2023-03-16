# GrillBot - RubbergodService

Microservice for connectivity between [GrillBot](https://github.com/GrillBot) and [Rubbergod](https://github.com/Toaster192/rubbergod).

## Requirements

- PostgreSQL server (minimal recomended version is 13).
- .NET 7.0 (with ASP.NET Core 7)

If you're running bot on Linux distributions, you have to install these packages: `tzdata`, `libc6-dev`.

Only debian based distros are tested. Funcionality cannot be guaranteed for other distributions.

### Development requirements

- JetBrains Rider or another IDE supports .NET (for example Microsoft Visual Studio)
- [dotnet-ef](https://learn.microsoft.com/cs-cz/ef/core/cli/dotnet) utility (for code first migrations).

## Configuration

If you starting service in development environment (require environment variable `ASPNETCORE_ENVIRONMENT=Development`), you have to fill `appsettings.Development.json`.

If you starting service in production environment (container is recommended), you have to configure environment variables.

### Required environment variables

- `ConnectionStrings:Default` - Connection string to the database. First start requires created empty database with correctly set permissions.
- `ConnectionStrings:DiscordBot` - Discord bot authentication token.
- `DirectApi:Rubbergod:ChannelId` - Channel ID for communication between rubbergod and this service.
- `DirectApi:Rubbergod:Id` - ID of rubbergod. Set only if you're using selfhosted instance of rubbergod.

## Containers

Latest docker image is published in GitHub packages.

## Licence

GrillBot and any other related microservices are licenced as All Rights Reserved. The source code is available for reading and contribution. Owner consent is required for use in a production environment or using some part of code in your project.
