using MarketBackend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MarketBackend.Models.Payment;

/// <summary>
/// Ödeme işlemi - Sipariş oluşturulmadan önce ödeme alınır
/// </summary>
public class Payment
{
    [Key]
    public int PaymentId { get; set; }
    
    // İlişkiler
    public string AppUserId { get; set; } = string.Empty;
    public AppUser AppUser { get; set; } = null!;
    
    public int? OrderId { get; set; }  // Ödeme başarılı olduktan sonra sipariş oluşturulur
    public Order? Order { get; set; }
    
    // Ödeme Bilgileri
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    
    public decimal Amount { get; set; }  // Toplam ödeme tutarı
    public string Currency { get; set; } = "TRY";
    
    // Ödeme Gateway Bilgileri
    public string? PaymentGateway { get; set; }  // "Stripe", "PayPal", "iyzico"
    public string? TransactionId { get; set; }    // Gateway'den dönen transaction ID
    public string? PaymentToken { get; set; }     // Gateway token (güvenlik için)
    
    // Sepet Snapshot (Ödeme anındaki sepet durumu)
    public string CartSnapshotJson { get; set; } = string.Empty;  // JSON olarak sakla
    
    // Adres Bilgileri (Ödeme anındaki adresler)
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    
    // Ödeme Detayları
    public string? ErrorMessage { get; set; }     // Hata mesajı (başarısız ödemelerde)
    public string? CustomerNote { get; set; }
    public string? CouponCode { get; set; }
    
    // Tarihler
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }    // Ödeme tamamlanma zamanı
    public DateTime? FailedAt { get; set; }       // Başarısız olma zamanı
    public DateTime? RefundedAt { get; set; }     // İade zamanı
    
    // Metadata
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
