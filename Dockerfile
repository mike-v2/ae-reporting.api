# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["ae-reporting.api.csproj", "./"]
RUN dotnet restore "./ae-reporting.api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "ae-reporting.api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ae-reporting.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "ae-reporting.api.dll"]
