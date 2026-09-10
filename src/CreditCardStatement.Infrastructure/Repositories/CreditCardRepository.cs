using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditCardStatement.Infrastructure.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly CreditCardDbContext _context;

        public CreditCardRepository(CreditCardDbContext context)
        {
            _context = context;
        }

        public async Task<CreditCard?> GetByIdAsync(int id)
        {
            return await _context.CreditCards
                .Include(cc => cc.CardHolder)
                .Include(cc => cc.Transactions)
                .FirstOrDefaultAsync(cc => cc.Id == id);
        }

        public async Task<CreditCard?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.CreditCards
                .Include(cc => cc.CardHolder)
                .Include(cc => cc.Transactions)
                .FirstOrDefaultAsync(cc => cc.CardNumber == cardNumber);
        }

        public async Task<IEnumerable<CreditCard>> GetByCardHolderIdAsync(int cardHolderId)
        {
            return await _context.CreditCards
                .Include(cc => cc.CardHolder)
                .Where(cc => cc.CardHolderId == cardHolderId)
                .ToListAsync();
        }

        public Task<CreditCard> CreateAsync(CreditCard creditCard)
        {
            creditCard.CreatedAt = DateTime.UtcNow;
            _context.CreditCards.Add(creditCard);
            return Task.FromResult(creditCard);
        }

        public Task UpdateAsync(CreditCard creditCard)
        {
            _context.CreditCards.Update(creditCard);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard != null)
            {
                _context.CreditCards.Remove(creditCard);
            }
        }
    }
}
