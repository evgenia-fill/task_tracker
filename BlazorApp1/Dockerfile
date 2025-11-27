FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["TgBot/TgBot.csproj", "TgBot/"]
COPY ["BlazorApp1.sln", "./"]

RUN dotnet restore BlazorApp1.sln

COPY . .

WORKDIR /src/BlazorApp1
RUN dotnet publish "BlazorApp1.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /src/BlazorApp1/app/publish .
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]