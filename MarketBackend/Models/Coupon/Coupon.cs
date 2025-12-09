namespace MarketBackend.Models;

public class Coupon
{
    public int CouponId { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public decimal DiscountPercentage { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime ValidFrom { get; set; }
    
    public DateTime ValidUntil { get; set; }
    
    public decimal MinimumPurchaseAmount { get; set; } = 0;
    
    public int? MaxUsageCount { get; set; }
    
    public int CurrentUsageCount { get; set; } = 0;
    
    // Admin platform kuponu için
    public string? CreatedByAdminId { get; set; }
    public AppUser? CreatedByAdmin { get; set; }
    
    // Seller mağaza kuponu için
    public string? CreatedBySellerId { get; set; }
    public AppUser? CreatedBySeller { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}