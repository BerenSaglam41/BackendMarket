namespace MarketBackend.Models;

public class Brand
{
    public int BrandId { get; set; }

    public string Name { get; set; }                  // Zorunlu
    public string Slug { get; set; }                  // Zorunlu
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public string LogoUrl { get; set; }               // Marka logosu
    public string Description { get; set; }           // Marka açıklaması
    public string WebsiteUrl { get; set; }            // Resmi site

    public bool IsActive { get; set; }                // Aktif/pasif
    public bool IsFeatured { get; set; }              // Öne çıkan marka mı?

    public string MetaTitle { get; set; }             // SEO başlığı
    public string MetaDescription { get; set; }       // SEO açıklaması

    // Ek profesyonel bilgiler
    public string Country { get; set; }               // Ülke
    public int? EstablishedYear { get; set; }         // Kuruluş yılı

    // İsteğe bağlı marka iletişim bilgileri
    public string SupportEmail { get; set; }
    public string SupportPhone { get; set; }

    public int PriorityRank { get; set; }             // Görünüm sırası

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}