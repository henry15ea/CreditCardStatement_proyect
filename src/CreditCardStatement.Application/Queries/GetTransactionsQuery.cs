using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Queries
{
    public class GetTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public int CreditCardId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
