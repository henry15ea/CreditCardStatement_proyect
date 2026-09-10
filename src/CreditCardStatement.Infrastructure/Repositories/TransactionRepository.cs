using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Enums;
using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditCardStatement.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly CreditCardDbContext _context;

        public TransactionRepository(CreditCardDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.CreditCard)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaction>> GetByCreditCardIdAsync(int creditCardId)
        {
            return await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByMonthAsync(int creditCardId, int month, int year)
        {
            return await _context.Transactions
                .Where(t => t.CreditCardId == creditCardId
                    && t.Date.Month == month
                    && t.Date.Year == year)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public Task<Transaction> CreateAsync(Transaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            return Task.FromResult(transaction);
        }

        public Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
        }
    }
}
