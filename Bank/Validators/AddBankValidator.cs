using Bank.Service.DTOs;
using FluentValidation;

namespace Bank.Validators;

public class AddBankValidator : AbstractValidator<AddBank>
{
    public AddBankValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Bank name is required")
            .MaximumLength(200).WithMessage("Bank name cannot exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}