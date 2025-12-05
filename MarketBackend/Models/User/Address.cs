using MarketBackend.Models.Enums;

namespace MarketBackend.Models;

public class Address
{
    public int AddressId { get; set; }                  // PK

    // Foreign Key → AppUser
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }                // Navigation

    // Adres Başlığı
    public string Title { get; set; }                   // "Evim", "İş", "Yazlık"

    // Teslimat Kişisi Bilgileri
    public string ContactName { get; set; }             // Ad + Soyad
    public string ContactPhone { get; set; }

    // Konum Bilgileri
    public string Country { get; set; }                 // Genelde "Türkiye"
    public string City { get; set; }                    // İl
    public string District { get; set; }                // İlçe
    public string Neighborhood { get; set; }            // Mahalle (önerilir)
    public string StreetAddress { get; set; }           // Sokak / Cadde / Apartman
    public string BuildingNumber { get; set; }          // Kapı no, Daire no
    public string ZipCode { get; set; }                 // Opsiyonel

    // Varsayılan Adres
    public bool IsDefaultAddress { get; set; }

    // Adres Türü
    public AddressType AddressType { get; set; }             // "Teslimat", "Fatura"

    // Oluşturulma ve Güncellenme
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}