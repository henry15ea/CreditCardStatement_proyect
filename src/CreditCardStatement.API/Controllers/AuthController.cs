using CreditCardStatement.Application.Commands;
using CreditCardStatement.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardStatement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto login)
        {
            var command = new LoginCommand
            {
                Login = login
            };
            var result = await _mediator.Send(command);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
