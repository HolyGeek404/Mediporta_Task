#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Website/ Website/
COPY Model/ Model/

RUN dotnet restore "Website/Website.csproj"
RUN dotnet publish "Website/Website.csproj" -c Release -o /app/publish --no-restore

#Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Website.dll"]
