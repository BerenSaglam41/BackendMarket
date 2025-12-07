namespace MarketBackend.Models;

public class WishlistItem
{
    public int WishlistItemId { get; set; }

    // Kullanıcı
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    // Ürün
    public int ProductId { get; set; }
    public Product Product { get; set; }

    // Favori eklendiği zaman
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    // Eklenen anda ürünün fiyatı
    public decimal PriceAtAddition { get; set; }

    // Kullanıcı hangi varyantı beğendi?
    public string? SelectedVariant { get; set; }        // opsiyonel / JSON
}
