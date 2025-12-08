namespace MarketBackend.Models;

public class ShoppingCart
{
    public int ShoppingCartId { get; set; }              // PK

    // Sepet sahibi (login kullanıcı) - ZORUNLU
    public required string AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    // Guest user için SessionId (opsiyonel - login users için null)
    public string? SessionId { get; set; }

    // Kuponlar
    public string? AppliedCouponCode { get; set; }
    public Coupon? AppliedCoupon { get; set; }
    public decimal DiscountApplied { get; set; }

    // Sepet expire kontrolü
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Para birimi (önerilen)
    public string Currency { get; set; } = "TRY";

    // Güvenlik/analiz için (opsiyonel)
    public string? IpAddress { get; set; }

    // Sepet Ürünleri
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();

    // Toplam hesaplama (DB saklanabilir veya dinamik yapılabilir)
    public decimal TotalAmount { get; set; }
    // 
}