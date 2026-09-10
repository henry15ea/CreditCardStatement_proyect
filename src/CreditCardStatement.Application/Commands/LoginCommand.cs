using CreditCardStatement.Application.DTOs;
using MediatR;

namespace CreditCardStatement.Application.Commands
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public LoginDto Login { get; set; } = null!;
    }
}
