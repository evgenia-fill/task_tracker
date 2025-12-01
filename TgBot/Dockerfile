FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["TgBot/TgBot.csproj", "TgBot/"]
COPY ["BlazorApp1.sln", "./"]

RUN dotnet restore BlazorApp1.sln

COPY . .

RUN dotnet publish TgBot/TgBot.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /src/TgBot/bin/Release/net8.0/publish/ .
ENTRYPOINT ["dotnet", "TgBot.dll"]