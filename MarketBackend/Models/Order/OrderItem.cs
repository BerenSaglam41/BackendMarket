namespace MarketBackend.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }              // PK

    // Sipariş ilişkisi
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // Ürün ilişkisi
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // ⭐ Satıcı ilişkisi - Hangi satıcıdan alındı?
    public int SellerProductId { get; set; }
    public SellerProduct SellerProduct { get; set; } = null!;

    // Ürün bilgisi (sipariş anındaki snapshot)
    public string ProductName { get; set; } = string.Empty;    // Ürün adı o an nasılsa öyle kalır
    
    // ⭐ Satıcı bilgisi (sipariş anındaki snapshot)
    public string SellerId { get; set; } = string.Empty;       // Satıcı ID
    public string SellerStoreName { get; set; } = string.Empty; // Sipariş anındaki mağaza adı

    // Miktar & Fiyat
    public int Quantity { get; set; }                 // Adet
    public decimal UnitPrice { get; set; }            // O anki birim fiyat
    public decimal DiscountApplied { get; set; }      // Bu satıra özel indirim (yoksa 0)

    // Vergi
    public decimal TaxRate { get; set; }              // Örn: 0.08m, 0.20m

    // Toplam fiyat (indirim ve vergi dahil modellerine göre)
    public decimal TotalPrice { get; set; }           // Quantity * UnitPrice - indirim + vergi (sipariş anında hesaplanır)

    // Kargo bölünmüşse, bu satıra özel takip numarası
    public string? TrackingNumber { get; set; }       // Opsiyonel
}