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

    // --- YENİ EKLENEN GÜVENLİK METODLARI ---
    
    // 1. Onaylanmış veya Reddedilmiş başvuruların değiştirilmesini engeller
    private void CheckIfActionable(SellerApplication app)
    {
        if (app.Status == SellerApplicationStatus.Approved)
            throw new BadRequestException("Bu başvuru zaten ONAYLANMIŞ. Üzerinde işlem yapılamaz.");
            
        if (app.Status == SellerApplicationStatus.Rejected)
            throw new BadRequestException("Bu başvuru zaten REDDEDİLMİŞ. Üzerinde işlem yapılamaz.");
    }

    // 2. Mağaza linkinin (slug) benzersiz olmasını sağlar
    private async Task CheckSlugUniqueness(string slug, int? excludeId = null)
    {
        var query = _context.SellerApplications.AsQueryable();
        
        if (excludeId.HasValue)
            query = query.Where(x => x.SellerApplicationId != excludeId.Value);

        var exists = await query.AnyAsync(x => x.StoreSlug == slug && x.Status != SellerApplicationStatus.Rejected);
        
        if (exists)
            throw new BadRequestException("Bu mağaza linki (slug) başka bir satıcı tarafından kullanılıyor.");
    }
    // ------------------------------------------

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
        
        // 🔥 Slug Kontrolü Eklendi
        await CheckSlugUniqueness(dto.StoreSlug);

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

    // KULLANICI GÜNCELLEME İŞLEMİ
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SellerApplicationCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var app = await _context.SellerApplications
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id && x.AppUserId == user.Id);

        if (app == null)
            throw new NotFoundException("Başvuru bulunamadı.");

        // Sadece Pending veya NeedsUpdate güncellenebilir.
        if (app.Status == SellerApplicationStatus.Approved || app.Status == SellerApplicationStatus.Rejected)
             throw new BadRequestException("Bu başvuru sonuçlandığı için güncellenemez.");

        // 🔥 Slug değiştiyse unique kontrolü yap
        if (app.StoreSlug != dto.StoreSlug)
        {
            await CheckSlugUniqueness(dto.StoreSlug, app.SellerApplicationId);
        }

        // Yeni verileri ata
        app.StoreName = dto.StoreName;
        app.StoreSlug = dto.StoreSlug;
        app.StoreDescription = dto.StoreDescription;
        app.StorePhone = dto.StorePhone;
        app.StoreLogoUrl = dto.StoreLogoUrl;

        // 🔥 Güncelleme yapıldığı an statü tekrar PENDING (Onay Bekliyor) olur.
        app.Status = SellerApplicationStatus.Pending;
        
        await _context.SaveChangesAsync();

        var response = SellerAppToDto(app);
        return Ok(ApiResponse.SuccessResponse(response, "Başvurunuz güncellendi ve tekrar onaya gönderildi."));
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
        // 🔥 Transaction Başlat (Veri güvenliği için)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var app = await _context.SellerApplications
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
            
            if (app == null)
                throw new NotFoundException("Satıcı başvurusu bulunamadı.");

            // 🔥 Kilit Kontrolü
            CheckIfActionable(app);

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
            
            // İşlem başarılı, kaydet
            await transaction.CommitAsync();

            var response = SellerAppToDto(app);
            return Ok(ApiResponse<SellerApplicationResponseDto>.SuccessResponse(
                response,
                "Satıcı başvurusu onaylandı ve kullanıcı satıcı olarak güncellendi."
            ));
        }
        catch (Exception)
        {
            // Hata varsa geri al
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpPost("admin/{id:int}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id, SellerApplicationReviewDto dto)
    {
        var app = await _context.SellerApplications
            .FirstOrDefaultAsync(x => x.SellerApplicationId == id);
        if (app == null)
            throw new NotFoundException("Satıcı başvurusu bulunamadı.");

        // 🔥 Kilit Kontrolü
        CheckIfActionable(app);

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
        
        // 🔥 Kilit Kontrolü
        CheckIfActionable(app);

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