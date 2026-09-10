using AutoMapper;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Application.Queries;
using CreditCardStatement.Domain.Interfaces;
using MediatR;

namespace CreditCardStatement.Application.Handlers
{
    public class GetCreditCardsHandler : IRequestHandler<GetCreditCardsQuery, List<CreditCardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCreditCardsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CreditCardDto>> Handle(GetCreditCardsQuery request, CancellationToken cancellationToken)
        {
            var creditCards = await _unitOfWork.CreditCards.GetByCardHolderIdAsync(request.CardHolderId);
            return _mapper.Map<List<CreditCardDto>>(creditCards);
        }
    }
}
