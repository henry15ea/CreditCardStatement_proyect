using CreditCardStatement.Domain.Enums;

namespace CreditCardStatement.Application.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int CreditCardId { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TypeName => Type == TransactionType.Purchase ? "Purchase" : "Payment";
    }
}
