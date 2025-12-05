namespace MarketBackend.Models.Enums;

public enum InvoiceStatus
{
    AwaitingIssue = 0,   // Sipariş kesildi ama fatura oluşturulmadı
    Issued = 1,          // Fatura kesildi ancak gönderilmedi
    SentToCustomer = 2,  // Müşteriye e-posta ile gönderildi
    Delivered = 3,       // Müşteri tarafından görüntülendi / teslim alındı
    Cancelled = 4        // Fatura iptal edildi
}