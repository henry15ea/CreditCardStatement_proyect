using AutoMapper;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Application.Queries;
using CreditCardStatement.Domain.Enums;
using CreditCardStatement.Domain.Interfaces;
using MediatR;

namespace CreditCardStatement.Application.Handlers
{
    public class GetStatementHandler : IRequestHandler<GetStatementQuery, StatementDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStatementHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<StatementDto> Handle(GetStatementQuery request, CancellationToken cancellationToken)
        {
            var creditCard = await _unitOfWork.CreditCards.GetByIdAsync(request.CreditCardId);
            if (creditCard == null)
                throw new InvalidOperationException($"Credit card with ID {request.CreditCardId} not found.");

            var transactions = await _unitOfWork.Transactions.GetByMonthAsync(
                request.CreditCardId,
                request.Month,
                request.Year);

            var previousMonth = request.Month == 1 ? 12 : request.Month - 1;
            var previousYear = request.Month == 1 ? request.Year - 1 : request.Year;

            var previousMonthTransactions = await _unitOfWork.Transactions.GetByMonthAsync(
                request.CreditCardId,
                previousMonth,
                previousYear);

            var totalPurchasesCurrentMonth = transactions
                .Where(t => t.Type == TransactionType.Purchase)
                .Sum(t => t.Amount);

            var totalPurchasesPreviousMonth = previousMonthTransactions
                .Where(t => t.Type == TransactionType.Purchase)
                .Sum(t => t.Amount);

            var bonifiableInterest = creditCard.CurrentBalance * (creditCard.InterestRate / 100);
            var minimumPayment = creditCard.CurrentBalance * (creditCard.MinimumPaymentRate / 100);
            var totalToPay = creditCard.CurrentBalance;
            var cashPaymentWithInterest = creditCard.CurrentBalance + bonifiableInterest;
            var availableBalance = creditCard.CreditLimit - creditCard.CurrentBalance;

            var statementDto = new StatementDto
            {
                CreditCard = _mapper.Map<CreditCardDto>(creditCard),
                TotalPurchasesCurrentMonth = totalPurchasesCurrentMonth,
                TotalPurchasesPreviousMonth = totalPurchasesPreviousMonth,
                BonifiableInterest = Math.Round(bonifiableInterest, 2),
                MinimumPayment = Math.Round(minimumPayment, 2),
                TotalToPay = totalToPay,
                CashPaymentWithInterest = Math.Round(cashPaymentWithInterest, 2),
                AvailableBalance = availableBalance,
                Transactions = _mapper.Map<List<TransactionDto>>(transactions)
            };

            return statementDto;
        }
    }
}
