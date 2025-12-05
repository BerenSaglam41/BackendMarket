using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Services;
using Microsoft.AspNetCore.Authorization;

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
            return BadRequest(new { message = "Bu email zaten kayıtlı." });

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
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        // 3) Rol ata
        await _userManager.AddToRoleAsync(user, "Customer");

        // 4) Token oluştur
        var token = await _tokenService.CreateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // 1) Kullanıcı var mı?
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            return Unauthorized(new { message = "Email veya şifre hatalı." });

        // 2) Kullanıcı aktif mi?
        if (!user.IsActive)
            return Unauthorized(new { message = "Hesabınız pasif durumda." });

        // 3) Şifre doğru mu?
        var passwordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!passwordCorrect)
            return Unauthorized(new { message = "Email veya şifre hatalı." });

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
        return Ok(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            Email = user.Email ?? "",
            FirstName = user.FirstName,
            LastName = user.LastName
        });
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
            return Unauthorized(new { message = "Kullanıcı bulunamadı." });

        // Zaten Seller mı?
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Seller"))
            return BadRequest(new { message = "Zaten bir satıcı hesabınız var." });

        // Store slug benzersiz mi?
        var existingStore = await _userManager.Users
            .AnyAsync(u => u.StoreSlug == dto.StoreSlug && u.Id != user.Id);
        if (existingStore)
            return BadRequest(new { message = "Bu mağaza URL'si zaten kullanılıyor." });

        // Mağaza bilgilerini güncelle
        user.StoreName = dto.StoreName;
        user.StoreSlug = dto.StoreSlug;
        user.StoreLogoUrl = dto.StoreLogoUrl;
        user.StoreDescription = dto.StoreDescription;
        user.StorePhone = dto.StorePhone;
        user.IsStoreVerified = false;  // Admin onayı bekleyecek

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return BadRequest(new { errors = updateResult.Errors.Select(e => e.Description) });

        // Seller rolü ekle (Customer rolü kalır - hem alıcı hem satıcı olabilir)
        await _userManager.AddToRoleAsync(user, "Seller");

        // Yeni token oluştur (roller değişti)
        var token = await _tokenService.CreateTokenAsync(user);

        return Ok(new
        {
            message = "Tebrikler! Artık bir satıcısınız.",
            token = token,
            expiresAt = DateTime.UtcNow.AddHours(24),
            store = new
            {
                storeName = user.StoreName,
                storeSlug = user.StoreSlug,
                isVerified = user.IsStoreVerified
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

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

        return Ok(response);
    }
}