namespace MarketBackend.Models.DTOs;

public class SellerRecentPendingProductDto
{
    public int ProductPendingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    public DateTime CreatedAt { get; set; }
}