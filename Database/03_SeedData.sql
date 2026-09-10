-- ============================================
-- Credit Card Statement Database
-- Seed Data Script
-- ============================================

USE CreditCardStatementDb;
GO

-- Password: "contraseña123" hashed with BCrypt
-- BCrypt hash for "contraseña123": $2a$11$424ycH0jcwpuNC5eP/jseu4XmBI6mHyqDMO5v402pbEPjI0o5n8Wa

-- Insert CardHolders
IF NOT EXISTS (SELECT 1 FROM CardHolders WHERE Email = 'henry.aq@mail.com')
BEGIN
    INSERT INTO CardHolders (Name, Email, PasswordHash, CreatedAt)
    VALUES ('Henry Aquino', 'henry.aq@mail.com', '$2a$11$424ycH0jcwpuNC5eP/jseu4XmBI6mHyqDMO5v402pbEPjI0o5n8Wa', GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM CardHolders WHERE Email = 'henry.guz@mail.com')
BEGIN
    INSERT INTO CardHolders (Name, Email, PasswordHash, CreatedAt)
    VALUES ('Henry Guzman', 'henry.guz@mail.com', '$2a$11$Kleys979S7UnBtJOsh1.Iu5BV6dNwOtf1Uozv9IxtNzfL0mBGueV.', GETUTCDATE());
END
GO

-- Insert CreditCards
IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE CardNumber = '4532015112830366')
BEGIN
    INSERT INTO CreditCards (CardHolderId, CardNumber, CreditLimit, CurrentBalance, InterestRate, MinimumPaymentRate, CreatedAt)
    VALUES (1, '4532015112830366', 5000.00, 114.47, 25.00, 5.00, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE CardNumber = '5425233430109903')
BEGIN
    INSERT INTO CreditCards (CardHolderId, CardNumber, CreditLimit, CurrentBalance, InterestRate, MinimumPaymentRate, CreatedAt)
    VALUES (1, '5425233430109903', 10000.00, 0.00, 25.00, 5.00, GETUTCDATE());
END
GO

IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE CardNumber = '4916338835470827')
BEGIN
    INSERT INTO CreditCards (CardHolderId, CardNumber, CreditLimit, CurrentBalance, InterestRate, MinimumPaymentRate, CreatedAt)
    VALUES (2, '4916338835470827', 7500.00, 0.00, 25.00, 5.00, GETUTCDATE());
END
GO

-- Insert sample transactions for card 1
IF NOT EXISTS (SELECT 1 FROM Transactions WHERE CreditCardId = 1 AND Description = 'Amazon Purchase')
BEGIN
    INSERT INTO Transactions (CreditCardId, Type, Date, Description, Amount, CreatedAt)
    VALUES 
        (1, 1, DATEADD(DAY, -2, GETUTCDATE()), 'Amazon Purchase', 45.99, GETUTCDATE()),
        (1, 1, DATEADD(DAY, -5, GETUTCDATE()), 'Netflix Subscription', 15.99, GETUTCDATE()),
        (1, 1, DATEADD(DAY, -8, GETUTCDATE()), 'Gas Station', 35.00, GETUTCDATE()),
        (1, 1, DATEADD(DAY, -12, GETUTCDATE()), 'Grocery Store', 17.49, GETUTCDATE()),
        (1, 2, DATEADD(DAY, -15, GETUTCDATE()), 'Payment', 200.00, GETUTCDATE());
END
GO

-- Update balance based on transactions
UPDATE CreditCards
SET CurrentBalance = ISNULL(
    (SELECT ISNULL(SUM(CASE WHEN Type = 1 THEN Amount ELSE -Amount END), 0)
     FROM Transactions 
     WHERE CreditCardId = CreditCards.Id), 0)
WHERE Id = 1;
GO

PRINT 'Seed data inserted successfully!';
GO

-- Verify data
SELECT 'CardHolders' AS TableName, COUNT(*) AS RecordCount FROM CardHolders
UNION ALL
SELECT 'CreditCards', COUNT(*) FROM CreditCards
UNION ALL
SELECT 'Transactions', COUNT(*) FROM Transactions;
GO
