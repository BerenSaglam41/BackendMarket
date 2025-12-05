using FluentValidation;
using MarketBackend.Models.DTOs;

public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(150).WithMessage("Ürün adı en fazla 150 karakter olabilir.");

        RuleFor(p => p.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(180).WithMessage("Slug en fazla 180 karakter olabilir.");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Açıklama en fazla 2000 karakter olabilir.");

        RuleFor(p => p.BrandId)
            .GreaterThan(0).When(p => p.BrandId.HasValue)
            .WithMessage("Geçerli bir marka ID belirtilmelidir.");

        RuleFor(p => p.OriginalPrice)
            .GreaterThan(0).WithMessage("Orijinal fiyat sıfırdan büyük olmalıdır.");

        RuleFor(p => p.DiscountPercentage)
            .InclusiveBetween(0, 100).WithMessage("İndirim oranı 0–100 arasında olmalıdır.");

        RuleFor(p => p.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

        RuleFor(p => p.ImageUrl)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        RuleFor(p => p.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.");

        RuleFor(p => p.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.");
    }
}