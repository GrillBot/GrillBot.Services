FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# Common lib
RUN mkdir -p /src/GrillBot.Services.Common
COPY "GrillBot.Services.Common/GrillBot.Services.Common.csproj" /src/GrillBot.Services.Common
RUN dotnet restore "src/GrillBot.Services.Common/GrillBot.Services.Common.csproj" -r linux-x64
COPY "GrillBot.Services.Common/" /src/GrillBot.Services.Common

# App
RUN mkdir -p /src/UnverifyService
COPY "UnverifyService/UnverifyService.csproj" /src/UnverifyService
RUN dotnet restore "src/UnverifyService/UnverifyService.csproj" -r linux-x64

COPY "UnverifyService/" /src/UnverifyService
RUN mkdir -p /publish
RUN dotnet publish /src/UnverifyService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5037

ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5037'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'

RUN apt update && apt install -y --no-install-recommends tzdata libc6-dev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "UnverifyService.dll" ]
