using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

/// <summary>
/// Admin ürün oluşturma validasyonu - Sadece ürün tanımı (fiyat/stok SellerProduct'ta)
/// </summary>
public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(150).WithMessage("Ürün adı en fazla 150 karakter olabilir.");

        RuleFor(p => p.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(180).WithMessage("Slug en fazla 180 karakter olabilir.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(p => p.Description)
            .MaximumLength(2000).WithMessage("Açıklama en fazla 2000 karakter olabilir.");

        RuleFor(p => p.BrandId)
            .GreaterThan(0).When(p => p.BrandId.HasValue)
            .WithMessage("Geçerli bir marka ID belirtilmelidir.");

        RuleFor(p => p.CategoryId)
            .GreaterThan(0).When(p => p.CategoryId.HasValue)
            .WithMessage("Geçerli bir kategori ID belirtilmelidir.");

        RuleFor(p => p.ImageUrl)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("Geçerli bir resim URL'si girilmelidir.");

        RuleFor(p => p.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.");

        RuleFor(p => p.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.");
    }
}