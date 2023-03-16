# GrillBot - FileService

Microservice for manage files in Azure BlobStorage in [GrillBot](https://github.com/GrillBot).

## Requirements

- Registered Azure Storage account and created container named "development" and "production".
- .NET 7.0 (with ASP.NET Core 7)

If you're running bot on Linux distributions, you have to install these packages: `tzdata`, `libc6-dev`.

Only debian based distros are tested. Funcionality cannot be guaranteed for other distributions.

### Development requirements

- JetBrains Rider or another IDE supports .NET (for example Microsoft Visual Studio)

## Configuration

If you starting service in development environment (require environment variable `ASPNETCORE_ENVIRONMENT=Development`), you have to fill `appsettings.Development.json`.

If you starting service in production environment (container is recommended), you have to configure environment variables.

### Required environment variables

- `ConnectionStrings:StorageAccount` - Connection string to the Azure storage account.

## Containers

Latest docker image is published in GitHub packages.

## Licence

GrillBot and any other related microservices are licenced as All Rights Reserved. The source code is available for reading and contribution. Owner consent is required for use in a production environment or using some part of code in your project.
