namespace MarketBackend.Models.DTOs;

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool AcceptTerms { get; set; }  // KVKK onayı
}

/// <summary>
/// Mevcut müşterinin Seller'a yükseltilmesi için DTO
/// POST /api/Auth/become-seller
/// </summary>
public class BecomeSellerDto
{
    // Mağaza bilgileri
    public string StoreName { get; set; } = string.Empty;        // "Ahşap Atölyesi"
    public string StoreSlug { get; set; } = string.Empty;        // "ahsap-atolyesi"
    public string? StoreLogoUrl { get; set; }
    public string? StoreDescription { get; set; }
    public string? StorePhone { get; set; }

    public bool AcceptSellerTerms { get; set; }  // Satıcı sözleşmesi
}