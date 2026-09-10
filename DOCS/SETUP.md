# Guía de Instalación y Configuración

## Requisitos Previos

### Software Necesario

| Software | Versión | Descarga |
|----------|---------|----------|
| .NET SDK | 6.0 | https://dotnet.microsoft.com/download/dotnet/6.0 |
| SQL Server | 2019+ | https://www.microsoft.com/en-us/sql-server |
| SQL Server Management Studio | 20+ | https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms |
| Visual Studio 2022 | 17.0+ | https://visualstudio.microsoft.com/downloads/ |
| Git | Latest | https://git-scm.com/downloads |

### Verificar Instalación

```bash
# Verificar .NET SDK
dotnet --version

# Verificar SQL Server
sqlcmd -?

# Verificar Git
git --version
```

## Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/usuario/CreditCardStatement.git
cd CreditCardStatement
```

### 2. Configurar Base de Datos

#### Opción A: Usar SQL Server LocalDB (Recomendado para desarrollo)

LocalDB se instala automáticamente con Visual Studio o con:

```bash
dotnet install sqlserver
```

#### Opción B: Usar SQL Server completo

1. Abrir SQL Server Management Studio (SSMS)
2. Conectar a la instancia de SQL Server
3. Abrir los scripts SQL en orden:
   - `database/scripts/01_CreateTables.sql`
   - `database/scripts/02_CreateStoredProcedures.sql`
   - `database/scripts/03_SeedData.sql`
4. Ejecutar cada script

#### Opción C: Usar sqlcmd desde línea de comandos

```bash
# Crear tablas
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\01_CreateTables.sql

# Crear procedimientos almacenados
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\02_CreateStoredProcedures.sql

# Insertar datos de prueba
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\03_SeedData.sql
```

### 3. Configurar Connection Strings

#### Archivo: `src/CreditCardStatement.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CreditCardStatementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyAtLeast32CharactersLong!",
    "Issuer": "CreditCardStatementAPI",
    "Audience": "CreditCardStatementApp"
  }
}
```

#### Archivo: `src/CreditCardStatement.MVC/appsettings.json`

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7001"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CreditCardStatementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 4. Restaurar Paquetes NuGet

```bash
dotnet restore
```

### 5. Compilar el Proyecto

```bash
dotnet build
```

### 6. Ejecutar la Aplicación

#### Ejecutar API (Terminal 1)

```bash
cd src/CreditCardStatement.API
dotnet run
```

La API estará disponible en:
- **API**: https://localhost:7001
- **Swagger**: https://localhost:7001/swagger

#### Ejecutar MVC (Terminal 2)

```bash
cd src/CreditCardStatement.MVC
dotnet run
```

La aplicación MVC estará disponible en:
- **MVC**: https://localhost:5001 (o el puerto configurado)

## Verificación

### 1. Verificar API

Abrir navegador en: https://localhost:7001/swagger

Debería mostrar la documentación de Swagger con todos los endpoints.

### 2. Verificar Health Check

```bash
curl https://localhost:7001/health
```

Respuesta esperada: `Healthy`

### 3. Probar Login

```bash
curl -X POST https://localhost:7001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"henry.aq@mail.com","password":"contraseña123"}'
```

### 4. Acceder a MVC

Abrir navegador en: https://localhost:5001

Iniciar sesión con:
- **Email**: henry.aq@mail.com
- **Contraseña**: contraseña123

## Solución de Problemas

### Error: No se puede conectar a SQL Server

**Solución:**
1. Verificar que SQL Server esté ejecutándose
2. Verificar el connection string en `appsettings.json`
3. Si usa LocalDB, ejecutar: `sqllocaldb start mssqllocaldb`

### Error: La base de datos no existe

**Solución:**
1. Ejecutar el script `01_CreateTables.sql`
2. Verificar que el nombre de la base de datos coincida con el connection string

### Error: Procedimiento almacenado no encontrado

**Solución:**
1. Ejecutar el script `02_CreateStoredProcedures.sql`
2. Verificar que los SPs se crearon correctamente en SSMS

### Error: Puerto ya en uso

**Solución:**
1. Cambiar el puerto en `Properties/launchSettings.json`
2. O usar un puerto diferente: `dotnet run --urls "https://localhost:5002"`

### Error: Paquete NuGet no encontrado

**Solución:**
1. Ejecutar: `dotnet restore`
2. Si persiste, limpiar: `dotnet clean && dotnet restore && dotnet build`

## Configuración de HTTPS

### Certificado de Desarrollo

Para desarrollo, se usa un certificado auto-firmado. Si hay problemas:

```bash
# Instalar certificado de desarrollo
dotnet dev-certs https --trust

# Verificar certificado
dotnet dev-certs https --check
```

### Puertos por Defecto

| Proyecto | Puerto |
|----------|--------|
| API | 7001 (HTTPS) |
| MVC | 5001 (HTTPS) |

## Variables de Entorno

### Desarrollo

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Producción

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:servidor.database.windows.net,1433;Database=CreditCardStatementDb;User ID=usuario;Password=contraseña;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Jwt": {
    "Key": "ClaveSeguraDeAlMenos32Caracteres!",
    "Issuer": "CreditCardStatementAPI",
    "Audience": "CreditCardStatementApp"
  }
}
```

## Comandos Útiles

```bash
# Limpiar proyecto
dotnet clean

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar en modo desarrollo
dotnet run --project src/CreditCardStatement.API

# Ejecutar conUrls específicas
dotnet run --project src/CreditCardStatement.API --urls "https://localhost:7001"

# Publicar para producción
dotnet publish -c Release -o ./publish
```
