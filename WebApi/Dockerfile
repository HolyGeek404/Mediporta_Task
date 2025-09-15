#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY WebApi/ WebApi/
COPY Model/ Model/

RUN dotnet restore "WebApi/WebApi.csproj"
RUN dotnet publish "WebApi/WebApi.csproj" -c Release -o /app/publish --no-restore

#Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
