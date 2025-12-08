namespace MarketBackend.Models;
using MarketBackend.Models.Enums;

public class Order
{
    public int OrderId { get; set; }                         // Primary Key

    public string OrderNumber { get; set; } = string.Empty;  // "MKT-20251204-1234"

    // Kullanıcı
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;

    // Adresler
    public int ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; } = null!;

    public int BillingAddressId { get; set; }
    public Address BillingAddress { get; set; } = null!;

    // Sipariş Kaynağı
    public string? OrderSource { get; set; }                  // Web, iOS, Android

    public string? CustomerNote { get; set; }                 // "Kapıya bırakın"

    // Fiyatlar
    public decimal Subtotal { get; set; }                     // Ürün toplam
    public decimal TaxAmount { get; set; }                    // KDV
    public decimal DiscountAmount { get; set; }               // Kupon / indirim
    public decimal ShippingCost { get; set; }                 // Kargo
    public decimal TotalAmount { get; set; }                  // Final fiyat

    // Ödeme
    public PaymentMethod PaymentMethod { get; set; }          // CreditCard, EFT, Kapıda Ödeme
    public string? PaymentTransactionId { get; set; }         // Ödeme sağlayıcı ID
    public PaymentStatus PaymentStatus { get; set; }          // Paid, Pending, Failed...

    // Kargo
    public OrderStatus OrderStatus { get; set; }              // AwaitingPayment, Processing...
    public string? ShippingProvider { get; set; }             // MNG, Aras...
    public string? TrackingNumber { get; set; }               // Kargo takip no

    // Zamanlar
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Bir siparişte birden fazla ürün olabilir
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}