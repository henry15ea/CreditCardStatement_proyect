namespace CreditCardStatement.MVC.ViewModels
{
    public class PurchaseViewModel
    {
        public int CreditCardId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
