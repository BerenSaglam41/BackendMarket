using MarketBackend.Data;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs; // Bu DTO'larin icinde Enum varsa dikkat
using MarketBackend.Models; // SellerApplicationStatus icin
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet] // EKLENDI
    public async Task<IActionResult> GetDashboardStats()
    {
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);


        var response = new AdminDashboardResponseDto
        {
            // --- COUNTS ---
            TotalUsers = await _context.Users.CountAsync(),
            
            TotalSellers = await _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .CountAsync(x => x.r.Name == "Seller"),

            TotalProducts = await _context.Products.CountAsync(p => p.IsActive),
            TotalListings = await _context.Listings.CountAsync(l => l.IsActive),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalBrands = await _context.Brands.CountAsync(),

            // --- ALERTS ---
            // DÜZELTME: Enum karşılaştırması yapıldı
            TotalPendingSellerApplications = await _context.SellerApplications.CountAsync(a => a.Status == SellerApplicationStatus.Pending),

            TotalPendingProducts = await _context.ProductPendings
                .CountAsync(p => p.Status == PendingStatus.Waiting || p.Status == PendingStatus.NeedsUpdate),

            TotalReportedReviews = await _context.Reviews
                .CountAsync(r => r.ReportCount > 0),

            // --- RECENT LISTS ---
            
            RecentSellerApplications = await _context.SellerApplications
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new RecentSellerApplicationDto
                {
                    SellerApplicationId = a.SellerApplicationId,
                    StoreName = a.StoreName,
                    // Enum'ı string'e çevirerek gönderiyoruz ki frontend rahat etsin
                    Status = a.Status.ToString(), 
                    CreatedAt = a.CreatedAt,
                    ApplicantEmail = a.AppUser != null && a.AppUser.Email != null ? a.AppUser.Email : string.Empty, // Explicit null check
                    AdminNote = a.AdminNote // Added field
                })
                .ToListAsync(),

            RecentOrders = await _context.Orders
                .Include(o => o.AppUser) // Explicitly include AppUser
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderId,
                    UserEmail = o.AppUser != null && o.AppUser.Email != null ? o.AppUser.Email : string.Empty, // Explicit null check
                    TotalAmount = o.TotalAmount,
                    Status = o.OrderStatus.ToString(),
                    CreatedAt = o.CreatedAt,
                    OrderNumber = o.OrderNumber, // Added field
                    PaymentStatus = o.PaymentStatus.ToString() // Added field
                })
                .ToListAsync(),

            RecentReportedReviews = await _context.Reviews
                .Include(r => r.User) // Explicitly include User
                .Include(r => r.Product) // Explicitly include Product
                .Where(r => r.ReportCount > 0)
                .OrderByDescending(r => r.ReportCount)
                .Take(5)
                .Select(r => new ReportedReviewDto
                {
                    ReviewId = r.ReviewId,
                    ProductName = r.Product.Name,
                    UserEmail = r.User != null && r.User.Email != null ? r.User.Email : "Unknown", // Explicit null check
                    Rating = r.Rating,
                    ReportCount = r.ReportCount,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync()
        };

        return Ok(ApiResponse<AdminDashboardResponseDto>.SuccessResponse(
            response,
            "Dashboard verileri güncellendi."
        ));
    }
}