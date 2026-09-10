using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Commands
{
    public class CreatePurchaseCommand : IRequest<TransactionDto>
    {
        public CreatePurchaseDto Purchase { get; set; } = null!;
    }
}
