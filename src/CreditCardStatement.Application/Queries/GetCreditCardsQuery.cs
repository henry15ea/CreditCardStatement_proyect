using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Queries
{
    public class GetCreditCardsQuery : IRequest<List<CreditCardDto>>
    {
        public int CardHolderId { get; set; }
    }
}
