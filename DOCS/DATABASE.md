# Base de Datos - Credit Card Statement

## Modelo de Datos

### Diagrama Entidad-Relación

```
┌─────────────────────────┐
│      CardHolders        │
├─────────────────────────┤
│ Id (PK)                 │
│ Name                    │
│ Email (UNIQUE)          │
│ PasswordHash            │
│ CreatedAt               │
└───────────┬─────────────┘
            │ 1
            │
            │ N
┌───────────▼─────────────┐
│      CreditCards        │
├─────────────────────────┤
│ Id (PK)                 │
│ CardHolderId (FK)       │
│ CardNumber (UNIQUE)     │
│ CreditLimit             │
│ CurrentBalance          │
│ InterestRate            │
│ MinimumPaymentRate      │
│ CreatedAt               │
└───────────┬─────────────┘
            │ 1
            │
            │ N
┌───────────▼─────────────┐
│      Transactions       │
├─────────────────────────┤
│ Id (PK)                 │
│ CreditCardId (FK)       │
│ Type (1=Purchase, 2=Pay)│
│ Date                    │
│ Description             │
│ Amount                  │
│ CreatedAt               │
└─────────────────────────┘
```

## Scripts SQL

### 01_CreateTables.sql

Crea las tablas de la base de datos:

```sql
-- CardHolders
CREATE TABLE CardHolders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_CardHolders_Email UNIQUE (Email)
);

-- CreditCards
CREATE TABLE CreditCards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CardHolderId INT NOT NULL,
    CardNumber NVARCHAR(20) NOT NULL,
    CreditLimit DECIMAL(18,2) NOT NULL DEFAULT 0,
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    InterestRate DECIMAL(5,2) NOT NULL DEFAULT 25.00,
    MinimumPaymentRate DECIMAL(5,2) NOT NULL DEFAULT 5.00,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_CreditCards_CardNumber UNIQUE (CardNumber),
    CONSTRAINT FK_CreditCards_CardHolders FOREIGN KEY (CardHolderId) 
        REFERENCES CardHolders(Id) ON DELETE CASCADE
);

-- Transactions
CREATE TABLE Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CreditCardId INT NOT NULL,
    Type INT NOT NULL,
    Date DATETIME2 NOT NULL,
    Description NVARCHAR(200) NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Transactions_CreditCards FOREIGN KEY (CreditCardId) 
        REFERENCES CreditCards(Id) ON DELETE CASCADE
);
```

### 02_CreateStoredProcedures.sql

Crea los procedimientos almacenados:

#### sp_GetStatement
```sql
CREATE PROCEDURE sp_GetStatement
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Información de la tarjeta
    SELECT * FROM CreditCards cc
    INNER JOIN CardHolders ch ON cc.CardHolderId = ch.Id
    WHERE cc.Id = @CreditCardId;

    -- Compras del mes actual
    SELECT ISNULL(SUM(Amount), 0) AS TotalPurchasesCurrentMonth
    FROM Transactions
    WHERE CreditCardId = @CreditCardId AND Type = 1
        AND MONTH(Date) = @Month AND YEAR(Date) = @Year;

    -- Compras del mes anterior
    DECLARE @PrevMonth INT = CASE WHEN @Month = 1 THEN 12 ELSE @Month - 1 END;
    DECLARE @PrevYear INT = CASE WHEN @Month = 1 THEN @Year - 1 ELSE @Year END;

    SELECT ISNULL(SUM(Amount), 0) AS TotalPurchasesPreviousMonth
    FROM Transactions
    WHERE CreditCardId = @CreditCardId AND Type = 1
        AND MONTH(Date) = @PrevMonth AND YEAR(Date) = @PrevYear;

    -- Valores calculados
    SELECT 
        CurrentBalance,
        CreditLimit,
        (CurrentBalance * InterestRate / 100) AS BonifiableInterest,
        (CurrentBalance * MinimumPaymentRate / 100) AS MinimumPayment,
        CurrentBalance AS TotalToPay,
        (CurrentBalance + (CurrentBalance * InterestRate / 100)) AS CashPaymentWithInterest,
        (CreditLimit - CurrentBalance) AS AvailableBalance
    FROM CreditCards WHERE Id = @CreditCardId;

    -- Transacciones del mes
    SELECT * FROM Transactions
    WHERE CreditCardId = @CreditCardId
        AND MONTH(Date) = @Month AND YEAR(Date) = @Year
    ORDER BY Date DESC;
END
```

