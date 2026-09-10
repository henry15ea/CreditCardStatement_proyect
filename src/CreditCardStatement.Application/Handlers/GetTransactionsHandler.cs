using AutoMapper;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Application.Queries;
using CreditCardStatement.Domain.Interfaces;
using MediatR;

namespace CreditCardStatement.Application.Handlers
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTransactionsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _unitOfWork.Transactions.GetByMonthAsync(
                request.CreditCardId,
                request.Month,
                request.Year);

            return _mapper.Map<List<TransactionDto>>(transactions);
        }
    }
}
