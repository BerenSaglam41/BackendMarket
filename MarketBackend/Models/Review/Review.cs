namespace MarketBackend.Models;

public class Review
{
    public int ReviewId { get; set; }                  // PK

    // Ürün ilişkisi
    public int ProductId { get; set; }
    public Product Product { get; set; }               // Navigation

    // Kullanıcı ilişkisi
    public string UserId { get; set; }
    public AppUser User { get; set; }                  // Navigation

    // Rating & yorum
    public int Rating { get; set; }                    // 1–5
    public string Comment { get; set; }                // Metin

    // Moderasyon
    public bool IsApproved { get; set; }               
    public bool IsVerifiedBuyer { get; set; }          
    public bool IsReported { get; set; }
    public int ReportCount { get; set; }

    // Medya
    public string ImageUrl { get; set; }               

    // Admin cevapları
    public string AdminReply { get; set; }
    public DateTime? RepliedAt { get; set; }

    // Zaman
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}