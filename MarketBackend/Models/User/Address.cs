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
    public string ContactName { get; set; }             // Alıcı adı (Kime teslim edilecek?)
    public string ContactPhone { get; set; }            // Alıcı telefonu

    // Konum Bilgileri (Dropdown'lardan seçilir)
    public string Country { get; set; }                 // "Türkiye" (Sabit veya dropdown)
    public string City { get; set; }                    // İl (Dropdown)
    public string District { get; set; }                // İlçe (Dropdown)
    public string Neighborhood { get; set; }            // Mahalle (Dropdown)
    
    // Tam Adres (Kullanıcı yazar: Sokak, Cadde, Bina No, Daire vb.)
    public string FullAddress { get; set; }             // Örn: "Atatürk Caddesi No:45 Daire:8"
    
    public string PostalCode { get; set; }              // Posta Kodu

    // Varsayılan Adres
    public bool IsDefault { get; set; }

    // Adres Türü
    public AddressType AddressType { get; set; }        // Shipping, Billing, Both

    // Oluşturulma ve Güncellenme
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}