using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;
using System.Security.Claims;

namespace MarketBackend.Controllers;

/// <summary>
/// Admin platform kupon yönetimi
/// Tüm sitede geçerli platform kuponları için
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CouponController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CouponController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Tüm kuponları getir (Admin + Seller kuponları)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _context.Coupons
            .Include(c => c.CreatedByAdmin)
            .Include(c => c.CreatedBySeller)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var response = coupons.Select(c => ToCouponDto(c)).ToList();

        return Ok(ApiResponse<List<CouponResponseDto>>.SuccessResponse(
            response,
            "Kuponlar başarıyla getirildi."
        ));
    }

    // ID ile kupon getir
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var coupon = await _context.Coupons
            .Include(c => c.CreatedByAdmin)
            .Include(c => c.CreatedBySeller)
            .FirstOrDefaultAsync(c => c.CouponId == id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        var dto = ToCouponDto(coupon);
        return Ok(ApiResponse<CouponResponseDto>.SuccessResponse(
            dto,
            "Kupon başarıyla getirildi."
        ));
    }

    // Platform kuponu oluştur (Admin)
    [HttpPost]
    public async Task<IActionResult> Create(AdminCouponCreateDto dto)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId))
            throw new UnauthorizedException("Kullanıcı kimliği alınamadı.");

        // Aynı kod var mı kontrol et
        bool codeExists = await _context.Coupons
            .AnyAsync(c => c.Code == dto.Code.ToUpper());

        if (codeExists)
            throw new ConflictException($"'{dto.Code}' kodu zaten kullanılıyor.");

        var coupon = new Coupon
        {
            Code = dto.Code.ToUpper(),
            DiscountPercentage = dto.DiscountPercentage,
            ValidFrom = dto.ValidFrom,
            ValidUntil = dto.ValidUntil,
            MinimumPurchaseAmount = dto.MinimumPurchaseAmount,
            MaxUsageCount = dto.MaxUsageCount,
            CreatedByAdminId = adminId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        var responseDto = ToCouponDto(coupon);
        return CreatedAtAction(
            nameof(GetById),
            new { id = coupon.CouponId },
            ApiResponse<CouponResponseDto>.SuccessResponse(
                responseDto,
                "Kupon başarıyla oluşturuldu.",
                201
            )
        );
    }

    // Kupon güncelle (Sadece platform kuponları)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CouponUpdateDto dto)
    {
        var coupon = await _context.Coupons.FindAsync(id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        // Sadece platform kuponları güncellenebilir
        if (coupon.CreatedByAdminId == null)
            throw new ForbiddenException("Sadece platform kuponları güncellenebilir.");

        if (dto.DiscountPercentage.HasValue)
            coupon.DiscountPercentage = dto.DiscountPercentage.Value;

        if (dto.ValidFrom.HasValue)
            coupon.ValidFrom = dto.ValidFrom.Value;

        if (dto.ValidUntil.HasValue)
            coupon.ValidUntil = dto.ValidUntil.Value;

        if (dto.MinimumPurchaseAmount.HasValue)
            coupon.MinimumPurchaseAmount = dto.MinimumPurchaseAmount.Value;

        if (dto.MaxUsageCount.HasValue)
            coupon.MaxUsageCount = dto.MaxUsageCount.Value;

        if (dto.IsActive.HasValue)
            coupon.IsActive = dto.IsActive.Value;

        coupon.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Kupon başarıyla güncellendi."
        ));
    }

    // Kupon sil (Sadece platform kuponları)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var coupon = await _context.Coupons.FindAsync(id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        // Sadece platform kuponları silinebilir
        if (coupon.CreatedByAdminId == null)
            throw new ForbiddenException("Sadece platform kuponları silinebilir.");

        _context.Coupons.Remove(coupon);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Kupon başarıyla silindi."
        ));
    }

    // Kupon aktif/pasif yap
    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var coupon = await _context.Coupons.FindAsync(id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        coupon.IsActive = !coupon.IsActive;
        coupon.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            new { IsActive = coupon.IsActive },
            coupon.IsActive ? "Kupon aktif edildi." : "Kupon pasif edildi."
        ));
    }

    // Kupon kullanım istatistikleri (Tüm kuponlar için - Admin)
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetCouponStats(int id)
    {
        var coupon = await _context.Coupons
            .Include(c => c.CreatedByAdmin)
            .Include(c => c.CreatedBySeller)
            .FirstOrDefaultAsync(c => c.CouponId == id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        var usageRate = coupon.MaxUsageCount.HasValue
            ? (double)coupon.CurrentUsageCount / coupon.MaxUsageCount.Value * 100
            : 0;

        int? remainingUses = coupon.MaxUsageCount.HasValue
            ? coupon.MaxUsageCount.Value - coupon.CurrentUsageCount
            : null;

        var daysRemaining = (coupon.ValidUntil - DateTime.UtcNow).Days;
        var isExpired = DateTime.UtcNow > coupon.ValidUntil;
        var isNotStarted = DateTime.UtcNow < coupon.ValidFrom;

        var stats = ToCouponStatsDto(coupon);
        return Ok(ApiResponse<CouponStatsResponseDto>.SuccessResponse(
            stats,
            "Kupon istatistikleri başarıyla getirildi."
        ));
    }
    private static CouponResponseDto ToCouponDto(Coupon c)
    {
        return new CouponResponseDto
        {
            CouponId = c.CouponId,
            Code = c.Code,
            DiscountPercentage = c.DiscountPercentage,
            IsActive = c.IsActive,
            ValidFrom = c.ValidFrom,
            ValidUntil = c.ValidUntil,
            MinimumPurchaseAmount = c.MinimumPurchaseAmount,
            MaxUsageCount = c.MaxUsageCount,
            CurrentUsageCount = c.CurrentUsageCount,
            CouponType = c.CreatedByAdminId != null ? "Platform" : "Seller",
            SellerStoreName = c.CreatedBySeller?.StoreName,
            CreatedAt = c.CreatedAt
        };
    }
    private static CouponStatsResponseDto ToCouponStatsDto(Coupon c)
{
    var usageRate = c.MaxUsageCount.HasValue 
        ? (double)c.CurrentUsageCount / c.MaxUsageCount.Value * 100 
        : 0;

    int? remainingUses = c.MaxUsageCount.HasValue 
        ? c.MaxUsageCount.Value - c.CurrentUsageCount 
        : null;

    var daysRemaining = (c.ValidUntil - DateTime.UtcNow).Days;
    var isExpired = DateTime.UtcNow > c.ValidUntil;
    var isNotStarted = DateTime.UtcNow < c.ValidFrom;

    return new CouponStatsResponseDto
    {
        CouponId = c.CouponId,
        Code = c.Code,
        CouponType = c.CreatedByAdminId != null ? "Platform" : "Seller",
        SellerStoreName = c.CreatedBySeller?.StoreName,
        CurrentUsageCount = c.CurrentUsageCount,
        MaxUsageCount = c.MaxUsageCount,
        RemainingUses = remainingUses,
        UsageRate = Math.Round(usageRate, 2),
        IsActive = c.IsActive,
        IsExpired = isExpired,
        IsNotStarted = isNotStarted,
        DaysRemaining = daysRemaining > 0 ? daysRemaining : 0,
        ValidFrom = c.ValidFrom,
        ValidUntil = c.ValidUntil,
        DiscountPercentage = c.DiscountPercentage,
        MinimumPurchaseAmount = c.MinimumPurchaseAmount
    };
}
}
