namespace MarketBackend.Models.DTOs;

public class SellerDashboardResponseDto
{
    public int PendingProducts { get; set; }
    public int NeedsUpdateProducts { get; set; }
    public int ActiveListings { get; set; }
    public int InactiveListings { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }

    public int OutOfStockListings { get; set; }
    public int LowStockListings { get; set; }

    public List<SellerRecentOrderDto> RecentOrders { get; set; } = new();
    public List<SellerRecentPendingProductDto> RecentPendingProducts { get; set; } = new();
}