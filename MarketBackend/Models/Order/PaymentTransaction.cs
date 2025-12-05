using MarketBackend.Models.Enums;

namespace MarketBackend.Models;

public class PaymentTransaction
{
    public int PaymentTransactionId { get; set; }   // ✔ EF otomatik PK kabul eder
    // Sipariş ilişkisi
    public int OrderId { get; set; }
    public Order Order { get; set; }

    // Ödeme sağlayıcı & işlem kimliği
    public string Last4Digits { get; set; }
    public string ProviderTransactionId { get; set; }          // Ödeme sağlayıcı referansı
    public string PaymentProvider { get; set; }                // Iyzico, Stripe, PayPal...

    // Ödeme tutarları
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";

    // Durum
    public TransactionStatus TransactionStatus { get; set; }              // Success, Failed, Refunded...
    public PaymentMethod PaymentMethod { get; set; }                  // CreditCard, EFT...
    
    // Kart bilgileri (gereksiz hassas bilgi yok)
    public string CardType { get; set; }                       // Visa, Mastercard...
    public int? Installments { get; set; }                     // Taksit sayısı

    // İade bilgileri
    public bool IsRefunded { get; set; }
    public decimal? RefundAmount { get; set; }

    // Fraud kontrolü
    public int? FraudScore { get; set; }
    public string IpAddress { get; set; }

    // Hata & ham veri
    public string ErrorMessage { get; set; }
    public string RawResponse { get; set; }

    // Zaman
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RefundedAt { get; set; }
}