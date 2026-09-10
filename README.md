# Credit Card Statement Application

![.NET 6](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoft-sql-server)
![JWT](https://img.shields.io/badge/Auth-JWT-black?logo=json-web-tokens)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-success)

Aplicacion full-stack desarrollada como prueba tecnica para la posicion de Desarrollador de Servicios Web. Muestra estados de cuenta de tarjetas de credito con seguimiento de compras, procesamiento de pagos y calculos financieros automatizados.

## Descripcion

Esta aplicacion permite a los usuarios consultar el estado de cuenta de sus tarjetas de credito, registrar nuevas compras, realizar pagos, visualizar el historial completo de transacciones y exportar el estado de cuenta a PDF. Implementa calculos financieros configurables como interes bonificable, cuota minima a pagar y pago de contado con intereses.

### Funcionalidades Principales

- Consulta de estado de cuenta con resumen financiero
- Registro de compras con validacion de datos
- Procesamiento de pagos
- Historial completo de transacciones ordenado por fecha
- Exportacion de estado de cuenta a PDF
- Autenticacion JWT segura
- Health checks de la aplicacion y base de datos

## Tecnologias

| Capa | Tecnologia |
|------|------------|
| Backend | ASP.NET Web API (.NET 6) |
| Frontend | ASP.NET MVC + Razor + jQuery |
| Base de Datos | SQL Server con Stored Procedures |
| ORM | Entity Framework Core 6 |
| Patrones | Clean Architecture, CQRS (MediatR), Repository, Unit of Work |
| Validacion | FluentValidation |
| Mapeo | AutoMapper |
| Documentacion API | Swagger / OpenAPI |
| Autenticacion | JWT Bearer |
| Hashing de Passwords | BCrypt |

## Arquitectura

El proyecto sigue el patron **Clean Architecture** con **CQRS** (Command Query Responsibility Segregation) para separar claramente las operaciones de lectura y escritura.

### Diagrama de la Aplicacion

```
  +-------------------------------------------------------------+
  |                    CLIENTE (Navegador)                       |
  |           HTML5 + CSS3 + JavaScript/jQuery                 |
  +---------------------------+---------------------------------+
                              | HTTP/HTTPS
                              v
  +-------------------------------------------------------------+
  |                PRESENTATION LAYER                           |
  |  +----------------------+  +-----------------------------+  |
  |  | CreditCardStatement  |  |  CreditCardStatement.API    |  |
  |  |       .MVC           |  |  - REST Controllers         |  |
  |  |  - Controllers       |  |  - Swagger / OpenAPI        |  |
  |  |  - Views (Razor)     |  |  - Global Exception Handler |  |
  |  |  - ViewModels        |  |  - Health Checks            |  |
  |  |  - jQuery / AJAX     |  |  - JWT Authentication       |  |
  |  +----------+-----------+  +--------------+--------------+  |
  |             |                             |                 |
  |             +-------------+---------------+                 |
  |                           | HTTP Client                     |
  +---------------------------+---------------------------------+
                              |
                              v
  +-------------------------------------------------------------+
  |                APPLICATION LAYER                            |
  |              CreditCardStatement.Application                |
  |                                                             |
  |   +----------------+     +----------------+                |
  |   |   Commands     |     |    Queries     |                |
  |   |   (Escritura)  |     |    (Lectura)   |                |
  |   +--------+-------+     +--------+-------+                |
  |            |                      |                         |
  |            +----------+-----------+                         |
  |                       |                                     |
  |            +----------v-----------+                         |
  |            |       Handlers       |                         |
  |            |  - CreatePurchase    |                         |
  |            |  - MakePayment       |                         |
  |            |  - GetStatement      |                         |
  |            |  - GetTransactions   |                         |
  |            |  - Login             |                         |
  |            +----------+-----------+                         |
  |                       |                                     |
  |   +-------------------+-------------------+                |
  |   | Validators (FluentValidation)          |                |
  |   | Mappings (AutoMapper)                  |                |
  |   | DTOs                                   |                |
  |   +----------------------------------------+                |
  +---------------------------+---------------------------------+
                              |
                              v
  +-------------------------------------------------------------+
  |              INFRASTRUCTURE LAYER                           |
  |            CreditCardStatement.Infrastructure               |
  |                                                             |
  |   +-------------------+  +-------------------+             |
  |   |   DbContext       |  |  Repositories     |             |
  |   |   (EF Core)       |  |  - CreditCard     |             |
  |   |                   |  |  - Transaction    |             |
  |   +-------------------+  |  - CardHolder     |             |
  |                          +-------------------+             |
  |   +-------------------+  +-------------------+             |
  |   |   UnitOfWork      |  | StoredProcedure   |             |
  |   |                   |  | Service           |             |
  |   +-------------------+  +-------------------+             |
  +---------------------------+---------------------------------+
                              |
                              v
  +-------------------------------------------------------------+
  |                   DOMAIN LAYER                              |
  |              CreditCardStatement.Domain                     |
  |                                                             |
  |   +-------------------+  +-------------------+             |
  |   |    Entities       |  |    Enums          |             |
  |   |  - CardHolder     |  |  - TransactionType|             |
  |   |  - CreditCard     |  |    (Purchase=1,   |             |
  |   |  - Transaction    |  |     Payment=2)    |             |
  |   +-------------------+  +-------------------+             |
  |                                                             |
  |   +-------------------+                                     |
  |   |   Interfaces      |                                     |
  |   |  (Repositories)   |                                     |
  |   +-------------------+                                     |
  +---------------------------+---------------------------------+
                              |
                              v
  +-------------------------------------------------------------+
  |              SQL SERVER DATABASE                            |
  |                                                             |
  |   +-------------------+  +-------------------+             |
  |   |     Tablas        |  | Stored Procedures |             |
  |   |  - CardHolders    |  |  - sp_GetStatement|             |
  |   |  - CreditCards    |  |  - sp_GetTrans... |             |
  |   |  - Transactions   |  |  - sp_AddPurchase |             |
  |   |                   |  |  - sp_AddPayment  |             |
  |   +-------------------+  +-------------------+             |
  +-------------------------------------------------------------+
```

La regla de dependencia apunta siempre hacia el centro: **Domain** no depende de ninguna otra capa. Las capas externas dependen de las internas a traves de interfaces e inyeccion de dependencias.

Para mas detalles sobre la arquitectura, consulta [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Requisitos Previos

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB o instancia completa)
- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) o [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/downloads)

