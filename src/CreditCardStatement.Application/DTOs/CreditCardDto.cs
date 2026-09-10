namespace CreditCardStatement.Application.DTOs
{
    public class CreditCardDto
    {
        public int Id { get; set; }
        public int CardHolderId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumPaymentRate { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
    }
}
