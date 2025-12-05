using MarketBackend.Models;

namespace MarketBackend.Models;

public class SellerProduct
{
    public int SellerProductId { get; set; }

    // Satıcı
    public required string SellerId { get; set; }
    public AppUser Seller { get; set; } = null!;

    // Ürün
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Satıcıya özel fiyat ve stok
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // Kargo bilgileri
    public int ShippingTimeInDays { get; set; } = 2;    // Teslim süresi (örn. 1-3 gün)
    public decimal ShippingCost { get; set; } = 0;      // Satıcının kargo ücreti

    // Satıcı açıklaması (ürün açıklaması değil!)
    public string? SellerNote { get; set; }

    // Kontrol
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}