## Instalacion Rapida

### 1. Clonar el Repositorio

```bash
git clone https://github.com/usuario/CreditCardStatement.git
cd CreditCardStatement
```

### 2. Configurar Base de Datos

Ejecutar los scripts SQL en orden usando sqlcmd o SQL Server Management Studio:

```bash
# 1. Crear tablas
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\01_CreateTables.sql

# 2. Crear procedimientos almacenados
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\02_CreateStoredProcedures.sql

# 3. Insertar datos de prueba
sqlcmd -S (localdb)\mssqllocaldb -i database\scripts\03_SeedData.sql
```

### 3. Actualizar Connection Strings

Actualizar `appsettings.json` en ambos proyectos (API y MVC):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CreditCardStatementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 4. Compilar y Ejecutar

```bash
# Restaurar paquetes
dotnet restore

# Compilar solucion
dotnet build

# Ejecutar API (Terminal 1)
cd src/CreditCardStatement.API
dotnet run

# Ejecutar MVC (Terminal 2)
cd src/CreditCardStatement.MVC
dotnet run
```

## Uso

Una vez ejecutando ambos proyectos:

| Servicio | URL |
|----------|-----|
| API + Swagger | https://localhost:7001/swagger |
| Aplicacion MVC | https://localhost:5001 |
| Health Check | https://localhost:7001/health |

### Credenciales por Defecto

