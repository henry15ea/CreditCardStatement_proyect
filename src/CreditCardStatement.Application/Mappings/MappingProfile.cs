using AutoMapper;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Domain.Entities;

namespace CreditCardStatement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CardHolder, CardHolderDto>().ReverseMap();
            CreateMap<CreditCard, CreditCardDto>()
                .ForMember(dest => dest.CardHolderName, opt => opt.MapFrom(src => src.CardHolder.Name));
            CreateMap<Transaction, TransactionDto>();
        }
    }
}
