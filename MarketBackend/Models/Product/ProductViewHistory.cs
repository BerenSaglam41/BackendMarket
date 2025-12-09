namespace MarketBackend.Models;

public class ProductViewHistory
{
    public int ProductViewHistoryId { get; set; }

    // Kullanıcı (anonim kullanıcılar için null olabilir)
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    // Session ID (giriş yapmamış kullanıcılar için)
    public string? SessionId { get; set; }

    // Görüntülenen ürün
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // İstatistik bilgileri
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public int ViewDuration { get; set; } = 0; // Saniye cinsinden sayfada kalma süresi
    public string? Source { get; set; } // "search", "category", "recommendation", "direct"
}
