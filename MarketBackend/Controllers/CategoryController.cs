using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

        // 🔹 PUBLIC: Aktif kategorileri listele
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync();
        
        var result = categories.Select(c => new CategoryResponseDto
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Slug = c.Slug,
            ParentCategoryId = c.ParentCategoryId,
            Description = c.Description,
            ImageUrl = c.ImageUrl,
            OrderIndex = c.OrderIndex,
            IsActive = c.IsActive,
            MetaTitle = c.MetaTitle,
            MetaDescription = c.MetaDescription,
            CreatedAt = c.CreatedAt
        }).ToList();

        return Ok(result);
    }
    // 🔹 PUBLIC: Slug ile tek kategori getir
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);
        
        if(category == null)
            return NotFound(new { message = "Kategori bulunamadı." });
        
        var dto = new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Slug = category.Slug,
            ParentCategoryId = category.ParentCategoryId,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            OrderIndex = category.OrderIndex,
            IsActive = category.IsActive,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            CreatedAt = category.CreatedAt
        };
        return Ok(dto);
    }
        // 🔹 ADMIN: Kategori oluştur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task <IActionResult> Create(CategoryCreateDto dto)
    {
        // Slug Benzersiz olcak
        var exists = await _context.Categories.AnyAsync(c => c.Slug == dto.Slug);

        if(exists)
            return BadRequest(new { message = "Bu slug zaten var "});
        
        var category = new Category
        {
            Name = dto.Name,
            Slug = dto.Slug,
            ParentCategoryId = dto.ParentCategoryId,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            OrderIndex = dto.OrderIndex,
            IsActive = dto.IsActive,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBySlug), new { slug = category.Slug }, new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Slug = category.Slug,
            ParentCategoryId = category.ParentCategoryId,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            OrderIndex = category.OrderIndex,
            IsActive = category.IsActive,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            CreatedAt = category.CreatedAt
        });
    }

        // 🔹 ADMIN: Kategori güncelle
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if(category == null)
            return NotFound(new { message = "Kategori bulunamadı." });  
        
        // Slug Cakismasi
        var slugExists = await _context.Categories
            .AnyAsync(c => c.Slug == dto.Slug && c.CategoryId != id);
        if(slugExists)
            return BadRequest(new { message = "Bu slug zaten kullanılıyor." });
        
        category.Name = dto.Name;
        category.Slug = dto.Slug;
        category.ParentCategoryId = dto.ParentCategoryId;
        category.Description = dto.Description;
        category.ImageUrl = dto.ImageUrl;
        category.OrderIndex = dto.OrderIndex;
        category.IsActive = dto.IsActive;
        category.MetaTitle = dto.MetaTitle;
        category.MetaDescription = dto.MetaDescription;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

        // 🔹 ADMIN: Kategori sil (ya da ileride soft delete yapabiliriz)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task <IActionResult> Delete (int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if(category == null)
            return NotFound( new{message = "Kategori bulunamadı."});
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}