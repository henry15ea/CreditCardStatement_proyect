using CreditCardStatement.Application.DTOs;
using FluentValidation;

namespace CreditCardStatement.Application.Validators
{
    public class CreatePaymentValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentValidator()
        {
            RuleFor(x => x.CreditCardId)
                .GreaterThan(0).WithMessage("Credit card ID must be greater than 0");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0")
                .LessThanOrEqualTo(100000).WithMessage("Amount cannot exceed 100,000");
        }
    }
}
