using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class BrandCreateDtoValidator : AbstractValidator<BrandCreateDto>
{
    public BrandCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı zorunludur.")
            .MaximumLength(100).WithMessage("Marka adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(120).WithMessage("Slug en fazla 120 karakter olabilir.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(200).WithMessage("Website URL'si en fazla 200 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.WebsiteUrl));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Ülke adı en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Country));

        RuleFor(x => x.EstablishedYear)
            .GreaterThan(1800).WithMessage("Kuruluş yılı 1800'den büyük olmalıdır.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Kuruluş yılı gelecekte olamaz.")
            .When(x => x.EstablishedYear.HasValue);

        RuleFor(x => x.SupportEmail)
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(100).WithMessage("Email en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SupportEmail));

        RuleFor(x => x.SupportPhone)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SupportPhone));

        RuleFor(x => x.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));
    }
}

public class BrandUpdateDtoValidator : AbstractValidator<BrandUpdateDto>
{
    public BrandUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı zorunludur.")
            .MaximumLength(100).WithMessage("Marka adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(120).WithMessage("Slug en fazla 120 karakter olabilir.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.LogoUrl));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.WebsiteUrl)
            .MaximumLength(200).WithMessage("Website URL'si en fazla 200 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.WebsiteUrl));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Ülke adı en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Country));

        RuleFor(x => x.EstablishedYear)
            .GreaterThan(1800).WithMessage("Kuruluş yılı 1800'den büyük olmalıdır.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Kuruluş yılı gelecekte olamaz.")
            .When(x => x.EstablishedYear.HasValue);

        RuleFor(x => x.SupportEmail)
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.")
            .MaximumLength(100).WithMessage("Email en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SupportEmail));

        RuleFor(x => x.SupportPhone)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.SupportPhone));

        RuleFor(x => x.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));
    }
}
