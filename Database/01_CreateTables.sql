-- ============================================
-- Credit Card Statement Database
-- Create Tables Script
-- ============================================

USE master;
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'CreditCardStatementDb')
BEGIN
    CREATE DATABASE CreditCardStatementDb;
END
GO

USE CreditCardStatementDb;
GO

-- Create CardHolders table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CardHolders' AND xtype='U')
BEGIN
    CREATE TABLE CardHolders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_CardHolders_Email UNIQUE (Email)
    );
END
GO

-- Create CreditCards table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CreditCards' AND xtype='U')
BEGIN
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
END
GO

-- Create Transactions table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Transactions' AND xtype='U')
BEGIN
    CREATE TABLE Transactions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CreditCardId INT NOT NULL,
        Type INT NOT NULL, -- 1=Purchase, 2=Payment
        Date DATETIME2 NOT NULL,
        Description NVARCHAR(200) NULL,
        Amount DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Transactions_CreditCards FOREIGN KEY (CreditCardId) 
            REFERENCES CreditCards(Id) ON DELETE CASCADE
    );
END
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Transactions_CreditCardId_Date')
BEGIN
    CREATE INDEX IX_Transactions_CreditCardId_Date 
    ON Transactions(CreditCardId, Date);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CreditCards_CardHolderId')
BEGIN
    CREATE INDEX IX_CreditCards_CardHolderId 
    ON CreditCards(CardHolderId);
END
GO

PRINT 'Tables created successfully!';
GO
