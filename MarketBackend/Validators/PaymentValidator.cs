using FluentValidation;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Validators;

public class PaymentInitiateDtoValidator : AbstractValidator<PaymentInitiateDto>
{
    public PaymentInitiateDtoValidator()
    {
        // Teslimat adresi zorunlu
        RuleFor(x => x)
            .Must(x => x.ShippingAddressId.HasValue || x.ShippingAddress != null)
            .WithMessage("Teslimat adresi gereklidir.");

        // Inline teslimat adresi validasyonu
        When(x => x.ShippingAddress != null, () =>
        {
            RuleFor(x => x.ShippingAddress!.ContactName)
                .NotEmpty()
                .WithMessage("İletişim adı zorunludur.")
                .MaximumLength(100)
                .WithMessage("İletişim adı en fazla 100 karakter olabilir.");
            RuleFor(x => x.ShippingAddress!.ContactPhone)
                .Matches(@"^0[0-9]{10}$")
                .WithMessage("Geçerli bir telefon numarası giriniz (05xxxxxxxxx formatında 11 haneli).");
            RuleFor(x => x.ShippingAddress!.Country)
                .NotEmpty()
                .WithMessage("Ülke zorunludur.");
            RuleFor(x => x.ShippingAddress!.City)
                .NotEmpty()
                .WithMessage("Şehir zorunludur.");
            RuleFor(x => x.ShippingAddress!.District)
                .NotEmpty()
                .WithMessage("İlçe zorunludur.");
            RuleFor(x => x.ShippingAddress!.Neighborhood)
                .NotEmpty()
                .WithMessage("Mahalle zorunludur.");
            RuleFor(x => x.ShippingAddress!.PostalCode)
                .Matches(@"^[0-9]{5}$")
                .WithMessage("Geçerli bir posta kodu giriniz (5 haneli).");

            RuleFor(x => x.ShippingAddress!.FullAddress)
                .MinimumLength(10)
                .WithMessage("Tam adres en az 10 karakter olmalıdır.")
                .MaximumLength(300)
                .WithMessage("Tam adres en fazla 300 karakter olabilir.")
                .NotEmpty()
                .WithMessage("Açık adres zorunludur.");
            RuleFor(x => x.ShippingAddress!.AddressType)
                .IsInEnum()
                .WithMessage("Geçerli bir adres türü seçiniz.");
        });

        // Inline fatura adresi validasyonu
        When(x => x.BillingAddress != null, () =>
        {
            RuleFor(x => x.BillingAddress!.ContactPhone)
                .Matches(@"^0[0-9]{10}$")
                .WithMessage("Geçerli bir telefon numarası giriniz (05xxxxxxxxx formatında 11 haneli).");

            RuleFor(x => x.BillingAddress!.PostalCode)
                .Matches(@"^[0-9]{5}$")
                .WithMessage("Geçerli bir posta kodu giriniz (5 haneli).");

            RuleFor(x => x.BillingAddress!.FullAddress)
                .MinimumLength(10)
                .WithMessage("Tam adres en az 10 karakter olmalıdır.");
        });

        // Ödeme yöntemi zorunlu
        RuleFor(x => x.PaymentMethod)
            .NotNull()
            .WithMessage("Ödeme yöntemi zorunludur.")
            .IsInEnum()
            .WithMessage("Geçerli bir ödeme yöntemi seçiniz. Geçerli değerler: CreditCard (0), BankTransfer (1), CashOnDelivery (2), Wallet (3), CardStored (4)");
    }
}

public class PaymentFailDtoValidator : AbstractValidator<PaymentFailDto>
{
    public PaymentFailDtoValidator()
    {
        RuleFor(x => x.ErrorMessage)
            .NotEmpty()
            .WithMessage("Hata mesajı zorunludur.")
            .MaximumLength(500)
            .WithMessage("Hata mesajı en fazla 500 karakter olabilir.");
    }
}

public class PaymentConfirmDtoValidator : AbstractValidator<PaymentConfirmDto>
{
    public PaymentConfirmDtoValidator()
    {
        RuleFor(x => x.PaymentId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir ödeme ID'si gereklidir.");

        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID zorunludur.");
    }
}
