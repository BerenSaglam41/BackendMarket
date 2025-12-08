using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

/// <summary>
/// Sepete ürün ekleme validasyonu
/// </summary>
public class CartAddDtoValidator : AbstractValidator<CartAddDto>
{
    public CartAddDtoValidator()
    {
        RuleFor(x => x.ListingId)
            .NotEmpty().WithMessage("Satış ilanı ID'si belirtilmelidir.")
            .GreaterThan(0).WithMessage("Geçerli bir satış ilanı ID'si belirtilmelidir.");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Miktar belirtilmelidir.")
            .GreaterThan(0).WithMessage("Miktar en az 1 olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Tek seferde en fazla 100 adet ekleyebilirsiniz.");

        RuleFor(x => x.SelectedVariant)
            .MaximumLength(500).WithMessage("Varyant bilgisi en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SelectedVariant));
    }
}

/// <summary>
/// Sepet ürün miktarı güncelleme validasyonu
/// </summary>
public class CartUpdateQuantityDtoValidator : AbstractValidator<CartUpdateQuantityDto>
{
    public CartUpdateQuantityDtoValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Miktar belirtilmelidir.")
            .GreaterThan(0).WithMessage("Miktar en az 1 olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Miktar en fazla 100 olabilir.");
    }
}
