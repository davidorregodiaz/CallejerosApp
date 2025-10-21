# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar la solución y archivos de proyecto
COPY *.sln .
COPY ./src/Adoption.API/*.csproj ./Adoption.API/
COPY ./src/Adoption.Domain/*.csproj ./Adoption.Domain/
COPY ./src/Adoption.Infrastructure/*.csproj ./Adoption.Infrastructure/
COPY ./src/Callejeros.DefaultServices/*.csproj ./Callejeros.DefaultServices/
COPY ./src/Shared/*.csproj ./Shared/

# Restaurar dependencias
RUN dotnet restore ./Adoption.API/Adoption.API.csproj

# Copiar todo el código
COPY . .

# Publicar la aplicación
WORKDIR /src/Adoption.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Adoption.API.dll"]
