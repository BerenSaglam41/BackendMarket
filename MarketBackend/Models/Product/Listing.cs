using MarketBackend.Models;

namespace MarketBackend.Models;

public class Listing
{
    public int ListingId { get; set; }

    // Satıcı
    public required string SellerId { get; set; }
    public AppUser Seller { get; set; } = null!;

    // Ürün
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Satıcıya özel fiyat bilgileri
    public decimal OriginalPrice { get; set; }          // İndirimsiz fiyat
    public decimal DiscountPercentage { get; set; }     // % İndirim oranı (0–100)
     
    // Otomatik hesaplanan fiyat
    public decimal UnitPrice 
    {
        get => OriginalPrice - (OriginalPrice * (DiscountPercentage / 100m));
    }

    // Satıcıya özel stok
    public int Stock { get; set; }

    // Kargo bilgileri
    public int ShippingTimeInDays { get; set; } = 2;    // Teslim süresi (örn. 1-3 gün)
    public decimal ShippingCost { get; set; } = 0;      // Satıcının kargo ücreti

    // Satıcı açıklaması (ürün açıklaması değil!)
    public string? SellerNote { get; set; }

    // SEO ve URL için benzersiz slug (product-slug + store-slug)
    public string Slug { get; set; } = string.Empty;

    // Kontrol
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}