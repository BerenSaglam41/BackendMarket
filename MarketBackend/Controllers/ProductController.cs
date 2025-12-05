using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public ProductController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // Tum Aktif Urunler
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.CreatedBySeller)
            .Where(p => p.IsAvailable)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        
        return Ok(products.Select(ToDto));   
    }
    
    // Slug ile urun getir
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.CreatedBySeller)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsAvailable);
        
        if (product == null)
            return NotFound(new { message = "Ürün bulunamadı." });

        return Ok(ToDto(product));
    }
    
    // Urun olustur - Sadece Admin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        // Slug benzersiz mi
        bool slugExists = await _context.Products.AnyAsync(p => p.Slug == dto.Slug);
        if (slugExists)
            return BadRequest(new { message = "Bu slug zaten kullanılıyor. Lütfen benzersiz bir slug seçin." });
        
        // Marka Var mi (opsiyonel)
        if (dto.BrandId.HasValue)
        {
            var brand = await _context.Brands.FindAsync(dto.BrandId.Value);
            if (brand == null)
                return BadRequest(new { message = "Belirtilen marka bulunamadı." });
        }

        // Kategori var mı (opsiyonel)
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest(new { message = "Belirtilen kategori bulunamadı." });
        }
        
        var product = new Product
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,

            OriginalPrice = dto.OriginalPrice,
            DiscountPercentage = dto.DiscountPercentage,

            StockQuantity = dto.StockQuantity,
            IsAvailable = dto.IsAvailable,

            ImageUrl = dto.ImageUrl,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBySlug), new { slug = product.Slug }, ToDto(product));
    }
    
    // Update Product - Sadece Admin
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { message = "Ürün bulunamadı." });

        // Slug benzersiz mi?
        bool slugExists = await _context.Products
            .AnyAsync(p => p.Slug == dto.Slug && p.ProductId != id);

        if (slugExists)
            return BadRequest(new { message = "Bu slug başka bir ürün tarafından kullanılıyor." });

        // Marka var mı?
        if (dto.BrandId.HasValue)
        {
            bool brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
            if (!brandExists)
                return BadRequest(new { message = "Belirtilen marka bulunamadı." });
        }
        
        // Kategori var mı?
        if (dto.CategoryId.HasValue)
        {
            bool categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                return BadRequest(new { message = "Geçersiz kategori ID." });
        }
        
        product.Name = dto.Name;
        product.Slug = dto.Slug;
        product.Description = dto.Description;
        product.BrandId = dto.BrandId;
        product.CategoryId = dto.CategoryId;
        product.OriginalPrice = dto.OriginalPrice;
        product.DiscountPercentage = dto.DiscountPercentage;
        product.StockQuantity = dto.StockQuantity;
        product.IsAvailable = dto.IsAvailable;
        product.ImageUrl = dto.ImageUrl;
        product.MetaTitle = dto.MetaTitle;
        product.MetaDescription = dto.MetaDescription;
        product.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    // Delete Product - Sadece Admin
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { message = "Ürün bulunamadı." });

        // Aktif satış kontrolü - Bu ürünü satan seller var mı?
        var activeListings = await _context.SellerProducts
            .Where(sp => sp.ProductId == id && sp.IsActive)
            .CountAsync();

        if (activeListings > 0)
            return BadRequest(new { 
                message = $"Bu ürünü aktif olarak satan {activeListings} satıcı var. Önce satışları kapatın veya ürünü pasife alın.",
                activeListingsCount = activeListings
            });

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    // DTO MAP
    private ProductResponseDto ToDto(Product p)
    {
        return new ProductResponseDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,

            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,

            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,

            OriginalPrice = p.OriginalPrice,
            DiscountPercentage = p.DiscountPercentage,
            UnitPrice = p.UnitPrice,

            StockQuantity = p.StockQuantity,
            IsAvailable = p.IsAvailable,

            ImageUrl = p.ImageUrl,
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            ReviewCount = p.ReviewCount,

            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,

            // Store bilgisi (Seller ürünüyse dolu, Admin ürünüyse null)
            CreatedBySellerId = p.CreatedBySellerId,
            Store = p.CreatedBySeller != null ? new ProductStoreInfoDto
            {
                StoreName = p.CreatedBySeller.StoreName ?? "",
                StoreSlug = p.CreatedBySeller.StoreSlug ?? "",
                StoreLogoUrl = p.CreatedBySeller.StoreLogoUrl,
                IsStoreVerified = p.CreatedBySeller.IsStoreVerified
            } : null
        };
    }
}
