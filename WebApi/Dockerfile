# Etap build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiujemy csproj-y i przywracamy zależności
COPY Model/Model.csproj Model/
COPY WebApi/WebApi.csproj WebApi/
RUN dotnet restore WebApi/WebApi.csproj

# Kopiujemy resztę kodu
COPY Model/ Model/
COPY WebApi/ WebApi/

WORKDIR /src/WebApi
RUN dotnet publish -c Release -o /app

# Etap runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebApi.dll"]
