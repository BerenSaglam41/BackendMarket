namespace MarketBackend.Models.DTOs;

/// <summary>
/// Wishlist'e ürün eklerken
/// </summary>
public class WishlistAddDto
{
    public int ProductId { get; set; }
    public string? SelectedVariant { get; set; }        // Opsiyonel varyant (JSON)
}

/// <summary>
/// Wishlist item response (ürün detaylarıyla birlikte)
/// </summary>
public class WishlistItemResponseDto
{
    // WishlistItem'den gelen alanlar
    public int WishlistItemId { get; set; }             // Model: WishlistItemId ✅
    public DateTime DateAdded { get; set; }             // Model: DateAdded ✅
    public decimal? PriceAtAddition { get; set; }       // Model: PriceAtAddition ✅ (nullable - satıcı yoksa null)
    public string? SelectedVariant { get; set; }        // Model: SelectedVariant ✅
    
    // Product'tan join edilen bilgiler (Model'de yok, controller'da doldurulacak)
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductSlug { get; set; }
    public string? ProductImage { get; set; }
    public string? BrandName { get; set; }
    
    // Hesaplanan bilgiler (Controller'da hesaplanacak)
    public decimal? CurrentMinPrice { get; set; }       // Şu anki minimum fiyat
    public bool PriceChanged { get; set; }              // Fiyat değişti mi?
    public bool IsAvailable { get; set; }               // Stokta mı?
    public int AvailableSellerCount { get; set; }       // Kaç satıcıda var?
}