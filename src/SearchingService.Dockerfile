FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# Common lib
RUN mkdir -p /src/GrillBot.Services.Common
COPY "GrillBot.Services.Common/GrillBot.Services.Common.csproj" /src/GrillBot.Services.Common
RUN dotnet restore "src/GrillBot.Services.Common/GrillBot.Services.Common.csproj" -r linux-x64
COPY "GrillBot.Services.Common/" /src/GrillBot.Services.Common

# App
RUN mkdir -p /src/SearchingService
COPY "SearchingService/SearchingService.csproj" /src/SearchingService
RUN dotnet restore "src/SearchingService/SearchingService.csproj" -r linux-x64

COPY "SearchingService/" /src/SearchingService
RUN mkdir -p /publish
RUN dotnet publish /src/SearchingService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5164

ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5164'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'

RUN apk update && apk add tzdata
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "SearchingService.dll" ]
