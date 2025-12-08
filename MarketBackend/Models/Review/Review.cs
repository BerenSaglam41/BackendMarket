namespace MarketBackend.Models;

public class Review
{
    public int ReviewId { get; set; }                  // PK

    // Ürün ilişkisi
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;      // Navigation

    // Kullanıcı ilişkisi
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;         // Navigation

    // Rating & yorum
    public int Rating { get; set; }                    // 1–5
    public string? Comment { get; set; }               // Metin (nullable - kısa yorum olabilir)

    // Moderasyon
    public bool IsApproved { get; set; }               
    public bool IsVerifiedBuyer { get; set; }          
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }

    // Medya
    public string? ImageUrl { get; set; }              // Nullable

    // Admin cevapları
    public string? AdminReply { get; set; }            // Nullable
    public DateTime? RepliedAt { get; set; }

    // Zaman
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}