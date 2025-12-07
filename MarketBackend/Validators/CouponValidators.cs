using System.Data;
using FluentValidation;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;

// Admin Kupon validasyon
public class AdminCouponCreateDtoValidator : AbstractValidator<AdminCouponCreateDto>
{
    public AdminCouponCreateDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kupon kodu zorunludur.")
            .MaximumLength(50).WithMessage("Kupon kodu en fazla 50 karakter olabilir.")
            .Matches("^[A-Z0-9]+$").WithMessage("Kupon kodu sadece büyük harf ve rakamlardan oluşabilir.");
        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0).WithMessage("İndirim yüzdesi 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("İndirim yüzdesi en fazla 100 olabilir.");
        RuleFor(x => x.ValidFrom)
            .LessThan(x => x.ValidUntil).WithMessage("Geçerlilik başlangıcı, geçerlilik bitişinden önce olmalıdır.");
        RuleFor(x => x.ValidUntil)
            .NotEmpty().WithMessage("Geçerlilik bitiş tarihi zorunludur.")
            .GreaterThan(x => x.ValidFrom).WithMessage("Geçerlilik bitişi, geçerlilik başlangıcından sonra olmalıdır.");
        RuleFor(x => x.MinimumPurchaseAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum satın alma tutarı negatif olamaz.");
        RuleFor(x => x.MaxUsageCount)
            .GreaterThan(0).When(x => x.MaxUsageCount.HasValue)
            .WithMessage("Maksimum kullanım sayısı 0'dan büyük olmalıdır.");
    }
}

public class SellerCouponCreateDtoValidator : AbstractValidator<SellerCouponCreateDto>
{
    public SellerCouponCreateDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kupon kodu zorunludur.")
            .MaximumLength(50).WithMessage("Kupon kodu en fazla 50 karakter olabilir.")
            .Matches("^[A-Z0-9]+$").WithMessage("Kupon kodu sadece büyük harf ve rakamlardan oluşabilir.");
        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0).WithMessage("İndirim yüzdesi 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).WithMessage("İndirim yüzdesi en fazla 100 olabilir.");
        RuleFor(x => x.ValidFrom)
            .LessThan(x => x.ValidUntil).WithMessage("Geçerlilik başlangıcı, geçerlilik bitişinden önce olmalıdır.");
        RuleFor(x => x.ValidUntil)
            .NotEmpty().WithMessage("Geçerlilik bitiş tarihi zorunludur.")
            .GreaterThan(x => x.ValidFrom).WithMessage("Geçerlilik bitişi, geçerlilik başlangıcından sonra olmalıdır.");
        RuleFor(x => x.MinimumPurchaseAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum satın alma tutarı negatif olamaz.");
        RuleFor(x => x.MaxUsageCount)
            .GreaterThan(0).When(x => x.MaxUsageCount.HasValue)
            .WithMessage("Maksimum kullanım sayısı 0'dan büyük olmalıdır.");
    }
}
// Kupon Update
// ⚠️ Sadece nullable alanları günceller - Code güncellenemez!
public class CouponUpdateDtoValidator : AbstractValidator<CouponUpdateDto>
{
    public CouponUpdateDtoValidator()
    {
        // En az bir alan dolu olmalı
        RuleFor(x => x)
            .Must(x => x.DiscountPercentage.HasValue || 
                      x.ValidFrom.HasValue || 
                      x.ValidUntil.HasValue || 
                      x.MinimumPurchaseAmount.HasValue || 
                      x.MaxUsageCount.HasValue || 
                      x.IsActive.HasValue)
            .WithMessage("Güncellemek için en az bir alan gönderilmelidir.");
            
        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0).When(x => x.DiscountPercentage.HasValue)
            .WithMessage("İndirim yüzdesi 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(100).When(x => x.DiscountPercentage.HasValue)
            .WithMessage("İndirim yüzdesi en fazla 100 olabilir.");
        RuleFor(x => x.ValidFrom)
            .LessThan(x => x.ValidUntil).When(x => x.ValidFrom.HasValue && x.ValidUntil.HasValue)
            .WithMessage("Geçerlilik başlangıcı, geçerlilik bitişinden önce olmalıdır.");
        RuleFor(x => x.ValidUntil)
            .GreaterThan(x => x.ValidFrom).When(x => x.ValidFrom.HasValue && x.ValidUntil.HasValue)
            .WithMessage("Geçerlilik bitişi, geçerlilik başlangıcından sonra olmalıdır.");
        RuleFor(x => x.MinimumPurchaseAmount)
            .GreaterThanOrEqualTo(0).When(x => x.MinimumPurchaseAmount.HasValue)
            .WithMessage("Minimum satın alma tutarı negatif olamaz.");
        RuleFor(x => x.MaxUsageCount)
            .GreaterThan(0).When(x => x.MaxUsageCount.HasValue)
            .WithMessage("Maksimum kullanım sayısı 0'dan büyük olmalıdır.");
    }
}

// Sepete kupon uygulama validasyonu
public class ApplyCouponDtoValidator : AbstractValidator<ApplyCouponDto>
{
    public ApplyCouponDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kupon kodu boş olamaz")
            .MaximumLength(50).WithMessage("Kupon kodu en fazla 50 karakter olabilir");
    }
}