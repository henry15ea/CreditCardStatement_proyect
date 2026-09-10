using CreditCardStatement.Domain.Entities;

namespace CreditCardStatement.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<IEnumerable<Transaction>> GetByCreditCardIdAsync(int creditCardId);
        Task<IEnumerable<Transaction>> GetByMonthAsync(int creditCardId, int month, int year);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id);
    }
}
