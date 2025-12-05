namespace MarketBackend.Models.DTOs;

public class MeResponseDto
{
    public required string Id { get; set; }

    public string? Email { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }

    public List<string> Roles { get; set; } = new();

    // Seller Mağaza Bilgileri
    public string? StoreName { get; set; }
    public string? StoreSlug { get; set; }
    public string? StoreLogoUrl { get; set; }
    public string? StoreDescription { get; set; }
    public bool IsStoreVerified { get; set; }
}