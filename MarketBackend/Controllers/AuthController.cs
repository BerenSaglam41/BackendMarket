using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Services;
using Microsoft.AspNetCore.Authorization;
using MarketBackend.Models.Common;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1) Email zaten var mı?
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new ConflictException($"'{dto.Email}' email adresi zaten kayıtlı.");

        // 2) Kullanıcı oluştur
        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new BadRequestException("Kullanıcı oluşturulamadı.", result.Errors.Select(e => e.Description).ToList());

        // 3) Rol ata
        await _userManager.AddToRoleAsync(user, "Customer");

        // 4) Token oluştur
        var token = await _tokenService.CreateTokenAsync(user);

        var response = new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
            response,
            "Kayıt başarılı. Hoş geldiniz!",
            201
        ));
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // 1) Kullanıcı var mı?
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            throw new UnauthorizedException("Email veya şifre hatalı.");

        // 2) Kullanıcı aktif mi?
        if (!user.IsActive)
            throw new UnauthorizedException("Hesabınız pasif durumda.");

        // 3) Şifre doğru mu?
        var passwordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordCorrect)
            throw new UnauthorizedException("Email veya şifre hatalı.");

        // LastLogin güncelle
        user.LastLogin = DateTime.UtcNow;
        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            Console.WriteLine($"LastLogin güncellenemedi: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
        }

        // 4) Token oluştur
        var token = await _tokenService.CreateTokenAsync(user);

        // 5) Login başarılı → kullanıcı bilgilerini döndür
        var response = new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Email = user.Email ?? "",
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
            response,
            "Giriş başarılı. Hoş geldiniz!"
        ));
    }

    /// <summary>
    /// Mevcut müşteriyi Seller'a yükselt
    /// POST /api/Auth/become-seller
    /// </summary>
    [HttpPost("become-seller")]
    [Authorize]
    public async Task<IActionResult> BecomeSeller(BecomeSellerDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Geçerli kullanıcı bulunamadı.");

        // Zaten Seller mı?
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Seller"))
            throw new BadRequestException("Zaten bir satıcı hesabınız var.");

        // Store slug benzersiz mi?
        var existingStore = await _userManager.Users
            .AnyAsync(u => u.StoreSlug == dto.StoreSlug && u.Id != user.Id);
        if (existingStore)
            throw new ConflictException($"'{dto.StoreSlug}' mağaza URL'si zaten kullanılıyor.");

        // Mağaza bilgilerini güncelle
        user.StoreName = dto.StoreName;
        user.StoreSlug = dto.StoreSlug;
        user.StoreLogoUrl = dto.StoreLogoUrl;
        user.StoreDescription = dto.StoreDescription;
        user.StorePhone = dto.StorePhone;
        user.IsStoreVerified = false;  // Admin onayı bekleyecek

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new BadRequestException("Kullanıcı bilgileri güncellenemedi.", updateResult.Errors.Select(e => e.Description).ToList());

        // Tüm mevcut rolleri kaldır ve sadece Seller rolü ekle
        var removeResult = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!removeResult.Succeeded)
            throw new BadRequestException("Roller kaldırılamadı.", removeResult.Errors.Select(e => e.Description).ToList());
        
        await _userManager.AddToRoleAsync(user, "Seller");

        // Yeni token oluştur (roller değişti)
        var token = await _tokenService.CreateTokenAsync(user);

        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                token = token,
                expiresAt = DateTime.UtcNow.AddHours(24),
                store = new
                {
                    storeName = user.StoreName,
                    storeSlug = user.StoreSlug,
                    isVerified = user.IsStoreVerified
                }
            },
            "Tebrikler! Artık bir satıcısınız."
        ));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Geçerli kullanıcı bulunamadı.");

        var roles = await _userManager.GetRolesAsync(user);
        var response = new MeResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,

            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin,

            Roles = roles.ToList(),

            // Seller Mağaza Bilgileri
            StoreName = user.StoreName,
            StoreSlug = user.StoreSlug,
            StoreLogoUrl = user.StoreLogoUrl,
            StoreDescription = user.StoreDescription,
            IsStoreVerified = user.IsStoreVerified
        };

        return Ok(ApiResponse<MeResponseDto>.SuccessResponse(
            response,
            "Kullanıcı bilgileriniz başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Eski kullanıcıların çoklu rollerini temizle - SADECE ADMIN
    /// POST /api/auth/cleanup-roles
    /// </summary>
    [HttpPost("cleanup-roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupUserRoles()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        int cleanedCount = 0;

        foreach (var user in allUsers)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            
            // Eğer birden fazla rol varsa
            if (userRoles.Count > 1)
            {
                // Tüm rolleri al
                var rolesList = userRoles.ToList();
                
                // Hangi rolü tutacağımıza karar ver (öncelik sırasına göre)
                string primaryRole;
                if (rolesList.Contains("Admin"))
                {
                    primaryRole = "Admin";
                }
                else if (rolesList.Contains("Seller"))
                {
                    primaryRole = "Seller";
                }
                else if (rolesList.Contains("Support"))
                {
                    primaryRole = "Support";
                }
                else
                {
                    primaryRole = "Customer";
                }

                // Diğer tüm rolleri sil
                var rolesToRemove = rolesList.Where(r => r != primaryRole).ToList();
                
                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    cleanedCount++;
                }
            }
        }

        return Ok(ApiResponse<object>.SuccessResponse(
            new { CleanedUsersCount = cleanedCount, TotalUsers = allUsers.Count },
            $"{cleanedCount} kullanıcının rolleri temizlendi."
        ));
    }
}