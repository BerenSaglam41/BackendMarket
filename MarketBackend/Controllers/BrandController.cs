using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController : ControllerBase

{
    private readonly ApplicationDbContext _context;

    public BrandController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Tum Markalari Getir
    [HttpGet]
    [AllowAnonymous]
    public async Task <IActionResult> GetAll()
    {
        var brands = await _context.Brands
            .Where(b => b.IsActive)
            .OrderBy(b => b.PriorityRank)
            .ToListAsync();

        var response = brands.Select(b => new BrandResponseDto
        {
            BrandId = b.BrandId,
            Name = b.Name,
            Slug = b.Slug,
            LogoUrl = b.LogoUrl,
            Description = b.Description,
            WebsiteUrl = b.WebsiteUrl,
            IsActive = b.IsActive,
            IsFeatured = b.IsFeatured,
            MetaTitle = b.MetaTitle,
            MetaDescription = b.MetaDescription,
            Country = b.Country,
            EstablishedYear = b.EstablishedYear,
            SupportEmail = b.SupportEmail,
            SupportPhone = b.SupportPhone,
            PriorityRank = b.PriorityRank,
            CreatedAt = b.CreatedAt
        });

        return Ok(response);
    }

    // Slug ile Marka Getir
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task <IActionResult> GetBySlug(string slug)
    {
        var brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Slug == slug && b.IsActive);

        if(brand == null)
            throw new NotFoundException($"'{slug}' slug'ına sahip marka bulunamadı.");
        
        return Ok(new BrandResponseDto
        {
            BrandId = brand.BrandId,
            Name = brand.Name,
            Slug = brand.Slug,
            LogoUrl = brand.LogoUrl,
            Description = brand.Description,
            WebsiteUrl = brand.WebsiteUrl,
            IsActive = brand.IsActive,
            IsFeatured = brand.IsFeatured,
            MetaTitle = brand.MetaTitle,
            MetaDescription = brand.MetaDescription,
            Country = brand.Country,
            EstablishedYear = brand.EstablishedYear,
            SupportEmail = brand.SupportEmail,
            SupportPhone = brand.SupportPhone,
            PriorityRank = brand.PriorityRank,
            CreatedAt = brand.CreatedAt 
        });
    }
    // Admin Marka Olustur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task <IActionResult> Create(BrandCreateDto dto)
    {
        // Slug Kontrol
        bool slugExists = await _context.Brands
            .AnyAsync(b => b.Slug == dto.Slug);
        if (slugExists)
            throw new ConflictException($"'{dto.Slug}' slug'ı zaten kullanılıyor.");
        
        var brand = new Brand
        {
            Name = dto.Name,
            Slug = dto.Slug,
            LogoUrl = dto.LogoUrl,
            Description = dto.Description,
            WebsiteUrl = dto.WebsiteUrl,
            IsActive = dto.IsActive,
            IsFeatured = dto.IsFeatured,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            Country = dto.Country,
            EstablishedYear = dto.EstablishedYear,
            SupportEmail = dto.SupportEmail,
            SupportPhone = dto.SupportPhone,
            PriorityRank = dto.PriorityRank,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        var response = new BrandResponseDto
            {
                BrandId = brand.BrandId,
                Name = brand.Name,
                Slug = brand.Slug,
                LogoUrl = brand.LogoUrl,
                Description = brand.Description,
                WebsiteUrl = brand.WebsiteUrl,
                IsActive = brand.IsActive,
                IsFeatured = brand.IsFeatured,
                MetaTitle = brand.MetaTitle,
                MetaDescription = brand.MetaDescription,
                Country = brand.Country,
                EstablishedYear = brand.EstablishedYear,
                SupportEmail = brand.SupportEmail,
                SupportPhone = brand.SupportPhone,
                PriorityRank = brand.PriorityRank,
                CreatedAt = brand.CreatedAt
            };
        return CreatedAtAction
        (
            nameof(GetBySlug), 
            new { slug = brand.Slug }, 
            response
        );
    }
    // Marka Guncelle
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task <IActionResult> Update(int id, BrandUpdateDto dto)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
            throw new NotFoundException($"ID: {id} olan marka bulunamadı.");
        // Slug Kontrol
        bool slugExists = await _context.Brands
            .AnyAsync(b => b.Slug == dto.Slug && b.BrandId != id);
        if (slugExists)
            throw new ConflictException($"'{dto.Slug}' slug'ı başka bir marka tarafından kullanılıyor.");

        brand.Name = dto.Name;
        brand.Slug = dto.Slug;
        brand.LogoUrl = dto.LogoUrl;
        brand.Description = dto.Description;
        brand.WebsiteUrl = dto.WebsiteUrl;
        brand.IsActive = dto.IsActive;
        brand.IsFeatured = dto.IsFeatured;
        brand.MetaTitle = dto.MetaTitle;
        brand.MetaDescription = dto.MetaDescription;
        brand.Country = dto.Country;
        brand.EstablishedYear = dto.EstablishedYear;
        brand.SupportEmail = dto.SupportEmail;
        brand.SupportPhone = dto.SupportPhone;
        brand.PriorityRank = dto.PriorityRank;
        brand.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
    // Marka Sil
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task <IActionResult> Delete(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
            throw new NotFoundException($"ID: {id} olan marka bulunamadı.");

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}