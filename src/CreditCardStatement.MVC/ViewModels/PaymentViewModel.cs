namespace CreditCardStatement.MVC.ViewModels
{
    public class PaymentViewModel
    {
        public int CreditCardId { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
    }
}
