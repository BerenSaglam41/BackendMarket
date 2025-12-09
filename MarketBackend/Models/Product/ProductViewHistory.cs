namespace MarketBackend.Models;

public class ProductViewHistory
{
    public int ProductViewHistoryId { get; set; }

    // Kullanıcı (misafir kullanıcılar için null)
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    // Görüntülenen listing
    public int ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    // View istatistikleri
    public int ViewCount { get; set; } = 1;
    public DateTime FirstViewedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastViewedAt { get; set; } = DateTime.UtcNow;

    // Ek bilgiler
    public string? Source { get; set; } // "search", "category", "home", "related", "direct"
    public string? DeviceType { get; set; } // "mobile", "tablet", "desktop"
}
