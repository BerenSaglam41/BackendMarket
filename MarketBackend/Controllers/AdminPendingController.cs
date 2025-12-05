using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;
using System.Text.Json;

namespace MarketBackend.Controllers;

/// <summary>
/// Admin'in pending ürünleri yönetimi (onay/red/güncelleme talebi)
/// </summary>
[ApiController]
[Route("api/admin/pending-products")]
[Authorize(Roles = "Admin")]
public class AdminPendingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AdminPendingController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Tüm pending ürünleri listeler (filtre destekli)
    /// GET /api/admin/pending-products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status = null,
        [FromQuery] string? sellerId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Pagination koruması - negatif veya sıfır değerlerde default'a dön
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Max limit
        
        var query = _context.ProductPendings
            .Include(p => p.Seller)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .AsQueryable();

        // Status filtresi
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PendingStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        // Seller filtresi
        if (!string.IsNullOrEmpty(sellerId))
        {
            query = query.Where(p => p.SellerId == sellerId);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = products.Select(p => ToAdminResponseDto(p));

        return Ok(new
        {
            Data = response,
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            }
        });
    }

    /// <summary>
    /// Tek bir pending ürünün detayını getirir
    /// GET /api/admin/pending-products/{id}
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.ProductPendings
            .Include(p => p.Seller)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductPendingId == id);

        if (product == null)
            throw new NotFoundException($"ID: {id} olan ürün önerisi bulunamadı.");

        return Ok(ToAdminResponseDto(product));
    }

    /// <summary>
    /// Ürün önerisini onaylar → Product + SellerProduct oluşturur
    /// POST /api/admin/pending-products/{id}/approve
    /// </summary>
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromBody] AdminApproveDto? dto)
    {
        var admin = await _userManager.GetUserAsync(User);
        if (admin == null)
            throw new UnauthorizedException("Admin bulunamadı.");

        var pending = await _context.ProductPendings
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.ProductPendingId == id);

        if (pending == null)
            throw new NotFoundException($"ID: {id} olan ürün önerisi bulunamadı.");

        if (pending.Status != PendingStatus.Waiting)
            throw new BadRequestException("Bu ürün önerisi zaten işlenmiş.");

        // Admin düzeltme yapmış olabilir
        var productName = dto?.Name ?? pending.Name;
        var productSlug = dto?.Slug ?? pending.Slug;
        var productDescription = dto?.Description ?? pending.Description;
        var brandId = dto?.BrandId ?? pending.BrandId;
        var categoryId = dto?.CategoryId ?? pending.CategoryId;
        var imageUrl = dto?.ImageUrl ?? pending.ImageUrl;

        // Slug kontrolü (hem Products hem ProductPendings tablosunda)
        bool slugExistsInProducts = await _context.Products.AnyAsync(p => p.Slug == productSlug);
        bool slugExistsInPending = await _context.ProductPendings
            .AnyAsync(p => p.Slug == productSlug && p.ProductPendingId != pending.ProductPendingId);

        if (slugExistsInProducts || slugExistsInPending)
            throw new ConflictException($"'{productSlug}' slug'ı zaten kullanılıyor.");

        using var transaction = await _context.Database.BeginTransactionAsync();
            // 1. Yeni Product oluştur (Seller'ın ürünü olarak işaretle)
            // Product artık sadece tanım içeriyor, fiyat/stok SellerProduct'ta
            var product = new Product
            {
                Name = productName,
                Slug = productSlug,
                Description = productDescription,
                BrandId = brandId,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
                ImageGalleryJson = pending.ImageGalleryJson,
                MetaTitle = dto?.MetaTitle ?? productName,
                MetaDescription = dto?.MetaDescription ?? productDescription,
                IsActive = true,
                CreatedBySellerId = pending.SellerId,  
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 2. Seller için SellerProduct oluştur (fiyat/stok burada)
            var sellerProduct = new SellerProduct
            {
                SellerId = pending.SellerId,
                ProductId = product.ProductId,
                OriginalPrice = pending.ProposedPrice,
                DiscountPercentage = 0,
                Stock = pending.ProposedStock,
                ShippingTimeInDays = pending.ShippingTimeInDays,
                ShippingCost = 0,
                SellerNote = pending.SellerNote,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.SellerProducts.Add(sellerProduct);

            // 3. Pending'i güncelle
            pending.Status = PendingStatus.Approved;
            pending.AdminNote = dto?.AdminNote;
            pending.ReviewedAt = DateTime.UtcNow;
            pending.ReviewedByAdminId = admin.Id;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                message = "Ürün onaylandı ve yayına alındı.",
                ProductId = product.ProductId,
                SellerProductId = sellerProduct.SellerProductId
            });
    }

    /// <summary>
    /// Ürün önerisini reddeder
    /// POST /api/admin/pending-products/{id}/reject
    /// </summary>
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] AdminRejectDto dto)
    {
        var admin = await _userManager.GetUserAsync(User);
        if (admin == null)
            throw new UnauthorizedException("Admin bulunamadı.");

        var pending = await _context.ProductPendings
            .FirstOrDefaultAsync(p => p.ProductPendingId == id);

        if (pending == null)
            throw new NotFoundException($"ID '{id}' ile ürün önerisi bulunamadı.");

        if (pending.Status != PendingStatus.Waiting)
            throw new BadRequestException("Bu ürün önerisi zaten işlenmiş.");

        pending.Status = PendingStatus.Rejected;
        pending.AdminNote = dto.AdminNote;
        pending.ReviewedAt = DateTime.UtcNow;
        pending.ReviewedByAdminId = admin.Id;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Ürün önerisi reddedildi." });
    }

    /// <summary>
    /// Seller'dan güncelleme talep eder
    /// POST /api/admin/pending-products/{id}/request-update
    /// </summary>
    [HttpPost("{id:int}/request-update")]
    public async Task<IActionResult> RequestUpdate(int id, [FromBody] AdminRequestUpdateDto dto)
    {
        var admin = await _userManager.GetUserAsync(User);
        if (admin == null)
            throw new UnauthorizedException("Admin bulunamadı.");

        var pending = await _context.ProductPendings
            .FirstOrDefaultAsync(p => p.ProductPendingId == id);

        if (pending == null)
            throw new NotFoundException($"ID '{id}' ile ürün önerisi bulunamadı.");

        if (pending.Status != PendingStatus.Waiting)
            throw new BadRequestException("Bu ürün önerisi zaten işlenmiş.");

        pending.Status = PendingStatus.NeedsUpdate;
        pending.AdminNote = dto.AdminNote;
        pending.ReviewedAt = DateTime.UtcNow;
        pending.ReviewedByAdminId = admin.Id;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Güncelleme talebi gönderildi." });
    }

    /// <summary>
    /// Dashboard için özet istatistikler
    /// GET /api/admin/pending-products/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var statusCounts = await _context.ProductPendings
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var todayCount = await _context.ProductPendings
            .CountAsync(p => p.CreatedAt.Date == DateTime.UtcNow.Date);

        var weeklyCount = await _context.ProductPendings
            .CountAsync(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-7));

        return Ok(new
        {
            ByStatus = statusCounts,
            TodaySubmissions = todayCount,
            WeeklySubmissions = weeklyCount,
            TotalPending = statusCounts.FirstOrDefault(s => s.Status == "Waiting")?.Count ?? 0
        });
    }

    // ==========================================
    // HELPER METHODS
    // ==========================================

    private static AdminPendingProductResponseDto ToAdminResponseDto(ProductPending p)
    {
        List<string>? imageGallery = null;
        if (!string.IsNullOrEmpty(p.ImageGalleryJson))
        {
            try
            {
                imageGallery = JsonSerializer.Deserialize<List<string>>(p.ImageGalleryJson);
            }
            catch { }
        }

        return new AdminPendingProductResponseDto
        {
            ProductPendingId = p.ProductPendingId,
            SellerId = p.SellerId,
            SellerName = $"{p.Seller?.FirstName} {p.Seller?.LastName}".Trim(),
            SellerEmail = p.Seller?.Email ?? "",
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            SellerCategorySuggestion = p.SellerCategorySuggestion,
            ImageUrl = p.ImageUrl,
            ImageGallery = imageGallery,
            SellerSku = p.SellerSku,
            Barcode = p.Barcode,
            AttributesJson = p.AttributesJson,
            SellerNote = p.SellerNote,
            ProposedPrice = p.ProposedPrice,
            ProposedStock = p.ProposedStock,
            ShippingTimeInDays = p.ShippingTimeInDays,
            Status = p.Status.ToString(),
            AdminNote = p.AdminNote,
            ReviewedAt = p.ReviewedAt,
            ReviewedByAdminId = p.ReviewedByAdminId,
            CreatedAt = p.CreatedAt
        };
    }
}
