using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class CartAddDtoValidator : AbstractValidator<CartAddDto>
{
    public CartAddDtoValidator()
    {
        RuleFor(x => x.SellerProductId)
            .GreaterThan(0).WithMessage("Geçerli bir satıcı ürün ID'si belirtilmelidir.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miktar en az 1 olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("Tek seferde en fazla 100 adet ekleyebilirsiniz.");

        RuleFor(x => x.SelectedVariant)
            .MaximumLength(500).WithMessage("Varyant bilgisi en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SelectedVariant));
    }
}
