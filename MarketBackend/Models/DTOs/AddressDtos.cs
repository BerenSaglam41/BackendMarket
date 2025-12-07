using MarketBackend.Models.Enums;

namespace MarketBackend.Models.DTOs;

/// <summary>
/// Yeni adres oluşturma DTO
/// </summary>
public class AddressCreateDto
{
    public string Title { get; set; }                  // "Ev", "İş"
    public string ContactName { get; set; }            // Alıcı adı
    public string ContactPhone { get; set; }           // Alıcı telefonu
    public string Country { get; set; }                // "Türkiye" (Dropdown)
    public string City { get; set; }                   // İl (Dropdown)
    public string District { get; set; }               // İlçe (Dropdown)
    public string Neighborhood { get; set; }           // Mahalle (Dropdown)
    public string FullAddress { get; set; }            // Tam adres (Kullanıcı yazar)
    public string PostalCode { get; set; }             // Posta kodu
    public AddressType AddressType { get; set; }       // Shipping, Billing, Both
    public bool IsDefault { get; set; }                // Varsayılan adres mi?
}

/// <summary>
/// Adres güncelleme DTO
/// </summary>
public class AddressUpdateDto
{
    public string Title { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Neighborhood { get; set; }
    public string FullAddress { get; set; }
    public string PostalCode { get; set; }
    public AddressType AddressType { get; set; }
    public bool IsDefault { get; set; }
}

/// <summary>
/// Adres response DTO
/// </summary>
public class AddressResponseDto
{
    public int AddressId { get; set; }
    public string Title { get; set; }
    public string ContactName { get; set; }
    public string ContactPhone { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string Neighborhood { get; set; }
    public string FullAddress { get; set; }
    public string PostalCode { get; set; }
    public AddressType AddressType { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
