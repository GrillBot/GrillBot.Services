FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# Common lib
RUN mkdir -p /src/GrillBot.Services.Common
COPY "GrillBot.Services.Common/GrillBot.Services.Common.csproj" /src/GrillBot.Services.Common
RUN dotnet restore "src/GrillBot.Services.Common/GrillBot.Services.Common.csproj" -r linux-x64
COPY "GrillBot.Services.Common/" /src/GrillBot.Services.Common
RUN dotnet build "src/GrillBot.Services.Common/GrillBot.Services.Common.csproj" -r linux-x64

# App
RUN mkdir -p /src/EmoteService
COPY "EmoteService/EmoteService.csproj" /src/EmoteService
RUN dotnet restore "src/EmoteService/EmoteService.csproj" -r linux-x64

COPY "EmoteService/" /src/EmoteService
RUN mkdir -p /publish
RUN dotnet publish /src/EmoteService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5062

ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5062'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'

RUN apt update && apt install -y --no-install-recommends tzdata libc6-dev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "EmoteService.dll" ]
