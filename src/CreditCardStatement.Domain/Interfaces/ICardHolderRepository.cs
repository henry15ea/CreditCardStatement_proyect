using CreditCardStatement.Domain.Entities;

namespace CreditCardStatement.Domain.Interfaces
{
    public interface ICardHolderRepository
    {
        Task<CardHolder?> GetByIdAsync(int id);
        Task<CardHolder?> GetByEmailAsync(string email);
        Task<IEnumerable<CardHolder>> GetAllAsync();
        Task<CardHolder> CreateAsync(CardHolder cardHolder);
        Task UpdateAsync(CardHolder cardHolder);
        Task DeleteAsync(int id);
    }
}
