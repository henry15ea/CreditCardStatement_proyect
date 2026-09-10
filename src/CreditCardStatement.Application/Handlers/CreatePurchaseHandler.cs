using CreditCardStatement.Application.Commands;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Domain.Entities;
using CreditCardStatement.Domain.Enums;
using CreditCardStatement.Domain.Interfaces;
using MediatR;

namespace CreditCardStatement.Application.Handlers
{
    public class CreatePurchaseHandler : IRequestHandler<CreatePurchaseCommand, TransactionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePurchaseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransactionDto> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var creditCard = await _unitOfWork.CreditCards.GetByIdAsync(request.Purchase.CreditCardId);
            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {request.Purchase.CreditCardId} not found.");

            var transaction = new Transaction
            {
                CreditCardId = request.Purchase.CreditCardId,
                Type = TransactionType.Purchase,
                Date = request.Purchase.Date,
                Description = request.Purchase.Description,
                Amount = request.Purchase.Amount
            };

            creditCard.CurrentBalance += request.Purchase.Amount;

            await _unitOfWork.Transactions.CreateAsync(transaction);
            await _unitOfWork.CreditCards.UpdateAsync(creditCard);
            await _unitOfWork.SaveChangesAsync();

            return new TransactionDto
            {
                CreditCardId = request.Purchase.CreditCardId,
                Type = TransactionType.Purchase,
                Date = request.Purchase.Date,
                Description = request.Purchase.Description,
                Amount = request.Purchase.Amount
            };
        }
    }
}
