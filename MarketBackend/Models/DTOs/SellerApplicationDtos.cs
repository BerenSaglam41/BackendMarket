namespace MarketBackend.Models.DTOs;
public class SellerApplicationCreateDto
{
    public string StoreLogoUrl { get; set; }
    public string StoreName { get; set; } = null;
    public string StoreSlug { get; set; } = null;
    public string StoreDescription { get; set; } 
    public string StorePhone { get; set; }
}
public class SellerApplicationResponseDto
{
    public int SellerApplicationId { get; set; }
    public string AppUserId { get; set; } = null!;
    public string StoreLogoUrl { get; set; }
    public string StoreName { get; set; } = null!;
    public string StoreSlug { get; set; } = null!;
    public string StoreDescription { get; set; } 
    public string StorePhone { get; set; }

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