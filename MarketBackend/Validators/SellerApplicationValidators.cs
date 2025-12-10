using FluentValidation;
using MarketBackend.Models.DTOs;

public class SellerApplicationRequestValidator : AbstractValidator<SellerApplicationCreateDto>
{
    public SellerApplicationRequestValidator()
    {
        RuleFor(x => x.StoreName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.StoreSlug)
            .NotEmpty()
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.StorePhone)
            .MaximumLength(20)
            .When(x => x.StorePhone != null);

        RuleFor(x => x.StoreDescription)
            .MaximumLength(500);

        RuleFor(x => x.StoreLogoUrl)
            .MaximumLength(300);
    }
}
public class SellerApplicationReviewDtoValidator : AbstractValidator<SellerApplicationReviewDto>
{
    public SellerApplicationReviewDtoValidator()
    {
        RuleFor(x => x.AdminNote)
            .MaximumLength(500)
            .WithMessage("Admin notu en fazla 500 karakter olabilir.");

    }
}