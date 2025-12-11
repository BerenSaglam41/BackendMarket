using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/seller-applications")]
[Authorize]
public class SellerApplicationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public SellerApplicationController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    // Basvuru
    [HttpPost]
    public async Task<IActionResult> Create(SellerApplicationCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");
        var hasActive = await _context.SellerApplications
            .AnyAsync(x => x.AppUserId == user.Id && (
                x.Status == SellerApplicationStatus.Pending ||
                x.Status == SellerApplicationStatus.NeedsUpdate
            ));
        if (await _userManager.IsInRoleAsync(user, "Seller"))
            throw new BadRequestException("Zaten satıcı olarak onaylanmışsınız. Bu işlemi yapamazsınız.");
        if (hasActive)
            throw new BadRequestException("Zaten aktif bir satıcı başvurunuz bulunmaktadır.");
        
        var app = new SellerApplication
        {
            AppUserId = user.Id,
            StoreName = dto.StoreName,
            StoreSlug = dto.StoreSlug,
            StoreDescription = dto.StoreDescription,
            StorePhone = dto.StorePhone,
            StoreLogoUrl = dto.StoreLogoUrl,
            Status = SellerApplicationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.SellerApplications.Add(app);
        await _context.SaveChangesAsync();
        var response = SellerAppToDto(app);
        return Ok(ApiResponse.SuccessResponse(response, "Başvurunuz alınmıştır. Değerlendirme süreci başlayacaktır.", 201));
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyApplication()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");
        var apps = await _context.SellerApplications
            .Where(x => x.AppUserId == user.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        var response = apps.Select(x => SellerAppToDto(x)).ToList();
        return Ok(ApiResponse.SuccessResponse(response, "Başvurularınız başarıyla getirildi."));
    }
    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");
        var app = await _context.SellerApplications
            .Where(x => x.AppUserId == user.Id)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
        if (app == null)
            throw new NotFoundException("Herhangi bir satıcı başvurunuz bulunamadı.");
        var response = SellerAppToDto(app);
        return Ok(ApiResponse<SellerApplicationResponseDto>.SuccessResponse(response, "Satıcı başvuru durumu getirildi."));
    }
    //                  Admin İşlemleri
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGetAll()
    {
        var apps = await _context.SellerApplications
            .Include(a => a.AppUser)
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
        var response = apps.Select(x => SellerAppToDto(x)).ToList();
        return Ok(ApiResponse<List<SellerApplicationResponseDto>>.SuccessResponse(
            response,
            "Tüm satıcı başvuruları başarıyla getirildi."
        ));
    }

    // Basvuru detay
    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminGet(int id)
    {
        var app = await _context.SellerApplications
            .Include(a => a.AppUser)
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
        if (app == null)
            throw new NotFoundException("Satıcı başvurusu bulunamadı.");
        var response = SellerAppToDto(app);
        return Ok(ApiResponse<SellerApplicationResponseDto>.SuccessResponse(
            response,
            "Satıcı başvuru detayı başarıyla getirildi."
        ));
    }
    // Basvuru Onayla
    [HttpPost("admin/{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id, SellerApplicationReviewDto dto)
    {
        var app = await _context.SellerApplications
            .Include(a => a.AppUser)
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
        if (app == null)
            throw new NotFoundException("Satıcı başvurusu bulunamadı.");
        var adminId = _userManager.GetUserId(User);
        var user = app.AppUser;
        if (user == null)
            throw new NotFoundException("Başvuru sahibi kullanıcı bulunamadı.");
        user.StoreName = app.StoreName;
        user.StoreSlug = app.StoreSlug;
        user.StoreDescription = app.StoreDescription;
        user.StorePhone = app.StorePhone;
        user.StoreLogoUrl = app.StoreLogoUrl ?? string.Empty;
        user.IsStoreVerified = true;
        await _userManager.AddToRoleAsync(user, "Seller");
        await _userManager.RemoveFromRoleAsync(user, "Customer");
        app.Status = SellerApplicationStatus.Approved;
        app.ReviewedAt = DateTime.UtcNow;
        app.ReviewedByAdminId = adminId;
        app.AdminNote = dto.AdminNote;
        await _context.SaveChangesAsync();
        var response = SellerAppToDto(app);
        return Ok(ApiResponse<SellerApplicationResponseDto>.SuccessResponse(
            response,
            "Satıcı başvurusu onaylandı ve kullanıcı satıcı olarak güncellendi."
        ));
    }
    [HttpPost("admin/{id:int}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id, SellerApplicationReviewDto dto)
    {
        var app = await _context.SellerApplications
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
        if (app == null)
            throw new NotFoundException("Satıcı başvurusu bulunamadı.");
        var adminId = _userManager.GetUserId(User);
        app.Status = SellerApplicationStatus.Rejected;
        app.ReviewedAt = DateTime.UtcNow;
        app.ReviewedByAdminId = adminId;
        app.AdminNote = dto.AdminNote;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse(
            "Satıcı başvurusu reddedildi."
        ));
    }
    [HttpPost("admin/{id:int}/needs-update")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> NeedsUpdate(int id, SellerApplicationReviewDto dto)
    {
        var app = await _context.SellerApplications
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
        if (app == null)
            throw new NotFoundException("Satıcı başvurusu bulunamadı.");
        var adminId = _userManager.GetUserId(User);
        app.Status = SellerApplicationStatus.NeedsUpdate;
        app.ReviewedAt = DateTime.UtcNow;
        app.ReviewedByAdminId = adminId;
        app.AdminNote = dto.AdminNote;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse(
            "Satıcı başvurusu güncelleme bekliyor durumuna getirildi."
        ));
    }
    private SellerApplicationResponseDto SellerAppToDto(SellerApplication x)
    {
        return new SellerApplicationResponseDto
        {
            SellerApplicationId = x.SellerApplicationId,
            AppUserId = x.AppUserId,
            StoreName = x.StoreName,
            StoreSlug = x.StoreSlug,
            StoreDescription = x.StoreDescription,
            StorePhone = x.StorePhone,
            StoreLogoUrl = x.StoreLogoUrl ?? string.Empty,
            Status = x.Status,
            AdminNote = x.AdminNote,
            CreatedAt = x.CreatedAt,
            ReviewedAt = x.ReviewedAt,
            ReviewedByAdminId = x.ReviewedByAdminId
        };
    }
}