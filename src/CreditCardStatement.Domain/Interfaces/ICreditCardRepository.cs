using CreditCardStatement.Domain.Entities;

namespace CreditCardStatement.Domain.Interfaces
{
    public interface ICreditCardRepository
    {
        Task<CreditCard?> GetByIdAsync(int id);
        Task<CreditCard?> GetByCardNumberAsync(string cardNumber);
        Task<IEnumerable<CreditCard>> GetByCardHolderIdAsync(int cardHolderId);
        Task<CreditCard> CreateAsync(CreditCard creditCard);
        Task UpdateAsync(CreditCard creditCard);
        Task DeleteAsync(int id);
    }
}
