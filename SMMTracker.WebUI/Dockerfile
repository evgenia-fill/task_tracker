FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SMMTracker.WebUI/SMMTracker.WebUI.csproj", "SMMTracker.WebUI/"]
COPY ["SMMTracker.Application/SMMTracker.Application.csproj", "SMMTracker.Application/"]
COPY ["SMMTracker.Domain/SMMTracker.Domain.csproj", "SMMTracker.Domain/"]
COPY ["SMMTracker.Infrastructure/SMMTracker.Infrastructure.csproj", "SMMTracker.Infrastructure/"]
RUN dotnet restore "./SMMTracker.WebUI/SMMTracker.WebUI.csproj"
COPY . .
WORKDIR "/src/SMMTracker.WebUI"
RUN dotnet build "./SMMTracker.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SMMTracker.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SMMTracker.WebUI.dll"]