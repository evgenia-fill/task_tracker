FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SMMTracker.TgBot/SMMTracker.TgBot.csproj", "SMMTracker.TgBot/"]
COPY ["SMMTracker.Application/SMMTracker.Application.csproj", "SMMTracker.Application/"]
COPY ["SMMTracker.Domain/SMMTracker.Domain.csproj", "SMMTracker.Domain/"]
COPY ["SMMTracker.Infrastructure/SMMTracker.Infrastructure.csproj", "SMMTracker.Infrastructure/"]
RUN dotnet restore "./SMMTracker.TgBot/SMMTracker.TgBot.csproj"
COPY . .
WORKDIR "/src/SMMTracker.TgBot"
RUN dotnet build "./SMMTracker.TgBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SMMTracker.TgBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SMMTracker.TgBot.dll"]