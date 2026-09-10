namespace CreditCardStatement.Application.DTOs
{
    public class StatementDto
    {
        public CreditCardDto CreditCard { get; set; } = null!;
        public decimal TotalPurchasesCurrentMonth { get; set; }
        public decimal TotalPurchasesPreviousMonth { get; set; }
        public decimal BonifiableInterest { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal CashPaymentWithInterest { get; set; }
        public decimal AvailableBalance { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new();
    }
}
