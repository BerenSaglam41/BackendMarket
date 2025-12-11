namespace MarketBackend.Models.DTOs;
public class SellerApplicationCreateDto
{
    public string? StoreLogoUrl { get; set; } // Made nullable
    public string? StoreName { get; set; } // Made nullable
    public string StoreSlug { get; set; } = string.Empty; // Assigned default value
    public string? StoreDescription { get; set; } // Made nullable
    public string? StorePhone { get; set; } // Made nullable
}
public class SellerApplicationResponseDto
{
    public int SellerApplicationId { get; set; }
    public string AppUserId { get; set; } = null!;
    public string? StoreLogoUrl { get; set; } // Made nullable
    public string? StoreName { get; set; } = null; // Made nullable
    public string StoreSlug { get; set; } = null!; // Assigned default value
    public string? StoreDescription { get; set; } // Made nullable
    public string? StorePhone { get; set; } // Made nullable

    public SellerApplicationStatus Status { get; set; } 
    public string? AdminNote { get; set; }

    public DateTime CreatedAt { get; set; } 
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }
}
public class SellerApplicationReviewDto
{
    public string? AdminNote { get; set; }
}