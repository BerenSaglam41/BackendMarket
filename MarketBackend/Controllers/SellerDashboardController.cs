using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/seller/dashboard")]
[Authorize(Roles = "Seller")]
public class SellerDashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public SellerDashboardController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var seller = await _userManager.GetUserAsync(User);
        if (seller == null)
            throw new UnauthorizedException("Satıcı bulunamadı.");

        // =========================
        // COUNTS
        // =========================

        var pendingProducts = await _context.ProductPendings
            .CountAsync(p =>
                p.SellerId == seller.Id &&
                p.Status == PendingStatus.Waiting);

        var needsUpdateProducts = await _context.ProductPendings
            .CountAsync(p =>
                p.SellerId == seller.Id &&
                p.Status == PendingStatus.NeedsUpdate);

        var dashboardData = await _context.Listings
            .Where(l => l.SellerId == seller.Id)
            .GroupBy(l => 1)
            .Select(g => new
            {
                ActiveListings = g.Count(l => l.IsActive),
                InactiveListings = g.Count(l => !l.IsActive),
                OutOfStockListings = g.Count(l => l.IsActive && l.Stock == 0),
                LowStockListings = g.Count(l => l.IsActive && l.Stock > 0 && l.Stock <= 5)
            })
            .FirstOrDefaultAsync();

        var totalOrders = await _context.OrderItems
            .Where(oi => oi.SellerId == seller.Id)
            .Select(oi => oi.OrderId)
            .Distinct()
            .CountAsync();

        var totalRevenue = _context.OrderItems
            .Where(oi => oi.SellerId == seller.Id && oi.Order != null && oi.Order.PaymentStatus == PaymentStatus.Paid)
            .AsEnumerable()
            .Sum(oi => (decimal?)oi.TotalPrice ?? 0);

        // =========================
        // ALERTS
        // =========================

        var outOfStockCount = await _context.Listings
            .CountAsync(l =>
                l.SellerId == seller.Id &&
                l.IsActive &&
                l.Stock == 0);

        var lowStockCount = await _context.Listings
            .CountAsync(l =>
                l.SellerId == seller.Id &&
                l.IsActive &&
                l.Stock > 0 &&
                l.Stock <= 5);

        // =========================
        // RECENT DATA
        // =========================

        var recentOrders = await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.SellerId == seller.Id)
            .GroupBy(oi => oi.OrderId)
            .Select(g => new SellerRecentOrderDto
            {
                OrderId = g.Key,
                OrderNumber = g.First().Order.OrderNumber,
                TotalPrice = g.Sum(x => x.TotalPrice), // seller'ın o siparişteki toplam kazancı
                PaymentStatus = g.First().Order.PaymentStatus.ToString(),
                OrderStatus = g.First().Order.OrderStatus.ToString(),
                CreatedAt = g.First().Order.CreatedAt
            })
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();

        var recentPendingProducts = await _context.ProductPendings
            .Where(p => p.SellerId == seller.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .Select(p => new SellerRecentPendingProductDto
            {
                ProductPendingId = p.ProductPendingId,
                Name = p.Name,
                Status = p.Status.ToString(),
                AdminNote = p.AdminNote,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        // =========================
        // RESPONSE
        // =========================

        var response = new SellerDashboardResponseDto
        {
            PendingProducts = pendingProducts,
            NeedsUpdateProducts = needsUpdateProducts,
            ActiveListings = dashboardData?.ActiveListings ?? 0,
            InactiveListings = dashboardData?.InactiveListings ?? 0,
            TotalOrders = totalOrders,
            TotalRevenue = Math.Round(totalRevenue, 2),

            OutOfStockListings = dashboardData?.OutOfStockListings ?? 0,
            LowStockListings = dashboardData?.LowStockListings ?? 0,

            RecentOrders = recentOrders,
            RecentPendingProducts = recentPendingProducts
        };

        return Ok(ApiResponse<SellerDashboardResponseDto>.SuccessResponse(
            response,
            "Seller dashboard verileri getirildi."
        ));
    }
}