using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

/// <summary>
/// Admin onay DTO'su için validasyon
/// </summary>
public class AdminApproveDtoValidator : AbstractValidator<AdminApproveDto>
{
    public AdminApproveDtoValidator()
    {
        // Name (opsiyonel ama girilirse kontrol et)
        RuleFor(x => x.Name)
            .MinimumLength(2).WithMessage("Ürün adı en az 2 karakter olmalıdır.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        // Slug (opsiyonel ama girilirse kontrol et)
        RuleFor(x => x.Slug)
            .MinimumLength(2).WithMessage("Slug en az 2 karakter olmalıdır.")
            .MaximumLength(200).WithMessage("Slug en fazla 200 karakter olabilir.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.")
            .When(x => !string.IsNullOrEmpty(x.Slug));

        // Description (opsiyonel)
        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Açıklama en fazla 5000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // BrandId (opsiyonel ama girilirse pozitif olmalı)
        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("Marka ID pozitif olmalıdır.")
            .When(x => x.BrandId.HasValue);

        // CategoryId (opsiyonel ama girilirse pozitif olmalı)
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategori ID pozitif olmalıdır.")
            .When(x => x.CategoryId.HasValue);

        // ImageUrl (opsiyonel ama girilirse URL formatında olmalı)
        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl).WithMessage("Geçerli bir resim URL'si giriniz.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        // MetaTitle (opsiyonel)
        RuleFor(x => x.MetaTitle)
            .MaximumLength(100).WithMessage("Meta başlık en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        // MetaDescription (opsiyonel)
        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));

        // AdminNote (opsiyonel)
        RuleFor(x => x.AdminNote)
            .MaximumLength(1000).WithMessage("Admin notu en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.AdminNote));
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var result) 
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Admin red DTO'su için validasyon
/// </summary>
public class AdminRejectDtoValidator : AbstractValidator<AdminRejectDto>
{
    public AdminRejectDtoValidator()
    {
        RuleFor(x => x.AdminNote)
            .NotEmpty().WithMessage("Red sebebi zorunludur.")
            .MinimumLength(10).WithMessage("Red sebebi en az 10 karakter olmalıdır.")
            .MaximumLength(1000).WithMessage("Red sebebi en fazla 1000 karakter olabilir.");
    }
}

/// <summary>
/// Admin güncelleme talebi DTO'su için validasyon
/// </summary>
public class AdminRequestUpdateDtoValidator : AbstractValidator<AdminRequestUpdateDto>
{
    public AdminRequestUpdateDtoValidator()
    {
        RuleFor(x => x.AdminNote)
            .NotEmpty().WithMessage("Güncelleme talebi açıklaması zorunludur.")
            .MinimumLength(10).WithMessage("Güncelleme talebi en az 10 karakter olmalıdır.")
            .MaximumLength(1000).WithMessage("Güncelleme talebi en fazla 1000 karakter olabilir.");
    }
}
