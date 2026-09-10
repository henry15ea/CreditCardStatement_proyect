namespace CreditCardStatement.MVC.ViewModels
{
    public class CreditCardSelectorViewModel
    {
        public int SelectedCardId { get; set; }
        public List<CreditCardOptionViewModel> CreditCards { get; set; } = new();
    }

    public class CreditCardOptionViewModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public decimal CurrentBalance { get; set; }
    }
}
