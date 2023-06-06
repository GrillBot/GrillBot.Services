FROM mcr.microsoft.com/dotnet/sdk:7.0 AS Build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# App
RUN mkdir -p /src/ImageProcessingService
COPY "ImageProcessingService.csproj" /src/ImageProcessingService
RUN dotnet restore "src/ImageProcessingService/ImageProcessingService.csproj" -r linux-x64

COPY . /src/ImageProcessingService
RUN mkdir -p /publish
RUN dotnet publish /src/ImageProcessingService -c Release -o /publish --no-restore -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as FinalImage
LABEL org.opencontainers.image.source https://github.com/grillbot/grillbot.services

WORKDIR /app
EXPOSE 5213
ENV TZ=Europe/Prague

ENV ASPNETCORE_URLS 'http://+:5213'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE 'false'

RUN sed -i'.bak' 's/$/ contrib/' /etc/apt/sources.list
RUN apt update && apt install -y --no-install-recommends tzdata ttf-mscorefonts-installer fontconfig libc6-dev libgdiplus libx11-dev
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
RUN fc-cache -fv
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=Build /publish .
ENTRYPOINT [ "dotnet", "ImageProcessingService.dll" ]
