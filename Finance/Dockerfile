# Base image (ASP.NET runtime)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build aþamasý
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "YourProject.csproj"
RUN dotnet build "YourProject.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "YourProject.csproj" -c Release -o /app/publish

# Final aþama
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourProject.dll"]
