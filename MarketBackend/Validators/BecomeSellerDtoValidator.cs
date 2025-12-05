using FluentValidation;
using MarketBackend.Models.DTOs;

public class BecomeSellerDtoValidator : AbstractValidator<BecomeSellerDto>
{
    public BecomeSellerDtoValidator()
    {
        // Mağaza bilgileri
        RuleFor(x => x.StoreName)
            .NotEmpty().WithMessage("Mağaza adı zorunludur.")
            .MinimumLength(3).WithMessage("Mağaza adı en az 3 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Mağaza adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.StoreSlug)
            .NotEmpty().WithMessage("Mağaza URL'si zorunludur.")
            .MinimumLength(3).WithMessage("Mağaza URL'si en az 3 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Mağaza URL'si en fazla 50 karakter olabilir.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Mağaza URL'si sadece küçük harf, rakam ve tire içerebilir (örn: ahsap-atolyesi).");

        RuleFor(x => x.StorePhone)
            .Matches(@"^(\+90|0)?[0-9]{10}$")
            .When(x => !string.IsNullOrEmpty(x.StorePhone))
            .WithMessage("Geçerli bir telefon numarası giriniz.");

        RuleFor(x => x.StoreDescription)
            .MaximumLength(500).WithMessage("Mağaza açıklaması en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.StoreDescription));

        // Sözleşme
        RuleFor(x => x.AcceptSellerTerms)
            .Equal(true).WithMessage("Satıcı sözleşmesini kabul etmelisiniz.");
    }
}
