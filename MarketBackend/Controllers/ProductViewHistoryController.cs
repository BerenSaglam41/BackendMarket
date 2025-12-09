using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;
using System.Security.Claims;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductViewHistoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMemoryCache _cache;

    public ProductViewHistoryController(
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        IMemoryCache cache)
    {
        _context = context;
        _userManager = userManager;
        _cache = cache;
    }

    // ==========================================
    // RECORD VIEW (Public - Misafir + Üye)
    // ==========================================

    /// <summary>
    /// Listing görüntülemesini kaydet
    /// POST /api/productviewhistory/record
    /// </summary>
    [HttpPost("record")]
    public async Task<IActionResult> RecordView([FromBody] RecordViewDto dto)
    {
        // SellerProduct var mı kontrol et
        var listing = await _context.Listings
            .FirstOrDefaultAsync(sp => sp.ListingId == dto.ListingId && sp.IsActive);

        if (listing == null)
            throw new NotFoundException("Listing bulunamadı.");

        // Kullanıcı ID'sini al (misafir için null)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Spam önleme: 5 dakika cooldown (cache ile)
        var cacheKey = $"view_{userId ?? "guest"}_{dto.ListingId}";
        if (_cache.TryGetValue(cacheKey, out _))
        {
            // Son 5 dakika içinde zaten görüntülemiş, sayma
            return Ok(ApiResponse.SuccessResponse("View already recorded recently."));
        }

        // Mevcut kaydı bul veya yeni oluştur
        var viewHistory = await _context.ProductViewHistories
            .FirstOrDefaultAsync(pvh => 
                pvh.UserId == userId && 
                pvh.ListingId == dto.ListingId);

        if (viewHistory == null)
        {
            // Yeni kayıt
            viewHistory = new ProductViewHistory
            {
                UserId = userId,
                ListingId = dto.ListingId,
                ViewCount = 1,
                FirstViewedAt = DateTime.UtcNow,
                LastViewedAt = DateTime.UtcNow,
                Source = dto.Source,
                DeviceType = dto.DeviceType
            };
            _context.ProductViewHistories.Add(viewHistory);
        }
        else
        {
            // Mevcut kaydı güncelle
            viewHistory.ViewCount++;
            viewHistory.LastViewedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(dto.Source))
                viewHistory.Source = dto.Source;
            if (!string.IsNullOrEmpty(dto.DeviceType))
                viewHistory.DeviceType = dto.DeviceType;
        }

        await _context.SaveChangesAsync();

        // Cache'e ekle (5 dakika)
        _cache.Set(cacheKey, true, TimeSpan.FromMinutes(5));

        return Ok(ApiResponse.SuccessResponse(
            "View recorded successfully."
        ));
    }

    // ==========================================
    // USER - SON GÖRÜNTÜLENEN LİSTINGLER
    // ==========================================

    /// <summary>
    /// Kullanıcının son görüntülediği listingler
    /// GET /api/productviewhistory/my-recent
    /// </summary>
    [HttpGet("my-recent")]
    [Authorize]
    public async Task<IActionResult> GetMyRecentViews([FromQuery] int limit = 20)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var recentViews = await _context.ProductViewHistories
            .Include(pvh => pvh.Listing)
                .ThenInclude(sp => sp.Product)
            .Where(pvh => pvh.UserId == user.Id)
            .OrderByDescending(pvh => pvh.LastViewedAt)
            .Take(limit)
            .Select(pvh => new ProductViewHistoryResponseDto
            {
                ProductViewHistoryId = pvh.ProductViewHistoryId,
                ListingId = pvh.ListingId,
                ListingName = pvh.Listing.Product.Name,
                ListingSlug = pvh.Listing.Slug,
                ViewCount = pvh.ViewCount,
                FirstViewedAt = pvh.FirstViewedAt,
                LastViewedAt = pvh.LastViewedAt,
                Source = pvh.Source
            })
            .ToListAsync();

        return Ok(ApiResponse<List<ProductViewHistoryResponseDto>>.SuccessResponse(
            recentViews,
            "Son görüntülenen listingler getirildi."
        ));
    }

    // ==========================================
    // ADMIN/SELLER - EN ÇOK GÖRÜNTÜLENEN LİSTİNGLER
    // ==========================================

    /// <summary>
    /// En çok görüntülenen listingler (Public)
    /// GET /api/productviewhistory/most-viewed
    /// </summary>
    [HttpGet("most-viewed")]
    public async Task<IActionResult> GetMostViewedListings([FromQuery] int limit = 10)
    {
        var mostViewed = await _context.ProductViewHistories
            .GroupBy(pvh => pvh.ListingId)
            .Select(g => new
            {
                ListingId = g.Key,
                TotalViews = g.Sum(pvh => pvh.ViewCount),
                UniqueViewers = g.Count()
            })
            .OrderByDescending(x => x.TotalViews)
            .Take(limit)
            .ToListAsync();

        var listingIds = mostViewed.Select(m => m.ListingId).ToList();

        var listings = await _context.Listings
            .Where(sp => listingIds.Contains(sp.ListingId) && sp.IsActive)
            .Include(sp => sp.Product)
            .Select(sp => new
            {
                sp.ListingId,
                sp.Product.Name,
                sp.Slug,
                sp.OriginalPrice,
                sp.Product.ImageUrl
            })
            .ToListAsync();

        var result = mostViewed
            .Join(listings,
                mv => mv.ListingId,
                l => l.ListingId,
                (mv, l) => new MostViewedListingDto
                {
                    ListingId = l.ListingId,
                    Name = l.Name,
                    Slug = l.Slug,
                    Price = l.OriginalPrice,
                    ImageUrl = l.ImageUrl,
                    TotalViews = mv.TotalViews,
                    UniqueViewers = mv.UniqueViewers
                })
            .ToList();

        return Ok(ApiResponse<List<MostViewedListingDto>>.SuccessResponse(
            result,
            "En çok görüntülenen listingler getirildi."
        ));
    }

    // ==========================================
    // SELLER - KENDİ LİSTİNGLERİNİN İSTATİSTİKLERİ
    // ==========================================

    /// <summary>
    /// Seller'ın kendi listinglerinin görüntülenme istatistikleri
    /// GET /api/productviewhistory/seller/stats
    /// </summary>
    [HttpGet("seller/stats")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> GetSellerViewStats()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Seller'ın listing ID'lerini al
        var sellerListingIds = await _context.Listings
            .Where(sp => sp.SellerId == user.Id)
            .Select(sp => sp.ListingId)
            .ToListAsync();

        // View istatistiklerini hesapla
        var viewStats = await _context.ProductViewHistories
            .Where(pvh => sellerListingIds.Contains(pvh.ListingId))
            .GroupBy(pvh => pvh.ListingId)
            .Select(g => new
            {
                ListingId = g.Key,
                TotalViews = g.Sum(pvh => pvh.ViewCount),
                UniqueViewers = g.Count(),
                LastViewedAt = g.Max(pvh => pvh.LastViewedAt)
            })
            .ToListAsync();

        var listings = await _context.Listings
            .Where(sp => sellerListingIds.Contains(sp.ListingId))
            .Include(sp => sp.Product)
            .Select(sp => new
            {
                sp.ListingId,
                sp.Product.Name,
                sp.Slug
            })
            .ToListAsync();

        var result = viewStats
            .Join(listings,
                vs => vs.ListingId,
                l => l.ListingId,
                (vs, l) => new
                {
                    l.ListingId,
                    l.Name,
                    l.Slug,
                    vs.TotalViews,
                    vs.UniqueViewers,
                    vs.LastViewedAt
                })
            .OrderByDescending(x => x.TotalViews)
            .ToList();

        return Ok(ApiResponse<object>.SuccessResponse(
            result,
            "Görüntülenme istatistikleri getirildi."
        ));
    }

    // ==========================================
    // DELETE - GEÇMİŞİ TEMİZLE
    // ==========================================

    /// <summary>
    /// Kullanıcının görüntüleme geçmişini temizle
    /// DELETE /api/productviewhistory/my-history
    /// </summary>
    [HttpDelete("my-history")]
    [Authorize]
    public async Task<IActionResult> ClearMyHistory()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var userHistory = await _context.ProductViewHistories
            .Where(pvh => pvh.UserId == user.Id)
            .ToListAsync();

        _context.ProductViewHistories.RemoveRange(userHistory);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Görüntüleme geçmişi temizlendi."
        ));
    }

    /// <summary>
    /// Belirli bir listing'in geçmişini sil
    /// DELETE /api/productviewhistory/{sellerProductId}
    /// </summary>
    [HttpDelete("{sellerProductId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSpecificView(int listingId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var viewHistory = await _context.ProductViewHistories
            .FirstOrDefaultAsync(pvh => 
                pvh.UserId == user.Id && 
                pvh.ListingId == listingId);

        if (viewHistory == null)
            throw new NotFoundException("Görüntüleme kaydı bulunamadı.");

        _context.ProductViewHistories.Remove(viewHistory);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Görüntüleme kaydı silindi."
        ));
    }
}
