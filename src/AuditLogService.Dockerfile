FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# Common lib
RUN mkdir -p /src/GrillBot.Services.Common
COPY "GrillBot.Services.Common/GrillBot.Services.Common.csproj" /src/GrillBot.Services.Common
RUN dotnet restore "src/GrillBot.Services.Common/GrillBot.Services.Common.csproj" -r linux-x64
COPY "GrillBot.Services.Common/" /src/GrillBot.Services.Common

# App
RUN mkdir -p /src/AuditLogService
COPY "AuditLogService/AuditLogService.csproj" /src/AuditLogService
RUN dotnet restore "src/AuditLogService/AuditLogService.csproj" -r linux-x64

COPY "AuditLogService/" /src/AuditLogService
RUN mkdir -p /publish
RUN dotnet publish /src/AuditLogService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

# Tools
RUN dotnet tool install -g dotnet-dump && dotnet tool install -g dotnet-gcdump

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5071
ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5071'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'

RUN apt update && apt install -y --no-install-recommends tzdata libc6-dev procps
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /root/.dotnet /root/.dotnet
ENV PATH="/root/.dotnet/tools:${PATH}"

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "AuditLogService.dll" ]
