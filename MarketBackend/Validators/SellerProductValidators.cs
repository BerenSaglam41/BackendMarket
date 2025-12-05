using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

/// <summary>
/// Seller'ın ürün önerisi oluşturma validasyonu
/// </summary>
public class SellerProductCreateValidator : AbstractValidator<SellerProductCreateDto>
{
    public SellerProductCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(220).WithMessage("Slug en fazla 220 karakter olabilir.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Açıklama en fazla 5000 karakter olabilir.");

        RuleFor(x => x.ProposedPrice)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.ProposedStock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");

        RuleFor(x => x.ShippingTimeInDays)
            .InclusiveBetween(1, 30).WithMessage("Kargo süresi 1-30 gün arasında olmalıdır.");

        RuleFor(x => x.ImageUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Geçerli bir resim URL'si girilmelidir.");

        RuleFor(x => x.SellerSku)
            .MaximumLength(50).WithMessage("SKU en fazla 50 karakter olabilir.");

        RuleFor(x => x.Barcode)
            .MaximumLength(50).WithMessage("Barkod en fazla 50 karakter olabilir.");

        RuleFor(x => x.SellerNote)
            .MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");

        RuleFor(x => x.SellerCategorySuggestion)
            .MaximumLength(200).WithMessage("Kategori önerisi en fazla 200 karakter olabilir.");
    }
}

/// <summary>
/// Seller'ın ürün önerisi güncelleme validasyonu
/// </summary>
public class SellerProductUpdateValidator : AbstractValidator<SellerProductUpdateDto>
{
    public SellerProductUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(220).WithMessage("Slug en fazla 220 karakter olabilir.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Açıklama en fazla 5000 karakter olabilir.");

        RuleFor(x => x.ProposedPrice)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.ProposedStock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");

        RuleFor(x => x.ShippingTimeInDays)
            .InclusiveBetween(1, 30).WithMessage("Kargo süresi 1-30 gün arasında olmalıdır.");

        RuleFor(x => x.ImageUrl)
            .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Geçerli bir resim URL'si girilmelidir.");
    }
}

/// <summary>
/// Seller'ın mevcut ürünü satışa sunması validasyonu
/// </summary>
public class SellerListingCreateValidator : AbstractValidator<SellerListingCreateDto>
{
    public SellerListingCreateValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Geçerli bir ürün ID'si belirtilmelidir.");

        RuleFor(x => x.OriginalPrice)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(10_000_000).WithMessage("Fiyat 10.000.000'den fazla olamaz.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 99).WithMessage("İndirim oranı 0-99 arasında olmalıdır.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.")
            .LessThanOrEqualTo(100_000).WithMessage("Stok 100.000'den fazla olamaz.");

        RuleFor(x => x.ShippingTimeInDays)
            .InclusiveBetween(1, 30).WithMessage("Kargo süresi 1-30 gün arasında olmalıdır.");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Kargo ücreti negatif olamaz.")
            .LessThanOrEqualTo(1000).WithMessage("Kargo ücreti 1000'den fazla olamaz.");

        RuleFor(x => x.SellerNote)
            .MaximumLength(500).WithMessage("Satıcı notu en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SellerNote));
    }
}

/// <summary>
/// Seller'ın satışını güncelleme validasyonu
/// </summary>
public class SellerListingUpdateValidator : AbstractValidator<SellerListingUpdateDto>
{
    public SellerListingUpdateValidator()
    {
        RuleFor(x => x.OriginalPrice)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(10_000_000).WithMessage("Fiyat 10.000.000'den fazla olamaz.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 99).WithMessage("İndirim oranı 0-99 arasında olmalıdır.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.")
            .LessThanOrEqualTo(100_000).WithMessage("Stok 100.000'den fazla olamaz.");

        RuleFor(x => x.ShippingTimeInDays)
            .InclusiveBetween(1, 30).WithMessage("Kargo süresi 1-30 gün arasında olmalıdır.");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Kargo ücreti negatif olamaz.")
            .LessThanOrEqualTo(1000).WithMessage("Kargo ücreti 1000'den fazla olamaz.");

        RuleFor(x => x.SellerNote)
            .MaximumLength(500).WithMessage("Satıcı notu en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SellerNote));
    }
}
