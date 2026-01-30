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
RUN mkdir -p /src/ImageProcessingService
COPY "ImageProcessingService/ImageProcessingService.csproj" /src/ImageProcessingService
RUN dotnet restore "src/ImageProcessingService/ImageProcessingService.csproj" -r linux-x64

COPY "ImageProcessingService/" /src/ImageProcessingService
RUN mkdir -p /publish
RUN dotnet publish /src/ImageProcessingService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final_image
LABEL org.opencontainers.image.source=https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5213
ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS='http://+:5213'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE='false'
ENV FONTCONFIG_PATH=/etc/fonts
ENV FONTCONFIG_FILE=/etc/fonts/fonts.conf

RUN apt update && apt install -y --no-install-recommends \
    tzdata \
    fontconfig \
    fontconfig-config \
    fonts-dejavu-core \
    fonts-dejavu-extra \
    fonts-liberation \
    fonts-open-sans \
    libgdiplus \
    libx11-6 \
    libc6-dev \
 && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
RUN mkdir -p /etc/fonts/conf.d && fc-cache -fv
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build /publish .
ENTRYPOINT [ "dotnet", "ImageProcessingService.dll" ]
