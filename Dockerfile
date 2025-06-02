FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ARG DB_CONNECTION_STRING
ENV ConnectionStrings__DefaultConnection=$DB_CONNECTION_STRING

ENTRYPOINT ["dotnet", "PokemonReviewApp.dll"]