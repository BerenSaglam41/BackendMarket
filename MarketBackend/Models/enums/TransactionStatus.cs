namespace MarketBackend.Models.Enums;

public enum TransactionStatus
{
    Pending = 0,       // Ödeme işlemi başlatıldı, sonuç bekleniyor
    Success = 1,       // Ödeme başarıyla gerçekleşti
    Failed = 2,        // Ödeme başarısız oldu
    Cancelled = 3,     // Kullanıcı veya sistem tarafından iptal edildi
    Refunded = 4,      // Tam veya kısmi iade yapıldı
    Chargeback = 5     // Banka tarafından ters ibraz (chargeback) açıldı
}