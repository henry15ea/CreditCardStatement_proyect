namespace CreditCardStatement.Domain.Entities
{
    public class CreditCard
    {
        public int Id { get; set; }
        public int CardHolderId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumPaymentRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public CardHolder CardHolder { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
