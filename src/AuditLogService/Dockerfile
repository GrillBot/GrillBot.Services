FROM mcr.microsoft.com/dotnet/sdk:7.0 AS Build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# App
RUN mkdir -p /src/AuditLogService
COPY "AuditLogService.csproj" /src/AuditLogService
RUN dotnet restore "src/AuditLogService/AuditLogService.csproj" -r linux-x64

COPY . /src/AuditLogService
RUN mkdir -p /publish
RUN dotnet publish /src/AuditLogService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as FinalImage
LABEL org.opencontainers.image.source https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5071
ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS 'http://+:5071'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE 'false'

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt update && apt install -y --no-install-recommends tzdata libc6-dev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=Build /publish .
ENTRYPOINT [ "dotnet", "AuditLogService.dll" ]
