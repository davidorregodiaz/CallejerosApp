# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos del proyecto y restaurar dependencias
COPY *.sln .
COPY ./CallejerosApp.API/*.csproj ./CallejerosApp.API/
RUN dotnet restore ./CallejerosApp.API/CallejerosApp.API.csproj

# Copiar todo y publicar
COPY . .
RUN dotnet publish ./CallejerosApp.API/CallejerosApp.API.csproj -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CallejerosApp.API.dll"]
