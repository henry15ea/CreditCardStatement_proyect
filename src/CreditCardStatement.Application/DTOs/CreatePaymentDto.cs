namespace CreditCardStatement.Application.DTOs
{
    public class CreatePaymentDto
    {
        public int CreditCardId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
