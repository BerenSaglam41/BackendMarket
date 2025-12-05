using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MarketBackend.Models;

namespace MarketBackend.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> CreateTokenAsync(AppUser user)
    {
        
        // 1) JWT ayarlarını config'den al
        var jwtSection = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSection["SecretKey"];
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiresInMinutes = int.Parse(jwtSection["ExpiresInMinutes"] ?? "1440"); // 24 saat

        // 2) Kullanıcının rollerini al
        var roles = await _userManager.GetRolesAsync(user);

        // 3) Token içine yazılacak claim'ler
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),                 // Kullanıcı ID
            new Claim(ClaimTypes.NameIdentifier, user.Id),                   // Identity için gerekli
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),      // Email
            new Claim(ClaimTypes.Email, user.Email ?? ""),                   // Identity için gerekli
            new Claim("firstName", user.FirstName ?? string.Empty),
            new Claim("lastName", user.LastName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Benzersiz token ID
        };

        // Rolleri claim olarak ekle
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // 4) Güvenlik anahtarı & imzalama
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 5) Token objesini oluştur
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes), // 24 saat
            signingCredentials: creds
        );

        // 6) Token string'e çevir ve dön
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}