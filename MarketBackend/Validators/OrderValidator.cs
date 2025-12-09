using FluentValidation;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        // En az biri olmalı: ShippingAddressId VEYA ShippingAddress
        RuleFor(x => x)
            .Must(x => x.ShippingAddressId.HasValue || x.ShippingAddress != null)
            .WithMessage("Teslimat adresi gereklidir. Kayıtlı adres ID'si veya yeni adres bilgileri girilmelidir.");
        
        // ShippingAddress girilmişse zorunlu alanları kontrol et
        When(x => x.ShippingAddress != null, () =>
        {
            RuleFor(x => x.ShippingAddress!.ContactName)
                .NotEmpty().WithMessage("İletişim adı gereklidir.")
                .MaximumLength(100).WithMessage("İletişim adı 100 karakterden fazla olamaz.");
            
            RuleFor(x => x.ShippingAddress!.ContactPhone)
                .NotEmpty().WithMessage("İletişim telefonu gereklidir.")
                .Matches(@"^0[0-9]{10}$").WithMessage("Geçerli bir telefon numarası giriniz (05xxxxxxxxx).");
            
            RuleFor(x => x.ShippingAddress!.Country)
                .NotEmpty().WithMessage("Ülke gereklidir.");
            
            RuleFor(x => x.ShippingAddress!.City)
                .NotEmpty().WithMessage("Şehir gereklidir.");
            
            RuleFor(x => x.ShippingAddress!.District)
                .NotEmpty().WithMessage("İlçe gereklidir.");
            
            RuleFor(x => x.ShippingAddress!.FullAddress)
                .NotEmpty().WithMessage("Açık adres gereklidir.")
                .MinimumLength(10).WithMessage("Açık adres en az 10 karakter olmalıdır.");
            
            RuleFor(x => x.ShippingAddress!.PostalCode)
                .NotEmpty().WithMessage("Posta kodu gereklidir.")
                .Matches(@"^[0-9]{5}$").WithMessage("Geçerli bir posta kodu giriniz (5 haneli).");
        });
        
        // ShippingAddressId girilmişse pozitif olmalı
        When(x => x.ShippingAddressId.HasValue, () =>
        {
            RuleFor(x => x.ShippingAddressId!.Value)
                .GreaterThan(0).WithMessage("Geçerli bir teslimat adresi ID'si giriniz.");
        });
        
        // BillingAddress validation (opsiyonel)
        When(x => x.BillingAddress != null, () =>
        {
            RuleFor(x => x.BillingAddress!.ContactName)
                .NotEmpty().WithMessage("Fatura adı gereklidir.");
            
            RuleFor(x => x.BillingAddress!.ContactPhone)
                .NotEmpty().WithMessage("Fatura telefonu gereklidir.")
                .Matches(@"^0[0-9]{10}$").WithMessage("Geçerli bir telefon numarası giriniz.");
            
            RuleFor(x => x.BillingAddress!.FullAddress)
                .NotEmpty().WithMessage("Fatura adresi gereklidir.")
                .MinimumLength(10).WithMessage("Fatura adresi en az 10 karakter olmalıdır.");
        });
        
        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Geçersiz ödeme yöntemi.");
        
        RuleFor(x => x.CouponCode)
            .MaximumLength(50).WithMessage("Kupon kodu 50 karakterden fazla olamaz.")
            .When(x => !string.IsNullOrEmpty(x.CouponCode));
        
        RuleFor(x => x.CustomerNote)
            .MaximumLength(500).WithMessage("Müşteri notu 500 karakterden fazla olamaz.")
            .When(x => !string.IsNullOrEmpty(x.CustomerNote));
    }
}
public class OrderUpdateStatusDtoValidator : AbstractValidator<OrderUpdateStatusDto>
{
    public OrderUpdateStatusDtoValidator()
    {
        RuleFor(x => x.NewStatus)
            .NotEmpty().WithMessage("New status is required.")
            .IsInEnum().WithMessage("Invalid order status.");
        
        When(x => x.NewStatus == OrderStatus.Shipped, () =>
        {
            RuleFor(x => x.TrackingNumber)
                .NotEmpty().WithMessage("Tracking number is required when marking order as shipped.");
        });
    }
}   