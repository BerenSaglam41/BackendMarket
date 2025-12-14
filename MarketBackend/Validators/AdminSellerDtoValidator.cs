using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class AdminSellerDtoValidator : AbstractValidator<AdminSellerDto>
{
    public AdminSellerDtoValidator()
    {
        RuleFor(x => x.BanReason)
            .NotEmpty().WithMessage("Ban reason is required.")
            .MaximumLength(500).WithMessage("Ban reason cannot exceed 500 characters.");
    }
}