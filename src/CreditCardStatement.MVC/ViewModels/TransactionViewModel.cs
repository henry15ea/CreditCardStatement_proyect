namespace CreditCardStatement.MVC.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public int CreditCardId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
