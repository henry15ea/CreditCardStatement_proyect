using CreditCardStatement.Application.Commands;
using CreditCardStatement.Application.DTOs;
using CreditCardStatement.Application.Interfaces;
using CreditCardStatement.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreditCardStatement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreditCardController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStatementPdfService _pdfService;

        public CreditCardController(IMediator mediator, IStatementPdfService pdfService)
        {
            _mediator = mediator;
            _pdfService = pdfService;
        }

        [HttpGet("statement/{creditCardId}")]
        public async Task<ActionResult<StatementDto>> GetStatement(int creditCardId, [FromQuery] int month, [FromQuery] int year)
        {
            var query = new GetStatementQuery
            {
                CreditCardId = creditCardId,
                Month = month,
                Year = year
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("statement/{creditCardId}/pdf")]
        public async Task<IActionResult> GetStatementPdf(int creditCardId, [FromQuery] int month, [FromQuery] int year)
        {
            var query = new GetStatementQuery
            {
                CreditCardId = creditCardId,
                Month = month,
                Year = year
            };
            var result = await _mediator.Send(query);
            var pdf = _pdfService.GenerateStatementPdf(result, month, year);
            return File(pdf, "application/pdf", $"EstadoCuenta_{creditCardId}_{month}_{year}.pdf");
        }

        [HttpGet("transactions/{creditCardId}")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions(int creditCardId, [FromQuery] int month, [FromQuery] int year)
        {
            var query = new GetTransactionsQuery
            {
                CreditCardId = creditCardId,
                Month = month,
                Year = year
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("cards/{cardHolderId}")]
        public async Task<ActionResult<List<CreditCardDto>>> GetCreditCards(int cardHolderId)
        {
            var query = new GetCreditCardsQuery
            {
                CardHolderId = cardHolderId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<ActionResult<TransactionDto>> CreatePurchase([FromBody] CreatePurchaseDto purchase)
        {
            var command = new CreatePurchaseCommand
            {
                Purchase = purchase
            };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransactions), new { creditCardId = result.CreditCardId, month = result.Date.Month, year = result.Date.Year }, result);
        }

        [HttpPost("payment")]
        public async Task<ActionResult<TransactionDto>> MakePayment([FromBody] CreatePaymentDto payment)
        {
            var command = new MakePaymentCommand
            {
                Payment = payment
            };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransactions), new { creditCardId = result.CreditCardId, month = result.Date.Month, year = result.Date.Year }, result);
        }
    }
}
