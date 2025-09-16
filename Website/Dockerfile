# Etap build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiujemy csproj-y i przywracamy zależności
COPY Model/Model.csproj Model/
COPY Website/Website.csproj Website/
RUN dotnet restore Website/Website.csproj

# Kopiujemy resztę kodu
COPY Model/ Model/
COPY Website/ Website/

WORKDIR /src/Website
RUN dotnet publish -c Release -o /app

# Etap runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Website.dll"]
