using MarketBackend.Models;

namespace MarketBackend.Models;

public class ProductPending
{
    public int ProductPendingId { get; set; }

    public required string SellerId { get; set; }
    public AppUser Seller { get; set; } = null!;

    // Ürün temel bilgileri
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Fotoğraflar
    public string? ImageUrl { get; set; }
    public string? ImageGalleryJson { get; set; } // Çoklu resim

    // Seller özel alanlar
    public string? SellerSku { get; set; }
    public string? Barcode { get; set; }
    public string? SellerCategorySuggestion { get; set; }
    public string? AttributesJson { get; set; }   // Teknik özellikler
    public string? SellerNote { get; set; }

    // Fiyat & Stok
    public decimal ProposedPrice { get; set; }
    public int ProposedStock { get; set; }

    // Kargo bilgileri
    public int ShippingTimeInDays { get; set; } = 2;

    // Admin işlemleri
    public PendingStatus Status { get; set; } = PendingStatus.Waiting;
    public string? AdminNote { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}