namespace MarketBackend.Models;

public class Product
{
    public int ProductId { get; set; }

    // Temel bilgiler
    public required string Name { get; set; }                   // Zorunlu
    public required string Slug { get; set; }                   // URL için zorunlu
    public string? Description { get; set; }                    // Ürün açıklaması

    // Marka ilişkisi
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }

    // Kategori ilişkisi
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Görseller
    public string? ImageUrl { get; set; }
    public string? ImageGalleryJson { get; set; }               // Çoklu resim JSON

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    // Ürünü kim oluşturdu?
    public string? CreatedBySellerId { get; set; }
    public AppUser? CreatedBySeller { get; set; }
    
    // Ürün aktif mi? (Admin tarafından kapatılabilir)
    public bool IsActive { get; set; } = true;
    
    // Seller ilişkileri
    public ICollection<SellerProduct> SellerProducts { get; set; } = new List<SellerProduct>();

    // İstatistikler (opsiyonel ama faydalı)
    public int ReviewCount { get; set; }

    // Zaman
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigasyonlar
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}