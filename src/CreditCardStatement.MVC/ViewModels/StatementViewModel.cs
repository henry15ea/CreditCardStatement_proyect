namespace CreditCardStatement.MVC.ViewModels
{
    public class StatementViewModel
    {
        public int CreditCardId { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal TotalPurchasesCurrentMonth { get; set; }
        public decimal TotalPurchasesPreviousMonth { get; set; }
        public decimal BonifiableInterest { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal CashPaymentWithInterest { get; set; }
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public List<TransactionViewModel> Transactions { get; set; } = new();
    }
}
