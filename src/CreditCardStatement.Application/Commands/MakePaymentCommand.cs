using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Commands
{
    public class MakePaymentCommand : IRequest<TransactionDto>
    {
        public CreatePaymentDto Payment { get; set; } = null!;
    }
}
