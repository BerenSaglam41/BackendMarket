using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(120).WithMessage("Slug en fazla 120 karakter olabilir.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));
    }
}

public class CategoryUpdateDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public CategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug zorunludur.")
            .MaximumLength(120).WithMessage("Slug en fazla 120 karakter olabilir.")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug sadece küçük harf, rakam ve tire içerebilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaTitle));

        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.MetaDescription));
    }
}
