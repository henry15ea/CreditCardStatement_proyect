using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Queries
{
    public class GetStatementQuery : IRequest<StatementDto>
    {
        public int CreditCardId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
