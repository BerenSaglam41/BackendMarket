using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;
using System.Text.Json;

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
    
    /// <summary>
    /// Tüm aktif ürünleri listele (en düşük fiyatla birlikte) - SADECE ADMIN VE SELLER
    /// GET /api/Product
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? brandId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        [FromQuery] string? createdBySellerId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Pagination koruması - negatif veya sıfır değerlerde default'a dön
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Max limit
        
        var query = _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.CreatedBySeller)
            .Include(p => p.SellerProducts.Where(sp => sp.IsActive))
            .Where(p => p.IsActive)
            .AsQueryable();
        
        // Filtreler
        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId);
        
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);
        
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Description!.Contains(search));
        
        // Belirli seller'ın oluşturduğu ürünler
        if (!string.IsNullOrWhiteSpace(createdBySellerId))
            query = query.Where(p => p.CreatedBySellerId == createdBySellerId);
        
        var totalCount = await query.CountAsync();
        
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var productDtos = products.Select(p => ToListDto(p)).ToList();
        
        return Ok(PagedApiResponse<List<ProductResponseDto>>.SuccessResponse(
            productDtos,
            page,
            pageSize,
            totalCount,
            "Ürünler başarıyla getirildi"
        ));
    }
    
    /// <summary>
    /// Ürün detayı - Tüm satıcılarla birlikte
    /// GET /api/Product/{slug} - SADECE ADMIN VE SELLER
    /// </summary>
    [HttpGet("{slug}")]
    [Authorize(Roles = "Admin,Seller")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.CreatedBySeller)
            .Include(p => p.SellerProducts.Where(sp => sp.IsActive))
                .ThenInclude(sp => sp.Seller)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
        
        if (product == null)
            throw new NotFoundException($"'{slug}' slug'ına sahip ürün bulunamadı.");

        var productDto = ToDetailDto(product);
        return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(productDto, "Ürün detayı başarıyla getirildi"));
    }
    
    /// <summary>
    /// Ürün oluştur - Sadece Admin (fiyat/stok yok, sadece tanım)
    /// POST /api/Product
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        // Slug benzersiz mi
        bool slugExists = await _context.Products.AnyAsync(p => p.Slug == dto.Slug);
        if (slugExists)
            throw new ConflictException($"'{dto.Slug}' slug'ı zaten kullanılıyor.");
        
        // Marka Var mi (opsiyonel)
        if (dto.BrandId.HasValue)
        {
            var brand = await _context.Brands.FindAsync(dto.BrandId.Value);
            if (brand == null)
                throw new NotFoundException($"ID: {dto.BrandId} olan marka bulunamadı.");
        }

        // Kategori var mı (opsiyonel)
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException($"ID: {dto.CategoryId} olan kategori bulunamadı.");
        }
        
        var product = new Product
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            ImageGalleryJson = dto.ImageGallery != null ? JsonSerializer.Serialize(dto.ImageGallery) : null,
            MetaTitle = dto.MetaTitle,
            MetaDescription = dto.MetaDescription,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        // Brand ve Category yükle
        await _context.Entry(product).Reference(p => p.Brand).LoadAsync();
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();
        
        var productDto = ToListDto(product);
        return CreatedAtAction(
            nameof(GetBySlug), 
            new { slug = product.Slug }, 
            ApiResponse<ProductResponseDto>.SuccessResponse(productDto, "Ürün başarıyla oluşturuldu", 201)
        );
    }
    
    /// <summary>
    /// Ürün güncelle - Sadece Admin (fiyat/stok yok, sadece tanım)
    /// PUT /api/Product/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            throw new NotFoundException($"ID: {id} olan ürün bulunamadı.");

        // Slug benzersiz mi?
        bool slugExists = await _context.Products
            .AnyAsync(p => p.Slug == dto.Slug && p.ProductId != id);

        if (slugExists)
            throw new ConflictException($"'{dto.Slug}' slug'ı başka bir ürün tarafından kullanılıyor.");

        // Marka var mı?
        if (dto.BrandId.HasValue)
        {
            bool brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException($"ID: {dto.BrandId} olan marka bulunamadı.");
        }
        
        // Kategori var mı?
        if (dto.CategoryId.HasValue)
        {
            bool categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException($"ID: {dto.CategoryId} olan kategori bulunamadı.");
        }
        
        product.Name = dto.Name;
        product.Slug = dto.Slug;
        product.Description = dto.Description;
        product.BrandId = dto.BrandId;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = dto.ImageUrl;
        product.ImageGalleryJson = dto.ImageGallery != null ? JsonSerializer.Serialize(dto.ImageGallery) : null;
        product.MetaTitle = dto.MetaTitle;
        product.MetaDescription = dto.MetaDescription;
        
        // Ürün pasif yapılırsa tüm satış ilanlarını da pasif yap
        if (product.IsActive && !dto.IsActive)
        {
            var sellerProducts = await _context.SellerProducts
                .Where(sp => sp.ProductId == id)
                .ToListAsync();
            
            foreach (var sp in sellerProducts)
            {
                sp.IsActive = false;
            }
        }
        
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse("Ürün başarıyla güncellendi"));
    }
    
    /// <summary>
    /// Ürün sil - Sadece Admin
    /// DELETE /api/Product/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.SellerProducts)
            .Include(p => p.Reviews)
            .Include(p => p.WishlistItems)
            .FirstOrDefaultAsync(p => p.ProductId == id);
            
        if (product == null)
            throw new NotFoundException($"ID: {id} olan ürün bulunamadı.");

        // Aktif satış kontrolü
        var activeListings = product.SellerProducts.Count(sp => sp.IsActive);
        if (activeListings > 0)
            throw new BadRequestException(
                $"Bu ürünü aktif olarak satan {activeListings} satıcı var. Önce satışları kapatın.",
                new List<string> { $"Aktif satış sayısı: {activeListings}" }
            );

        // Manuel silme: Reviews ve WishlistItems (Product'a bağlı)
        _context.Reviews.RemoveRange(product.Reviews);
        _context.WishlistItems.RemoveRange(product.WishlistItems);
        
        // Product silinince Cascade ile otomatik silinecekler:
        // - SellerProducts (Cascade)
        // - CartItems (SellerProduct üzerinden Cascade)
        _context.Products.Remove(product);
        
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse("Ürün başarıyla silindi"));
    }
    
    // ==========================================
    // HELPER METHODS
    // ==========================================
    
    /// <summary>
    /// Liste için DTO (satıcı detayı yok, sadece min fiyat)
    /// </summary>
    private ProductResponseDto ToListDto(Product p)
    {
        var activeSellers = p.SellerProducts?.Where(sp => sp.IsActive).ToList() ?? new List<SellerProduct>();
        
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

            ImageUrl = p.ImageUrl,
            ImageGallery = ParseImageGallery(p.ImageGalleryJson),
            
            MetaTitle = p.MetaTitle,
            MetaDescription = p.MetaDescription,
            
            IsActive = p.IsActive,
            ReviewCount = p.ReviewCount,

            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,

            CreatedBySellerId = p.CreatedBySellerId,
            Store = p.CreatedBySeller != null ? new ProductStoreInfoDto
            {
                StoreName = p.CreatedBySeller.StoreName ?? "",
                StoreSlug = p.CreatedBySeller.StoreSlug ?? "",
                StoreLogoUrl = p.CreatedBySeller.StoreLogoUrl,
                IsStoreVerified = p.CreatedBySeller.IsStoreVerified
            } : null,
            
            // Satıcı özeti
            MinPrice = activeSellers.Any() ? activeSellers.Min(sp => sp.UnitPrice) : null,
            SellerCount = activeSellers.Count,
            Sellers = null  // Liste'de satıcı detayı yok
        };
    }
    
    /// <summary>
    /// Detay için DTO (tüm satıcılarla birlikte)
    /// </summary>
    private ProductResponseDto ToDetailDto(Product p)
    {
        var dto = ToListDto(p);
        
        // Satıcıları ekle
        dto.Sellers = p.SellerProducts?
            .Where(sp => sp.IsActive)
            .OrderBy(sp => sp.UnitPrice)
            .Select(sp => new ProductSellerDto
            {
                ListingId = sp.SellerProductId,
                SellerId = sp.SellerId,
                StoreName = sp.Seller?.StoreName ?? "",
                StoreSlug = sp.Seller?.StoreSlug ?? "",
                IsStoreVerified = sp.Seller?.IsStoreVerified ?? false,
                OriginalPrice = sp.OriginalPrice,
                DiscountPercentage = sp.DiscountPercentage,
                UnitPrice = sp.UnitPrice,
                Stock = sp.Stock,
                ShippingTimeInDays = sp.ShippingTimeInDays,
                ShippingCost = sp.ShippingCost
            })
            .ToList();
        
        return dto;
    }
    
    private List<string>? ParseImageGallery(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
    }
}
