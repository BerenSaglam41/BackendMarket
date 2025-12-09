using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;

namespace MarketBackend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AddressController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AddressController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Kullanıcının tüm adreslerini getir
    /// GET /api/Address
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        var addresses = await _context.Addresses
            .Where(a => a.AppUserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AddressResponseDto
            {
                AddressId = a.AddressId,
                Title = a.Title,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                Country = a.Country,
                City = a.City,
                District = a.District,
                Neighborhood = a.Neighborhood,
                FullAddress = a.FullAddress,
                PostalCode = a.PostalCode,
                AddressType = a.AddressType,
                IsDefault = a.IsDefault,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<List<AddressResponseDto>>.SuccessResponse(
            addresses,
            "Adresler başarıyla getirildi"
        ));
    }

    /// <summary>
    /// Tek adres detayı
    /// GET /api/Address/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAddressById(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        var address = await _context.Addresses
            .Where(a => a.AddressId == id && a.AppUserId == userId)
            .Select(a => new AddressResponseDto
            {
                AddressId = a.AddressId,
                Title = a.Title,
                ContactName = a.ContactName,
                ContactPhone = a.ContactPhone,
                Country = a.Country,
                City = a.City,
                District = a.District,
                Neighborhood = a.Neighborhood,
                FullAddress = a.FullAddress,
                PostalCode = a.PostalCode,
                AddressType = a.AddressType,
                IsDefault = a.IsDefault,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (address == null)
            throw new NotFoundException("Adres bulunamadı veya size ait değil");

        return Ok(ApiResponse<AddressResponseDto>.SuccessResponse(
            address,
            "Adres başarıyla getirildi"
        ));
    }

    /// <summary>
    /// Yeni adres ekle
    /// POST /api/Address
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAddress(AddressCreateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        // İlk adres otomatik varsayılan
        var hasNoAddress = !await _context.Addresses.AnyAsync(a => a.AppUserId == userId);
        if (hasNoAddress)
            dto.IsDefault = true;

        // Varsayılan yapılacaksa, diğerlerini kaldır
        if (dto.IsDefault)
        {
            await _context.Addresses
                .Where(a => a.AppUserId == userId && a.IsDefault)
                .ForEachAsync(a => a.IsDefault = false);
        }

        var newAddress = new Address
        {
            AppUserId = userId,
            Title = dto.Title,
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            Country = dto.Country,
            City = dto.City,
            District = dto.District,
            Neighborhood = dto.Neighborhood,
            FullAddress = dto.FullAddress,
            PostalCode = dto.PostalCode,
            AddressType = dto.AddressType,
            IsDefault = dto.IsDefault,
            CreatedAt = DateTime.UtcNow
        };

        _context.Addresses.Add(newAddress);
        await _context.SaveChangesAsync();

        var response = new AddressResponseDto
        {
            AddressId = newAddress.AddressId,
            Title = newAddress.Title,
            ContactName = newAddress.ContactName,
            ContactPhone = newAddress.ContactPhone,
            Country = newAddress.Country,
            City = newAddress.City,
            District = newAddress.District,
            Neighborhood = newAddress.Neighborhood,
            FullAddress = newAddress.FullAddress,
            PostalCode = newAddress.PostalCode,
            AddressType = newAddress.AddressType,
            IsDefault = newAddress.IsDefault,
            CreatedAt = newAddress.CreatedAt,
            UpdatedAt = newAddress.UpdatedAt
        };

        return CreatedAtAction(nameof(GetAddressById), new { id = newAddress.AddressId }, 
            ApiResponse<AddressResponseDto>.SuccessResponse(
                response,
                "Adres başarıyla oluşturuldu",
                201
            ));
    }

    /// <summary>
    /// Adres güncelle
    /// PUT /api/Address/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress(int id, AddressUpdateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == id && a.AppUserId == userId);

        if (address == null)
            throw new NotFoundException("Adres bulunamadı veya size ait değil");

        // Varsayılan yapılacaksa, diğerlerini kaldır
        if (dto.IsDefault && !address.IsDefault)
        {
            await _context.Addresses
                .Where(a => a.AppUserId == userId && a.IsDefault && a.AddressId != id)
                .ForEachAsync(a => a.IsDefault = false);
        }

        // Güncelle
        address.Title = dto.Title;
        address.ContactName = dto.ContactName;
        address.ContactPhone = dto.ContactPhone;
        address.Country = dto.Country;
        address.City = dto.City;
        address.District = dto.District;
        address.Neighborhood = dto.Neighborhood;
        address.FullAddress = dto.FullAddress;
        address.PostalCode = dto.PostalCode;
        address.AddressType = dto.AddressType;
        address.IsDefault = dto.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Adres başarıyla güncellendi"
        ));
    }

    /// <summary>
    /// Adres sil
    /// DELETE /api/Address/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == id && a.AppUserId == userId);

        if (address == null)
            throw new NotFoundException("Adres bulunamadı veya size ait değil");

        bool wasDefault = address.IsDefault;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        // Varsayılan silinmişse, en yeni adresi varsayılan yap
        if (wasDefault)
        {
            var firstAddress = await _context.Addresses
                .Where(a => a.AppUserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (firstAddress != null)
            {
                firstAddress.IsDefault = true;
                await _context.SaveChangesAsync();
            }
        }

        return Ok(ApiResponse.SuccessResponse(
            "Adres başarıyla silindi"
        ));
    }

    /// <summary>
    /// Varsayılan adres yap
    /// POST /api/Address/{id}/set-default
    /// </summary>
    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor");

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == id && a.AppUserId == userId);

        if (address == null)
            throw new NotFoundException("Adres bulunamadı veya size ait değil");

        // Tüm adresleri güncelle
        await _context.Addresses
            .Where(a => a.AppUserId == userId)
            .ForEachAsync(a => a.IsDefault = a.AddressId == id);

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Varsayılan adres güncellendi"
        ));
    }
}