using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditCardStatement.Infrastructure.Repositories
{
    public class CardHolderRepository : ICardHolderRepository
    {
        private readonly CreditCardDbContext _context;

        public CardHolderRepository(CreditCardDbContext context)
        {
            _context = context;
        }

        public async Task<CardHolder?> GetByIdAsync(int id)
        {
            return await _context.CardHolders
                .Include(ch => ch.CreditCards)
                .FirstOrDefaultAsync(ch => ch.Id == id);
        }

        public async Task<CardHolder?> GetByEmailAsync(string email)
        {
            return await _context.CardHolders
                .Include(ch => ch.CreditCards)
                .FirstOrDefaultAsync(ch => ch.Email == email);
        }

        public async Task<IEnumerable<CardHolder>> GetAllAsync()
        {
            return await _context.CardHolders
                .Include(ch => ch.CreditCards)
                .ToListAsync();
        }

        public Task<CardHolder> CreateAsync(CardHolder cardHolder)
        {
            cardHolder.CreatedAt = DateTime.UtcNow;
            _context.CardHolders.Add(cardHolder);
            return Task.FromResult(cardHolder);
        }

        public Task UpdateAsync(CardHolder cardHolder)
        {
            _context.CardHolders.Update(cardHolder);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var cardHolder = await _context.CardHolders.FindAsync(id);
            if (cardHolder != null)
            {
                _context.CardHolders.Remove(cardHolder);
            }
        }
    }
}
