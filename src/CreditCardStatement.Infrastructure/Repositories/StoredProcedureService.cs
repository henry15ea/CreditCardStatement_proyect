using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Enums;
using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CreditCardStatement.Infrastructure.Repositories
{
    public class StoredProcedureService : IStoredProcedureService
    {
        private readonly CreditCardDbContext _context;

        public StoredProcedureService(CreditCardDbContext context)
        {
            _context = context;
        }

        public async Task<StatementResult> GetStatementAsync(int creditCardId, int month, int year)
        {
            var creditCard = await _context.CreditCards
                .Include(cc => cc.CardHolder)
                .FirstOrDefaultAsync(cc => cc.Id == creditCardId);

            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {creditCardId} not found.");

            var transactions = await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId
                    && t.Date.Month == month
                    && t.Date.Year == year)
                .ToListAsync();

            var previousMonth = month == 1 ? 12 : month - 1;
            var previousYear = month == 1 ? year - 1 : year;

            var previousMonthTransactions = await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId
                    && t.Date.Month == previousMonth
                    && t.Date.Year == previousYear)
                .ToListAsync();

            var totalPurchasesCurrentMonth = transactions
                .Where(t => t.Type == TransactionType.Purchase)
                .Sum(t => t.Amount);

            var totalPurchasesPreviousMonth = previousMonthTransactions
                .Where(t => t.Type == TransactionType.Purchase)
                .Sum(t => t.Amount);

            var bonifiableInterest = creditCard.CurrentBalance * (creditCard.InterestRate / 100);
            var minimumPayment = creditCard.CurrentBalance * (creditCard.MinimumPaymentRate / 100);
            var totalToPay = creditCard.CurrentBalance;
            var cashPaymentWithInterest = creditCard.CurrentBalance + bonifiableInterest;
            var availableBalance = creditCard.CreditLimit - creditCard.CurrentBalance;

            return new StatementResult
            {
                CreditCard = creditCard,
                TotalPurchasesCurrentMonth = totalPurchasesCurrentMonth,
                TotalPurchasesPreviousMonth = totalPurchasesPreviousMonth,
                BonifiableInterest = Math.Round(bonifiableInterest, 2),
                MinimumPayment = Math.Round(minimumPayment, 2),
                TotalToPay = totalToPay,
                CashPaymentWithInterest = Math.Round(cashPaymentWithInterest, 2),
                AvailableBalance = availableBalance
            };
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int creditCardId, int month, int year)
        {
            return await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId
                    && t.Date.Month == month
                    && t.Date.Year == year)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task AddPurchaseAsync(int creditCardId, DateTime date, string description, decimal amount)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {creditCardId} not found.");

            var transaction = new Transaction
            {
                CreditCardId = creditCardId,
                Type = TransactionType.Purchase,
                Date = date,
                Description = description,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            creditCard.CurrentBalance += amount;

            _context.Transactions.Add(transaction);
            _context.CreditCards.Update(creditCard);
            await _context.SaveChangesAsync();
        }

        public async Task AddPaymentAsync(int creditCardId, DateTime date, decimal amount)
        {
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {creditCardId} not found.");

            var transaction = new Transaction
            {
                CreditCardId = creditCardId,
                Type = TransactionType.Payment,
                Date = date,
                Description = "Payment",
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            creditCard.CurrentBalance -= amount;
            if (creditCard.CurrentBalance < 0)
                creditCard.CurrentBalance = 0;

            _context.Transactions.Add(transaction);
            _context.CreditCards.Update(creditCard);
            await _context.SaveChangesAsync();
        }
    }
}
