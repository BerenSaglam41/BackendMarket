namespace MarketBackend.Models.Enums;

public enum InvoiceType
{
    EArsiv = 0,       // E-Arşiv fatura (B2C için)
    EFatura = 1,      // E-Fatura (B2B için)
    Istisna = 2,      // Vergi istisnalı fatura türü
    Export = 3,       // İhracat faturası
    Proforma = 4      // Teklif niteliğinde, resmi olmayan fatura
}