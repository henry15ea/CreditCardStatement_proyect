using CreditCardStatement.Application.Commands;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Enums;
using CreditCardStatement.Domain.Interfaces;
using MediatR;

namespace CreditCardStatement.Application.Handlers
{
    public class MakePaymentHandler : IRequestHandler<MakePaymentCommand, TransactionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MakePaymentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransactionDto> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
        {
            var creditCard = await _unitOfWork.CreditCards.GetByIdAsync(request.Payment.CreditCardId);
            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {request.Payment.CreditCardId} not found.");

            var transaction = new Transaction
            {
                CreditCardId = request.Payment.CreditCardId,
                Type = TransactionType.Payment,
                Date = request.Payment.Date,
                Description = "Payment",
                Amount = request.Payment.Amount
            };

            creditCard.CurrentBalance -= request.Payment.Amount;
            if (creditCard.CurrentBalance < 0)
                creditCard.CurrentBalance = 0;

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.CreditCards.UpdateAsync(creditCard);
            await _unitOfWork.SaveChangesAsync();

            return new TransactionDto
            {
                CreditCardId = request.Payment.CreditCardId,
                Type = TransactionType.Payment,
                Date = request.Payment.Date,
                Description = "Payment",
                Amount = request.Payment.Amount
            };
        }
    }
}
