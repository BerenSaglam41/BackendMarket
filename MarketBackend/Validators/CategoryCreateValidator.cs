using FluentValidation;
using MarketBackend.Models.DTOs;

public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
{
    public CategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı zorunludur.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Kategori slug'ı zorunludur.")
            .MaximumLength(150).WithMessage("Kategori slug'ı en fazla 150 karakter olabilir.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
        RuleFor(x => x.ImageUrl)
            .MaximumLength(250).WithMessage("Resim URL'si en fazla 250 karakter olabilir.");
        RuleFor(x => x.MetaTitle)
            .MaximumLength(150).WithMessage("Meta başlık en fazla 150 karakter olabilir.");
        RuleFor(x => x.MetaDescription)
            .MaximumLength(300).WithMessage("Meta açıklama en fazla 300 karakter olabilir.");
    }
}