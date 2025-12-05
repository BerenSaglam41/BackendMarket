using System.Data;
using FluentValidation;
using MarketBackend.Models.DTOs;

public class BrandCreateDtoValidator : AbstractValidator<BrandCreateDto>
{
    public BrandCreateDtoValidator()
    {
        RuleFor(b => b.Name)
            .NotEmpty().WithMessage("Marka adı boş olamaz.")
            .MaximumLength(100).WithMessage("Marka adı en fazla 100 karakter olabilir.");   
        RuleFor(b => b.Slug)
            .NotEmpty().WithMessage("Slug boş olamaz.")
            .MaximumLength(150).WithMessage("Slug en fazla 150 karakter olabilir.");
        RuleFor(b => b.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
        RuleFor(b => b.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.");
        RuleFor(b => b.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.");
        RuleFor(b => b.SupportEmail)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.SupportEmail))
            .WithMessage("Geçersiz destek emaili.");
        RuleFor(b => b.WebsiteUrl)
            .Must(u => Uri.IsWellFormedUriString(u, UriKind.Absolute))
            .When(u => !string.IsNullOrEmpty(u.WebsiteUrl))
            .WithMessage("Website URL formatı geçerli değil.");
    }
}