-- ============================================
-- Credit Card Statement Database
-- Stored Procedures Script
-- ============================================

USE CreditCardStatementDb;
GO

-- ============================================
-- sp_GetStatement
-- Returns credit card statement for a given month/year
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetStatement')
    DROP PROCEDURE sp_GetStatement;
GO

CREATE PROCEDURE sp_GetStatement
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Get credit card info
    SELECT 
        cc.Id,
        cc.CardHolderId,
        cc.CardNumber,
        cc.CreditLimit,
        cc.CurrentBalance,
        cc.InterestRate,
        cc.MinimumPaymentRate,
        ch.Name AS CardHolderName
    FROM CreditCards cc
    INNER JOIN CardHolders ch ON cc.CardHolderId = ch.Id
    WHERE cc.Id = @CreditCardId;

    -- Get total purchases for current month
    SELECT 
        ISNULL(SUM(Amount), 0) AS TotalPurchasesCurrentMonth
    FROM Transactions
    WHERE CreditCardId = @CreditCardId
        AND Type = 1 -- Purchase
        AND MONTH(Date) = @Month
        AND YEAR(Date) = @Year;

    -- Get total purchases for previous month
    DECLARE @PrevMonth INT = CASE WHEN @Month = 1 THEN 12 ELSE @Month - 1 END;
    DECLARE @PrevYear INT = CASE WHEN @Month = 1 THEN @Year - 1 ELSE @Year END;

    SELECT 
        ISNULL(SUM(Amount), 0) AS TotalPurchasesPreviousMonth
    FROM Transactions
    WHERE CreditCardId = @CreditCardId
        AND Type = 1 -- Purchase
        AND MONTH(Date) = @PrevMonth
        AND YEAR(Date) = @PrevYear;

    -- Get calculated values
    SELECT 
        CurrentBalance,
        CreditLimit,
        (CurrentBalance * InterestRate / 100) AS BonifiableInterest,
        (CurrentBalance * MinimumPaymentRate / 100) AS MinimumPayment,
        CurrentBalance AS TotalToPay,
        (CurrentBalance + (CurrentBalance * InterestRate / 100)) AS CashPaymentWithInterest,
        (CreditLimit - CurrentBalance) AS AvailableBalance
    FROM CreditCards
    WHERE Id = @CreditCardId;

    -- Get transactions for current month
    SELECT 
        Id,
        CreditCardId,
        Type,
        Date,
        Description,
        Amount
    FROM Transactions
    WHERE CreditCardId = @CreditCardId
        AND MONTH(Date) = @Month
        AND YEAR(Date) = @Year
    ORDER BY Date DESC;
END
GO

-- ============================================
-- sp_GetTransactions
-- Returns all transactions for a given month/year
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetTransactions')
    DROP PROCEDURE sp_GetTransactions;
GO

CREATE PROCEDURE sp_GetTransactions
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        CreditCardId,
        Type,
        Date,
        Description,
        Amount
    FROM Transactions
    WHERE CreditCardId = @CreditCardId
        AND MONTH(Date) = @Month
        AND YEAR(Date) = @Year
    ORDER BY Date DESC;
END
GO

-- ============================================
-- sp_AddPurchase
-- Adds a new purchase transaction
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_AddPurchase')
    DROP PROCEDURE sp_AddPurchase;
GO

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

        -- Validate credit card exists
        IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId)
        BEGIN
            RAISERROR('Credit card not found', 16, 1);
            RETURN;
        END

        -- Insert transaction
        INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount, CreatedAt)
        VALUES (@CreditCardId, 1, @Date, @Description, @Amount, GETUTCDATE());

        -- Update current balance
        UPDATE CreditCards
        SET CurrentBalance = CurrentBalance + @Amount
        WHERE Id = @CreditCardId;

        COMMIT TRANSACTION;

        -- Return the new transaction
        SELECT 
            Id,
            CreditCardId,
            Type,
            Date,
            Description,
            Amount
        FROM Transactions
        WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- sp_AddPayment
-- Adds a new payment transaction
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_AddPayment')
    DROP PROCEDURE sp_AddPayment;
GO

CREATE PROCEDURE sp_AddPayment
    @CreditCardId INT,
    @Date DATETIME2,
    @Amount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validate credit card exists
        IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId)
        BEGIN
            RAISERROR('Credit card not found', 16, 1);
            RETURN;
        END

        -- Insert transaction
        INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount, CreatedAt)
        VALUES (@CreditCardId, 2, @Date, 'Payment', @Amount, GETUTCDATE());

        -- Update current balance (ensure it doesn't go below 0)
        UPDATE CreditCards
        SET CurrentBalance = CASE 
            WHEN CurrentBalance - @Amount < 0 THEN 0 
            ELSE CurrentBalance - @Amount 
        END
        WHERE Id = @CreditCardId;

        COMMIT TRANSACTION;

        -- Return the new transaction
        SELECT 
            Id,
            CreditCardId,
            Type,
            Date,
            Description,
            Amount
        FROM Transactions
        WHERE Id = SCOPE_IDENTITY();
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ============================================
-- sp_CalculateMinimumPayment
-- Returns the minimum payment for a credit card
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CalculateMinimumPayment')
    DROP PROCEDURE sp_CalculateMinimumPayment;
GO

CREATE PROCEDURE sp_CalculateMinimumPayment
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CurrentBalance,
        MinimumPaymentRate,
        (CurrentBalance * MinimumPaymentRate / 100) AS MinimumPayment
    FROM CreditCards
    WHERE Id = @CreditCardId;
END
GO

-- ============================================
-- sp_CalculateBonifiableInterest
-- Returns the bonifiable interest for a credit card
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CalculateBonifiableInterest')
    DROP PROCEDURE sp_CalculateBonifiableInterest;
GO

CREATE PROCEDURE sp_CalculateBonifiableInterest
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CurrentBalance,
        InterestRate,
        (CurrentBalance * InterestRate / 100) AS BonifiableInterest
    FROM CreditCards
    WHERE Id = @CreditCardId;
END
GO

PRINT 'Stored procedures created successfully!';
GO