#### sp_AddPurchase
```sql
CREATE PROCEDURE sp_AddPurchase
    @CreditCardId INT,
    @Date DATETIME2,
    @Description NVARCHAR(200),
    @Amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que la tarjeta existe
        IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId)
        BEGIN
            RAISERROR('Credit card not found', 16, 1);
            RETURN;
        END

        -- Insertar transacción
        INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount, CreatedAt)
        VALUES (@CreditCardId, 1, @Date, @Description, @Amount, GETUTCDATE());

        -- Actualizar saldo
        UPDATE CreditCards
        SET CurrentBalance = CurrentBalance + @Amount
        WHERE Id = @CreditCardId;

        COMMIT TRANSACTION;

        -- Retornar nueva transacción
        SELECT * FROM Transactions WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

#### sp_AddPayment
```sql
CREATE PROCEDURE sp_AddPayment
    @CreditCardId INT,
    @Date DATETIME2,
    @Amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que la tarjeta existe
        IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId)
        BEGIN
            RAISERROR('Credit card not found', 16, 1);
            RETURN;
        END

        -- Insertar transacción
        INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount, CreatedAt)
        VALUES (@CreditCardId, 2, @Date, 'Payment', @Amount, GETUTCDATE());

        -- Actualizar saldo (asegurar que no sea negativo)
        UPDATE CreditCards
        SET CurrentBalance = CASE 
            WHEN CurrentBalance - @Amount < 0 THEN 0 
            ELSE CurrentBalance - @Amount 
        END
        WHERE Id = @CreditCardId;

        COMMIT TRANSACTION;

        -- Retornar nueva transacción
        SELECT * FROM Transactions WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

#### sp_CalculateMinimumPayment
```sql
CREATE PROCEDURE sp_CalculateMinimumPayment
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CurrentBalance,
        MinimumPaymentRate,
        (CurrentBalance * MinimumPaymentRate / 100) AS MinimumPayment
    FROM CreditCards WHERE Id = @CreditCardId;
END
```

#### sp_CalculateBonifiableInterest
```sql
CREATE PROCEDURE sp_CalculateBonifiableInterest
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CurrentBalance,
        InterestRate,
        (CurrentBalance * InterestRate / 100) AS BonifiableInterest
    FROM CreditCards WHERE Id = @CreditCardId;
END
```

### 03_SeedData.sql

Inserta datos de prueba:

```sql
-- Titulares
INSERT INTO CardHolders (Name, Email, PasswordHash) VALUES
('Henry Aquino', 'henry.aq@mail.com', '$2a$11$424ycH0jcwpuNC5eP/jseu4XmBI6mHyqDMO5v402pbEPjI0o5n8Wa'),
('Henry Guzman', 'henry.guz@mail.com', '$2a$11$Kleys979S7UnBtJOsh1.Iu5BV6dNwOtf1Uozv9IxtNzfL0mBGueV.');

-- Tarjetas de crédito
INSERT INTO CreditCards (CardHolderId, CardNumber, CreditLimit, CurrentBalance, InterestRate, MinimumPaymentRate) VALUES
(1, '4532015112830366', 5000.00, 114.47, 25.00, 5.00),
(1, '5425233430109903', 10000.00, 0.00, 25.00, 5.00),
(2, '4916338835470827', 7500.00, 0.00, 25.00, 5.00);

-- Transacciones de ejemplo
INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount) VALUES
(1, 1, DATEADD(DAY, -2, GETUTCDATE()), 'Amazon Purchase', 45.99),
(1, 1, DATEADD(DAY, -5, GETUTCDATE()), 'Netflix Subscription', 15.99),
(1, 1, DATEADD(DAY, -8, GETUTCDATE()), 'Gas Station', 35.00),
(1, 1, DATEADD(DAY, -12, GETUTCDATE()), 'Grocery Store', 17.49),
(1, 2, DATEADD(DAY, -15, GETUTCDATE()), 'Payment', 200.00);
```

## Configuración de Conexión

### appsettings.json (API)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CreditCardStatementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Entity Framework DbContext
```csharp
public class CreditCardDbContext : DbContext
{
    public CreditCardDbContext(DbContextOptions<CreditCardDbContext> options) 
        : base(options) { }

    public DbSet<CardHolder> CardHolders => Set<CardHolder>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de entidades...
    }
}
```

## Índices

```sql
-- Índice para búsquedas por tarjeta y fecha
CREATE INDEX IX_Transactions_CreditCardId_Date 
ON Transactions(CreditCardId, Date);

-- Índice para búsquedas por titular
CREATE INDEX IX_CreditCards_CardHolderId 
ON CreditCards(CardHolderId);
```

## Configuración por Defecto

| Parámetro | Valor |
|-----------|-------|
| InterestRate | 25% |
| MinimumPaymentRate | 5% |
| CreditLimit (default) | $0.00 |
| CurrentBalance (default) | $0.00 |
