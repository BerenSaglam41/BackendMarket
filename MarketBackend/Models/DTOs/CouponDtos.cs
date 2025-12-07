namespace MarketBackend.Models.DTOs;

// Admin platform Kupon
public class AdminCouponCreateDto
{
    public string Code {get;set;} = string.Empty;
    public decimal DiscountPercentage {get;set;}
    public DateTime ValidFrom {get;set;}
    public DateTime ValidUntil {get;set;}
    public decimal MinimumPurchaseAmount {get;set;}
    public int? MaxUsageCount {get;set;}
}


// seller mağaza Kupon
public class SellerCouponCreateDto
{
    public string Code {get;set;} = string.Empty;
    public decimal DiscountPercentage {get;set;}
    public DateTime ValidFrom {get;set;}
    public DateTime ValidUntil {get;set;}
    public decimal MinimumPurchaseAmount {get;set;}
    public int? MaxUsageCount {get;set;}
}
// Kupon güncelleme DTO - hem admin hem seller için ortak
// ⚠️ UYARI: Sadece aşağıdaki alanlar güncellenebilir!
// ❌ Code (Kupon kodu) DEĞİŞTİRİLEMEZ
// ❌ CouponId DEĞİŞTİRİLEMEZ
// ❌ CurrentUsageCount DEĞİŞTİRİLEMEZ (otomatik güncellenir)
public class CouponUpdateDto
{
    public decimal? DiscountPercentage {get;set;}
    public DateTime? ValidFrom {get;set;}
    public DateTime? ValidUntil {get;set;}
    public decimal? MinimumPurchaseAmount {get;set;}
    public int? MaxUsageCount {get;set;}
    public bool? IsActive {get;set;}
}
// Kupon detay response
public class CouponResponseDto
{
    public int CouponId { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public decimal MinimumPurchaseAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public int CurrentUsageCount { get; set; }
    public string CouponType { get; set; } = string.Empty; // "Platform" veya "Seller"
    public string? SellerStoreName { get; set; } // Seller kuponu ise mağaza adı
    public DateTime CreatedAt { get; set; }
}
// Sepete kupon uygulama
public class ApplyCouponDto
{
    public string Code {get;set;} = string.Empty;
}