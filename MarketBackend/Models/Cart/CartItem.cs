namespace MarketBackend.Models;

public class CartItem
{
    public int CartItemId { get; set; }                        // PK

    // Cart ilişkisi
    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; } = null!;

    // Ürün ilişkisi (genel ürün bilgisi için)
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // ⭐ Satıcı ilişkisi - Hangi satıcıdan alınıyor?
    public int SellerProductId { get; set; }
    public SellerProduct SellerProduct { get; set; } = null!;

    public int Quantity { get; set; }                          // Adet

    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Kupon
    public string? AppliedCouponCode { get; set; }             // Ürün özel kupon
    public decimal DiscountApplied { get; set; }               // Bu ürüne uygulanan indirim

    // Stok
    public bool IsOutOfStock { get; set; }

    // Varyant
    public string? SelectedVariant { get; set; }               // JSON veya string

    // Checkout seçim kontrolü
    public bool IsSelectedForCheckout { get; set; } = true;

    // Fiyatlar (sepete ekleme anında)
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }                    // UnitPrice * Quantity - discount
}