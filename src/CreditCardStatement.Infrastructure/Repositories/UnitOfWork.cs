using CreditCardStatement.Domain.Interfaces;
using CreditCardStatement.Infrastructure.Data;

namespace CreditCardStatement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CreditCardDbContext _context;
        private ICreditCardRepository? _creditCards;
        private ITransactionRepository? _transactions;
        private ICardHolderRepository? _cardHolders;

        public UnitOfWork(CreditCardDbContext context)
        {
            _context = context;
        }

        public ICreditCardRepository CreditCards =>
            _creditCards ??= new CreditCardRepository(_context);

        public ITransactionRepository Transactions =>
            _transactions ??= new TransactionRepository(_context);

        public ICardHolderRepository CardHolders =>
            _cardHolders ??= new CardHolderRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
