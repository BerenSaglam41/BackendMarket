using FluentValidation;
using MarketBackend.Models.DTOs;
namespace MarketBackend.Validators;
public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Ürün ID'si zorunludur.")
            .GreaterThan(0).WithMessage("Geçerli bir Ürün ID'si girilmelidir.");
        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Puan zorunludur.")
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");
        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));
        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}
public class ReviewUpdateDtoValidator : AbstractValidator<ReviewUpdateDto>
{
    public ReviewUpdateDtoValidator()
    {
        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Puan belirtilmelidir.")
            .InclusiveBetween(1, 5).WithMessage("Puan 1-5 arasında olmalıdır.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Comment));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));
    }
}

public class ReviewAdminReplyDtoValidator : AbstractValidator<ReviewAdminReplyDto>
{
    public ReviewAdminReplyDtoValidator()
    {
        RuleFor(x => x.AdminReply)
            .NotEmpty().WithMessage("Admin yanıtı boş olamaz.")
            .MaximumLength(500).WithMessage("Admin yanıtı en fazla 500 karakter olabilir.");
    }
}