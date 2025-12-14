namespace MarketBackend.Models.DTOs;

public class AdminDashboardResponseDto
{
    // ===== COUNTS =====
    public int TotalUsers { get; set; }
    public int TotalSellers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalListings { get; set; }
    public int TotalCategories { get; set; }
    public int TotalBrands { get; set; }

    // ===== ALERTS =====
    public int TotalPendingSellerApplications { get; set; }
    public int TotalPendingProducts { get; set; }
    public int TotalReportedReviews { get; set; }

    // ===== RECENT =====
    public List<RecentSellerApplicationDto> RecentSellerApplications { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<ReportedReviewDto> RecentReportedReviews { get; set; } = new();
}

public class RecentSellerApplicationDto
{
    public int SellerApplicationId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ApplicantEmail { get; set; } = string.Empty; // Added field
    public string? AdminNote { get; set; } // Added field
}

public class RecentOrderDto
{
    public int OrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string OrderNumber { get; set; } = string.Empty; // Added field
    public string PaymentStatus { get; set; } = string.Empty; // Added field
}

public class ReportedReviewDto
{
    public int ReviewId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int ReportCount { get; set; }
    public DateTime CreatedAt { get; set; }
}