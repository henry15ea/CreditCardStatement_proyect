namespace CreditCardStatement.Application.DTOs
{
    public class CreatePurchaseDto
    {
        public int CreditCardId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
