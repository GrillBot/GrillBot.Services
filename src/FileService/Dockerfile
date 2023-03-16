FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS Build

# Run external NuGet source
ARG github_actions_token
RUN dotnet nuget add source https://nuget.pkg.github.com/GrillBot/index.json -n GrillBot -u Misha12 -p "${github_actions_token}" --store-password-in-clear-text

# App
RUN mkdir -p /src/FileService
COPY "FileService/FileService.csproj" /src/FileService
RUN dotnet restore "src/FileService/FileService.csproj" -r linux-musl-x64

COPY "FileService/" /src/FileService
RUN mkdir -p /publish
RUN dotnet publish /src/FileService -c Release -o /publish --no-restore -r linux-musl-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as FinalImage
LABEL org.opencontainers.image.source https://github.com/grillbot/fileService

WORKDIR /app
EXPOSE 5273
ENV TZ=Europe/Prague
ENV ASPNETCORE_URLS 'http://+:5273'
ENV DOTNET_PRINT_TELEMETRY_MESSAGE 'false'

RUN apk update && apk add tzdata libc-dev
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=Build /publish .
ENTRYPOINT [ "dotnet", "FileService.dll" ]
