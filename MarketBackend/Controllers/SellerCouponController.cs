using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Controllers;

/// <summary>
/// Seller mağaza kupon yönetimi
/// Seller'ların kendi mağazalarına özel kupon oluşturması için
/// </summary>
[ApiController]
[Route("api/seller/coupon")]
[Authorize(Roles = "Seller")]
public class SellerCouponController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public SellerCouponController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Seller'ın kendi kuponlarını getir
    [HttpGet]
    public async Task<IActionResult> GetMyCoupons()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupons = await _context.Coupons
            .Where(c => c.CreatedBySellerId == user.Id)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var response = coupons.Select(c => new CouponResponseDto
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
            CouponType = "Seller",
            SellerStoreName = user.StoreName,
            CreatedAt = c.CreatedAt
        });

        return Ok(response);
    }

    // ID ile seller kupon getir
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponId == id && c.CreatedBySellerId == user.Id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı.");

        return Ok(new CouponResponseDto
        {
            CouponId = coupon.CouponId,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage,
            IsActive = coupon.IsActive,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MinimumPurchaseAmount = coupon.MinimumPurchaseAmount,
            MaxUsageCount = coupon.MaxUsageCount,
            CurrentUsageCount = coupon.CurrentUsageCount,
            CouponType = "Seller",
            SellerStoreName = user.StoreName,
            CreatedAt = coupon.CreatedAt
        });
    }

    // Seller mağaza kuponu oluştur
    [HttpPost]
    public async Task<IActionResult> Create(SellerCouponCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Seller mağaza bilgisi var mı kontrol
        if (string.IsNullOrEmpty(user.StoreName))
            throw new BadRequestException("Mağaza bilgileriniz eksik. Lütfen önce mağazanızı tamamlayın.");

        // Aynı kod var mı kontrol et
        bool codeExists = await _context.Coupons
            .AnyAsync(c => c.Code == dto.Code.ToUpper());

        if (codeExists)
            throw new ConflictException($"'{dto.Code}' kodu zaten kullanılıyor. Lütfen farklı bir kod seçin.");

        var coupon = new Coupon
        {
            Code = dto.Code.ToUpper(),
            DiscountPercentage = dto.DiscountPercentage,
            ValidFrom = dto.ValidFrom,
            ValidUntil = dto.ValidUntil,
            MinimumPurchaseAmount = dto.MinimumPurchaseAmount,
            MaxUsageCount = dto.MaxUsageCount,
            CreatedBySellerId = user.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = coupon.CouponId }, new CouponResponseDto
        {
            CouponId = coupon.CouponId,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage,
            IsActive = coupon.IsActive,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            MinimumPurchaseAmount = coupon.MinimumPurchaseAmount,
            MaxUsageCount = coupon.MaxUsageCount,
            CurrentUsageCount = coupon.CurrentUsageCount,
            CouponType = "Seller",
            SellerStoreName = user.StoreName,
            CreatedAt = coupon.CreatedAt
        });
    }

    // Seller kuponu güncelle
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CouponUpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponId == id && c.CreatedBySellerId == user.Id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı veya size ait değil.");

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

        await _context.SaveChangesAsync();

        return Ok(new { message = "Kupon başarıyla güncellendi." });
    }

    // Seller kuponu sil
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponId == id && c.CreatedBySellerId == user.Id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı veya size ait değil.");

        _context.Coupons.Remove(coupon);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Kupon başarıyla silindi." });
    }

    // Kupon aktif/pasif yap
    [HttpPut("{id}/toggle")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponId == id && c.CreatedBySellerId == user.Id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı veya size ait değil.");

        coupon.IsActive = !coupon.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = coupon.IsActive ? "Kupon aktif edildi." : "Kupon pasif edildi.",
            isActive = coupon.IsActive
        });
    }

    // Kupon kullanım istatistikleri
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetCouponStats(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.CouponId == id && c.CreatedBySellerId == user.Id);

        if (coupon == null)
            throw new NotFoundException($"ID: {id} olan kupon bulunamadı veya size ait değil.");

        var usageRate = coupon.MaxUsageCount.HasValue 
            ? (double)coupon.CurrentUsageCount / coupon.MaxUsageCount.Value * 100 
            : 0;

        int? remainingUses = coupon.MaxUsageCount.HasValue 
            ? coupon.MaxUsageCount.Value - coupon.CurrentUsageCount 
            : null;

        var daysRemaining = (coupon.ValidUntil - DateTime.UtcNow).Days;
        var isExpired = DateTime.UtcNow > coupon.ValidUntil;
        var isNotStarted = DateTime.UtcNow < coupon.ValidFrom;

        return Ok(new
        {
            CouponId = coupon.CouponId,
            Code = coupon.Code,
            CurrentUsageCount = coupon.CurrentUsageCount,
            MaxUsageCount = coupon.MaxUsageCount,
            RemainingUses = remainingUses,
            UsageRate = Math.Round(usageRate, 2),
            IsActive = coupon.IsActive,
            IsExpired = isExpired,
            IsNotStarted = isNotStarted,
            DaysRemaining = daysRemaining > 0 ? daysRemaining : 0,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil
        });
    }
}
