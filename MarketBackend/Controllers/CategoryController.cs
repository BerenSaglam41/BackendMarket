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

        var result = categories.Select(c => ToCategoryDto(c)).ToList();
        return Ok(ApiResponse<List<CategoryResponseDto>>.SuccessResponse(
            result,
            "Kategoriler başarıyla getirildi."
        ));

    }
    // 🔹 PUBLIC: Slug ile tek kategori getir
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

        if (category == null)
            throw new NotFoundException($"'{slug}' slug'ına sahip kategori bulunamadı.");

        var dto = ToCategoryDto(category);
        return Ok(ApiResponse<CategoryResponseDto>.SuccessResponse(
            dto,
            "Kategori başarıyla getirildi."
        ));
    }
    [HttpGet("tree")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTree()
    {
        var categories = await _context.Categories
            .Include(c => c.SubCategories)
            .Where(c => c.IsActive)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync();
        var root = categories
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.OrderIndex)
            .ToList();
        CategoryTreeDto BuildTree(Category c)
        {
            return new CategoryTreeDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Slug = c.Slug,
                ImageUrl = c.ImageUrl,
                OrderIndex = c.OrderIndex,
                IsActive = c.IsActive,
                Children = c.SubCategories
                    .Where(sc => sc.IsActive)
                    .OrderBy(sc => sc.OrderIndex)
                    .Select(sc => BuildTree(sc))
                    .ToList()
            };
        }
        var tree = root.Select(c => BuildTree(c)).ToList();
        return Ok(ApiResponse<List<CategoryTreeDto>>.SuccessResponse(
            tree,
            "Kategori ağacı başarıyla getirildi."
        ));
    }
    // 🔹 ADMIN: Kategori oluştur
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        // Slug Benzersiz olcak
        var exists = await _context.Categories.AnyAsync(c => c.Slug == dto.Slug);

        if (exists)
            throw new ConflictException($"'{dto.Slug}' slug'ı zaten kullanılıyor.");

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
        var responseDto = ToCategoryDto(category);
        return CreatedAtAction(
            nameof(GetBySlug),
            new { slug = category.Slug },
            ApiResponse<CategoryResponseDto>.SuccessResponse(
                responseDto,
                "Kategori başarıyla oluşturuldu.",
                201
            )
        );
    }

    // 🔹 ADMIN: Kategori güncelle
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            throw new NotFoundException($"ID: {id} olan kategori bulunamadı.");

        // Slug Cakismasi
        var slugExists = await _context.Categories
            .AnyAsync(c => c.Slug == dto.Slug && c.CategoryId != id);
        if (slugExists)
            throw new ConflictException($"'{dto.Slug}' slug'ı başka bir kategori tarafından kullanılıyor.");

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

        return Ok(ApiResponse.SuccessResponse(
            "Kategori başarıyla güncellendi."
        ));
    }

    // 🔹 ADMIN: Kategori sil (ya da ileride soft delete yapabiliriz)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            throw new NotFoundException($"ID: {id} olan kategori bulunamadı.");

        // Kategoriye ait ürün var mı kontrol et
        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts)
            throw new BadRequestException($"Bu kategoriye ait ürünler bulunduğu için silinemez. Önce ürünleri silin veya başka bir kategoriye atayın.");

        // Alt kategoriler var mı kontrol et
        var hasSubCategories = await _context.Categories.AnyAsync(c => c.ParentCategoryId == id);
        if (hasSubCategories)
            throw new BadRequestException($"Bu kategorinin alt kategorileri bulunduğu için silinemez. Önce alt kategorileri silin.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse(
            "Kategori başarıyla silindi."
        ));
    }
    public static CategoryResponseDto ToCategoryDto(Category c)
    {
        return new CategoryResponseDto
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
        };
    }
}