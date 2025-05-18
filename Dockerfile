# Шаг 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# Шаг 2: Миграции (новый этап)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrations
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ef", "database", "update"]

# Шаг 3: Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "PokemonReviewApp.dll"]