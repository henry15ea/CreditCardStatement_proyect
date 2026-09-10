namespace CreditCardStatement.Domain.Entities
{
    public class CardHolder
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
    }
}