| Usuario | Email | Contrasena |
|---------|-------|------------|
| Henry Aquino | henry.aq@mail.com | contrasena123 |
| Henry Guzman | henry.guz@mail.com | contrasena123 |

### Flujo de Uso Tipico

1. Acceder a la aplicacion MVC en `https://localhost:5001`
2. Iniciar sesion con las credenciales proporcionadas
3. Seleccionar una tarjeta de credito del dropdown
4. Visualizar el estado de cuenta con calculos financieros
5. Registrar compras o pagos desde los modales correspondientes
6. Exportar el estado de cuenta a PDF si es necesario

## API Endpoints

| Metodo | Endpoint | Descripcion |
|--------|----------|-------------|
| POST | `/api/auth/login` | Autenticar usuario y obtener token JWT |
| GET | `/api/creditcard/cards/{cardHolderId}` | Obtener tarjetas de credito del titular |
| GET | `/api/creditcard/statement/{creditCardId}` | Obtener estado de cuenta completo |
| GET | `/api/creditcard/transactions/{creditCardId}` | Obtener transacciones por mes |
| POST | `/api/creditcard/purchase` | Registrar nueva compra |
| POST | `/api/creditcard/payment` | Registrar pago |
| GET | `/health` | Verificar estado de la aplicacion |

Para documentacion completa de la API con ejemplos de request/response, consulta [docs/API.md](docs/API.md).

## Calculos Financieros

| Concepto | Formula | Ejemplo (Saldo = $114.47) |
|----------|---------|---------------------------|
| Interes Bonificable | Saldo * Tasa de Interes (25%) | $28.62 |
| Cuota Minima | Saldo * Tasa Minima (5%) | $5.72 |
| Total a Pagar | Saldo Actual | $114.47 |
| Pago de Contado | Saldo + Interes Bonificable | $143.09 |
| Saldo Disponible | Limite de Credito - Saldo Actual | $4,885.53 |

Las tasas de interes y pago minimo son configurables por tarjeta.

## Estructura del Proyecto

```
CreditCardStatement/
├── src/
│   ├── CreditCardStatement.Domain/           # Entidades, Enums, Interfaces
│   ├── CreditCardStatement.Application/      # CQRS (MediatR), Validators, Mappings
│   ├── CreditCardStatement.Infrastructure/   # DbContext, Repositories, UnitOfWork
│   ├── CreditCardStatement.API/              # REST API + Swagger + Health Checks
│   └── CreditCardStatement.MVC/              # MVC Frontend + Razor + jQuery
├── database/
│   ├── scripts/                              # Scripts SQL
│   └── postman/                              # Coleccion Postman
├── docs/                                     # Documentacion completa
└── README.md                                 # Este archivo
```

## Documentacion

La carpeta `docs/` contiene la documentacion tecnica completa del proyecto:

| Documento | Contenido |
|-----------|-----------|
| [docs/README.md](docs/README.md) | Guia general del proyecto |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Arquitectura, patrones y flujo de datos |
| [docs/API.md](docs/API.md) | Documentacion completa de endpoints |
| [docs/DATABASE.md](docs/DATABASE.md) | Modelo de datos, scripts SQL y SPs |
| [docs/SETUP.md](docs/SETUP.md) | Guia de instalacion y configuracion detallada |
| [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) | Opciones de despliegue (Docker, Azure) |
| [docs/TESTING.md](docs/TESTING.md) | Guia de pruebas (Postman, cURL, UI) |
| [docs/CONTRIBUTING.md](docs/CONTRIBUTING.md) | Convenciones de codigo y flujo de trabajo |
| [docs/CHANGELOG.md](docs/CHANGELOG.md) | Historial de cambios |
| [docs/UML_DIAGRAMS.md](docs/UML_DIAGRAMS.md) | Diagramas UML en PlantUML |

## Autor

**Henry Ernesto Aquino Guzman**

## Licencia

Proyecto de prueba tecnica con fines educativos.
