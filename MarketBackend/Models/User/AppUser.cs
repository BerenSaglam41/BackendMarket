using Microsoft.AspNetCore.Identity;

namespace MarketBackend.Models;

public class AppUser : IdentityUser
{
    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public string? Preferences { get; set; }                 // JSON string
    public string? ReferralCode { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ==========================================
    // SELLER MAĞAZA BİLGİLERİ
    // ==========================================
    // Eğer kullanıcı Seller rolündeyse bu alanlar kullanılır
    public string? StoreName { get; set; }           // "Ahşap Atölyesi"
    public string? StoreSlug { get; set; }           // "ahsap-atolyesi" (URL için)
    public string? StoreLogoUrl { get; set; }        // Mağaza logosu
    public string? StoreDescription { get; set; }   // Mağaza açıklaması
    public string? StorePhone { get; set; }          // Mağaza telefonu
    public bool IsStoreVerified { get; set; } = false;  // Admin onaylı mağaza mı?

    // Navigation Properties
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ShoppingCart? ShoppingCart { get; set; }         
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    // Seller ilişkileri
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<ProductPending> ProductPendings { get; set; } = new List<ProductPending>();
    
    // Coupon ilişkileri
    public ICollection<Coupon> CreatedAdminCoupons { get; set; } = new List<Coupon>();
    public ICollection<Coupon> CreatedSellerCoupons { get; set; } = new List<Coupon>();
}