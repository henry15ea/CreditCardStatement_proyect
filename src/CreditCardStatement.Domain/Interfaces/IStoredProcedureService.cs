using CreditCardStatement.Domain.Entities;

namespace CreditCardStatement.Domain.Interfaces
{
    public interface IStoredProcedureService
    {
        Task<StatementResult> GetStatementAsync(int creditCardId, int month, int year);
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int creditCardId, int month, int year);
        Task AddPurchaseAsync(int creditCardId, DateTime date, string description, decimal amount);
        Task AddPaymentAsync(int creditCardId, DateTime date, decimal amount);
    }

    public class StatementResult
    {
        public CreditCard CreditCard { get; set; } = null!;
        public decimal TotalPurchasesCurrentMonth { get; set; }
        public decimal TotalPurchasesPreviousMonth { get; set; }
        public decimal BonifiableInterest { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal CashPaymentWithInterest { get; set; }
        public decimal AvailableBalance { get; set; }
    }
}
