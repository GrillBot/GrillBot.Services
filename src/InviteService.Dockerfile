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
RUN mkdir -p /src/InviteService
COPY "InviteService/InviteService.csproj" /src/InviteService
RUN dotnet restore "src/InviteService/InviteService.csproj" -r linux-x64

COPY "InviteService/" /src/InviteService
RUN mkdir -p /publish
RUN dotnet publish /src/InviteService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5196

ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5196'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'

RUN apt update && apt install -y --no-install-recommends tzdata libc6-dev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "InviteService.dll" ]
