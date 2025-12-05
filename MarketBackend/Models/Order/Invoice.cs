using MarketBackend.Models.Enums;

namespace MarketBackend.Models;

public class Invoice
{
    public int InvoiceId { get; set; }                      // PK

    // Sipariş ilişkisi
    public int OrderId { get; set; }
    public Order Order { get; set; }

    // Yasal bilgiler
    public string InvoiceNumber { get; set; }               // GİB seri/sıra
    public DateTime InvoiceDate { get; set; }               // Faturanın kesildiği tarih/saat
    public InvoiceType InvoiceType { get; set; }                 // E-Fatura, E-Arşiv, İstisna
    public InvoiceStatus InvoiceStatus { get; set; }               // Issued, Cancelled, AwaitingIssue

    // Görüntüleme
    public string InvoiceUrl { get; set; }                  // PDF veya görüntü linki

    // Tutarlar
    public decimal TotalAmount { get; set; }                // KDV dahil, indirim sonrası
    public decimal TotalTaxAmount { get; set; }             // Toplam KDV
    public decimal? TotalDiscountAmount { get; set; }       // Uygulanan indirim (varsa)

    public string Currency { get; set; } = "TRY";           // Para birimi

    // Müşteri bilgileri (snapshot)
    public string CustomerTaxId { get; set; }               // TCKN / VKN
    public string CustomerName { get; set; }                // Firma / kişi adı
    public string BillingAddressSnapshot { get; set; }      // JSON / düz metin olarak adres

    // Sistemsel izleme
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}