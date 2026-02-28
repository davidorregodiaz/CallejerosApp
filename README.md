# CallejerosAPI

CallejerosAPI es una API desarrollada en .NET 9 diseñada para **gestionar adopciones de animales callejeros** y la
interacción entre usuarios y organizaciones de rescate.

Entre sus funcionalidades principales:

- Gestión de **usuarios** y autenticación mediante **JWT**.
- Visualización de animales disponibles para adopción.
- Solicitudes de adopción por parte de usuarios, con un flujo de **evaluación de postulantes** por los administradores
  mediante citas y revisiones.
- Gestión de **posts** sobre animales enfermos o en situaciones especiales, así como historias de adopciones compartidas
  por los usuarios.
- Seguimiento y **traceo de adopciones**, permitiendo a la organización realizar visitas de seguimiento a los animales
  después de ser adoptados.
- Envío de notificaciones y correos electrónicos a usuarios y administradores.

**Reglas importantes**:

- Está **terminantemente prohibido la venta de animales**; la API solo permite la gestión de adopciones.
- Toda interacción y posteo está orientada a **fomentar la adopción responsable** y el cuidado de los animales.

Tecnologías utilizadas: **PostgreSQL**, **Minio**, **SMTP** para emails, y .NET 9 para toda la lógica de
negocio y servicios.

---

## Requisitos

Antes de levantar la API, asegúrate de tener instalado:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [PostgreSQL 18.1](https://www.postgresql.org/download/)
- [Minio](https://min.io/download)
- Cliente SMTP válido (Mailtrap, Gmail, etc.)
- Docker y Docker Compose (opcional, recomendado para levantar Postgres y Minio)

---

## Instalación

1. Clonar el repositorio:

```
git clone https://github.com/davidorregodiaz/CallejerosAPI.git
cd CallejerosAPI
```

2. Copiar el archivo de configuración de ejemplo:

```
cp appsettings.example.json appsettings.Development.json
```

3. Editar appsettings.Development.json con tus credenciales locales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=1234"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Minio": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "mybucket",
    "WithSSL": false
  },
  "Email": {
    "SmtpHost": "smtp.mailtrap.io",
    "SmtpPort": 587,
    "Username": "username",
    "Password": "password",
    "UseSsl": true
  },
  "Jwt": {
    "Url": "https://localhost:5001",
    "Audience": "MyDotNetAPI",
    "ExpireMinutes": 20,
    "SigningKey": "your_signing_key"
  }
}
```

## Comandos de Consola

Sigue este orden para preparar el entorno y ejecutar la aplicación:

1. Restaurar dependencias
   Descarga todos los paquetes NuGet necesarios para el proyecto:

``` 

dotnet restore

```

2. Actualizar la Base de Datos (EF Core)
   Si ya tienes migraciones en el proyecto, aplica los cambios a tu base de datos PostgreSQL:

#### Nota: Requiere la herramienta dotnet-ef instalada

```

dotnet ef database update

```

3. Ejecutar el Proyecto
   Inicia la API en modo desarrollo con recarga en caliente (Hot Reload):

```

dotnet run

```

## Infraestructura con Docker (Opcional)

Si tienes Docker instalado, puedes levantar rápidamente los servicios externos (Postgres y Minio) utilizando el archivo
docker-compose.yml incluido:

```

docker-compose up -d

```