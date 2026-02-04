
# -----------------------------
# Build stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY src/Adoption.API/Adoption.API.csproj src/Adoption.API/
COPY src/Adoption.Domain/Adoption.Domain.csproj src/Adoption.Domain/
COPY src/Adoption.Infrastructure/Adoption.Infrastructure.csproj src/Adoption.Infrastructure/
COPY src/Shared/Shared.csproj src/Shared/

RUN dotnet restore src/Adoption.API/Adoption.API.csproj

# Copiamos el resto del código
COPY . .

# Publicamos
RUN dotnet publish src/Adoption.API/Adoption.API.csproj \
    -c Release \
    -o /out \
    /p:UseAppHost=false

# -----------------------------
# Runtime stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Adoption.API.dll"]

