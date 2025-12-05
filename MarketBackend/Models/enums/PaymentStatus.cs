namespace MarketBackend.Models.Enums;

public enum PaymentStatus
{
    Pending = 0,     // Ödeme başlatıldı, tamamlanmadı
    Paid = 1,        // Ödeme başarılı
    Failed = 2,      // Ödeme başarısız
    Refunded = 3,    // Tam/ kısmi iade yapıldı
    Cancelled = 4    // Ödeme iptal edildi
}