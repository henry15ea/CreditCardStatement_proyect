namespace CreditCardStatement.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICreditCardRepository CreditCards { get; }
        ITransactionRepository Transactions { get; }
        ICardHolderRepository CardHolders { get; }
        Task<int> SaveChangesAsync();
    }
}
