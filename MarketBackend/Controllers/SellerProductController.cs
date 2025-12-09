using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using System.Text.Json;

namespace MarketBackend.Controllers;

/// <summary>
/// Seller'ların ürün önerisi ve satış yönetimi
/// Admin de bu endpoint'lere erişebilir (yönetim amaçlı)
/// </summary>
[ApiController]
[Route("api/seller")]
[Authorize(Roles = "Seller,Admin")]
public class SellerProductController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public SellerProductController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    [HttpGet("products")]
    public async Task<IActionResult> GetMyPendingProducts(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Pagination koruması
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.ProductPendings
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Where(p => p.SellerId == user.Id);

        // Status filtresi
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<PendingStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = products.Select(p => ToSellerResponseDto(p)).ToList();
        
        return Ok(PagedApiResponse<List<SellerProductResponseDto>>.SuccessResponse(
            response,
            page,
            pageSize,
            totalCount,
            "Ürün önerileriniz başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Seller'ın tek bir ürün önerisini getirir
    /// GET /api/seller/products/{id}
    /// </summary>
    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> GetMyPendingProduct(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var product = await _context.ProductPendings
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductPendingId == id && p.SellerId == user.Id);

        if (product == null)
            throw new NotFoundException($"ID '{id}' ile ürün önerisi bulunamadı.");

        var dto = ToSellerResponseDto(product);
        return Ok(ApiResponse<SellerProductResponseDto>.SuccessResponse(
            dto,
            "Ürün önerisi başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Yeni ürün önerisi oluşturur (Admin onayına gider)
    /// POST /api/seller/products
    /// </summary>
    [HttpPost("products")]
    public async Task<IActionResult> CreatePendingProduct(SellerProductCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Slug benzersizlik kontrolü (hem Product hem ProductPending'de)
        bool slugExistsInProducts = await _context.Products.AnyAsync(p => p.Slug == dto.Slug);
        bool slugExistsInPending = await _context.ProductPendings.AnyAsync(p => p.Slug == dto.Slug);
        
        if (slugExistsInProducts || slugExistsInPending)
            throw new ConflictException($"'{dto.Slug}' slug'ı zaten kullanılıyor. Lütfen benzersiz bir slug seçin.");

        // Brand kontrolü (opsiyonel)
        if (dto.BrandId.HasValue)
        {
            var brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException($"ID '{dto.BrandId}' ile marka bulunamadı.");
        }

        // Category kontrolü (opsiyonel)
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException($"ID '{dto.CategoryId}' ile kategori bulunamadı.");
        }

        var pending = new ProductPending
        {
            SellerId = user.Id,
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            SellerCategorySuggestion = dto.SellerCategorySuggestion,
            ImageUrl = dto.ImageUrl,
            ImageGalleryJson = dto.ImageGallery != null ? JsonSerializer.Serialize(dto.ImageGallery) : null,
            SellerSku = dto.SellerSku,
            Barcode = dto.Barcode,
            AttributesJson = dto.AttributesJson,
            SellerNote = dto.SellerNote,
            ProposedPrice = dto.ProposedPrice,
            ProposedStock = dto.ProposedStock,
            ShippingTimeInDays = dto.ShippingTimeInDays,
            Status = PendingStatus.Waiting,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductPendings.Add(pending);
        await _context.SaveChangesAsync();

        // Brand ve Category'yi yükle (response için)
        await _context.Entry(pending).Reference(p => p.Brand).LoadAsync();
        await _context.Entry(pending).Reference(p => p.Category).LoadAsync();

        var responseDto = ToSellerResponseDto(pending);
        return CreatedAtAction(
            nameof(GetMyPendingProduct), 
            new { id = pending.ProductPendingId }, 
            ApiResponse<SellerProductResponseDto>.SuccessResponse(
                responseDto,
                "Ürün öneriniz oluşturuldu. Admin onayından sonra yayınlanacaktır.",
                201
            )
        );
    }

    /// <summary>
    /// Ürün önerisini günceller (Sadece Waiting veya NeedsUpdate durumunda)
    /// PUT /api/seller/products/{id}
    /// </summary>
    [HttpPut("products/{id:int}")]
    public async Task<IActionResult> UpdatePendingProduct(int id, SellerProductUpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var pending = await _context.ProductPendings
            .FirstOrDefaultAsync(p => p.ProductPendingId == id && p.SellerId == user.Id);

        if (pending == null)
            throw new NotFoundException($"ID '{id}' ile ürün önerisi bulunamadı.");

        // Sadece Waiting veya NeedsUpdate durumunda güncellenebilir
        if (pending.Status != PendingStatus.Waiting && pending.Status != PendingStatus.NeedsUpdate)
            throw new BadRequestException("Bu ürün önerisi artık güncellenemez.");

        // Slug benzersizlik kontrolü (kendisi hariç)
        bool slugExistsInProducts = await _context.Products.AnyAsync(p => p.Slug == dto.Slug);
        bool slugExistsInPending = await _context.ProductPendings
            .AnyAsync(p => p.Slug == dto.Slug && p.ProductPendingId != id);

        if (slugExistsInProducts || slugExistsInPending)
            throw new ConflictException($"'{dto.Slug}' slug'ı zaten kullanılıyor.");

        // Brand kontrolü
        if (dto.BrandId.HasValue)
        {
            var brandExists = await _context.Brands.AnyAsync(b => b.BrandId == dto.BrandId);
            if (!brandExists)
                throw new NotFoundException($"ID '{dto.BrandId}' ile marka bulunamadı.");
        }

        // Category kontrolü
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
                throw new NotFoundException($"ID '{dto.CategoryId}' ile kategori bulunamadı.");
        }

        // Güncelle
        pending.Name = dto.Name;
        pending.Slug = dto.Slug;
        pending.Description = dto.Description;
        pending.BrandId = dto.BrandId;
        pending.CategoryId = dto.CategoryId;
        pending.SellerCategorySuggestion = dto.SellerCategorySuggestion;
        pending.ImageUrl = dto.ImageUrl;
        pending.UpdatedAt = DateTime.UtcNow;
        pending.ImageGalleryJson = dto.ImageGallery != null ? JsonSerializer.Serialize(dto.ImageGallery) : null;
        pending.SellerSku = dto.SellerSku;
        pending.Barcode = dto.Barcode;
        pending.AttributesJson = dto.AttributesJson;
        pending.SellerNote = dto.SellerNote;
        pending.ProposedPrice = dto.ProposedPrice;
        pending.ProposedStock = dto.ProposedStock;
        pending.ShippingTimeInDays = dto.ShippingTimeInDays;

        // NeedsUpdate durumundaysa tekrar Waiting'e al
        if (pending.Status == PendingStatus.NeedsUpdate)
        {
            pending.Status = PendingStatus.Waiting;
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Ürün öneriniz başarıyla güncellendi."
        ));
    }

    /// <summary>
    /// Ürün önerisini siler (Sadece Waiting veya NeedsUpdate durumunda)
    /// DELETE /api/seller/products/{id}
    /// </summary>
    [HttpDelete("products/{id:int}")]
    public async Task<IActionResult> DeletePendingProduct(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var pending = await _context.ProductPendings
            .FirstOrDefaultAsync(p => p.ProductPendingId == id && p.SellerId == user.Id);

        if (pending == null)
            throw new NotFoundException($"ID '{id}' ile ürün önerisi bulunamadı.");

        // Sadece Waiting veya NeedsUpdate durumunda silinebilir
        if (pending.Status != PendingStatus.Waiting && pending.Status != PendingStatus.NeedsUpdate)
            throw new BadRequestException("Bu ürün önerisi artık silinemez.");

        _context.ProductPendings.Remove(pending);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Ürün öneriniz başarıyla silindi."
        ));
    }

    // ==========================================
    // AKTİF SATIŞLAR (SellerProduct) İŞLEMLERİ
    // ==========================================

    /// <summary>
    /// Seller'ın tüm aktif satışlarını listeler
    /// GET /api/seller/listings
    /// </summary>
    [HttpGet("listings")]
    public async Task<IActionResult> GetMyListings(
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Pagination koruması
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.SellerProducts
            .Include(sp => sp.Product)
            .Where(sp => sp.SellerId == user.Id);

        if (isActive.HasValue)
        {
            query = query.Where(sp => sp.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync();

        var listings = await query
            .OrderByDescending(sp => sp.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = listings.Select(sp => new SellerListingResponseDto
        {
            ListingId = sp.SellerProductId,
            ProductId = sp.ProductId,
            ProductName = sp.Product.Name,
            ProductSlug = sp.Product.Slug,
            ProductImageUrl = sp.Product.ImageUrl,
            OriginalPrice = sp.OriginalPrice,
            DiscountPercentage = sp.DiscountPercentage,
            UnitPrice = sp.UnitPrice,
            Stock = sp.Stock,
            ShippingTimeInDays = sp.ShippingTimeInDays,
            ShippingCost = sp.ShippingCost,
            SellerNote = sp.SellerNote,
            IsActive = sp.IsActive,
            CreatedAt = sp.CreatedAt
        }).ToList();

        return Ok(PagedApiResponse<List<SellerListingResponseDto>>.SuccessResponse(
            response,
            page,
            pageSize,
            totalCount,
            "Satışlarınız başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Mevcut bir ürünü satışa sunar
    /// POST /api/seller/listings
    /// </summary>
    [HttpPost("listings")]
    public async Task<IActionResult> CreateListing(SellerListingCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Ürün var mı ve aktif mi?
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            throw new NotFoundException($"ID '{dto.ProductId}' ile ürün bulunamadı.");

        if (!product.IsActive)
            throw new BadRequestException("Bu ürün şu an aktif değil.");

        // Kullanıcının rollerini al
        var userRoles = await _userManager.GetRolesAsync(user);
        var isAdmin = userRoles.Contains("Admin");

        // Admin istisna: test/yönetim amaçlı herhangi bir ürünü satışa sunabilir
        if (!isAdmin && product.CreatedBySellerId != null && product.CreatedBySellerId != user.Id)
        {
            throw new ForbiddenException("Ürün başka bir satıcıya ait. Sadece kendi ürünlerinizi veya genel katalogdaki ürünleri satabilirsiniz.");
        }

        // Aynı ürünü zaten satıyor mu?
        var existingListing = await _context.SellerProducts
            .AnyAsync(sp => sp.SellerId == user.Id && sp.ProductId == dto.ProductId);

        if (existingListing)
            throw new ConflictException($"ID '{dto.ProductId}' ürününü zaten satıyorsunuz.");

        // Generate unique slug: product-slug-seller-slug (e.g., "iphone-15-pro-techstore")
        var slug = await GenerateListingSlug(product.Slug, user.StoreSlug);

        var listing = new SellerProduct
        {
            SellerId = user.Id,
            ProductId = dto.ProductId,
            Slug = slug,
            OriginalPrice = dto.OriginalPrice,
            DiscountPercentage = dto.DiscountPercentage,
            Stock = dto.Stock,
            ShippingTimeInDays = dto.ShippingTimeInDays,
            ShippingCost = dto.ShippingCost,
            SellerNote = dto.SellerNote,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SellerProducts.Add(listing);
        await _context.SaveChangesAsync();

        var responseDto = new SellerListingResponseDto
        {
            ListingId = listing.SellerProductId,
            ProductId = listing.ProductId,
            ProductName = product.Name,
            ProductSlug = product.Slug,
            ProductImageUrl = product.ImageUrl,
            OriginalPrice = listing.OriginalPrice,
            DiscountPercentage = listing.DiscountPercentage,
            UnitPrice = listing.UnitPrice,
            Stock = listing.Stock,
            ShippingTimeInDays = listing.ShippingTimeInDays,
            ShippingCost = listing.ShippingCost,
            SellerNote = listing.SellerNote,
            IsActive = listing.IsActive,
            CreatedAt = listing.CreatedAt
        };

        return CreatedAtAction(
            nameof(GetMyListings), 
            new { id = listing.SellerProductId }, 
            ApiResponse<SellerListingResponseDto>.SuccessResponse(
                responseDto,
                "Satış başarıyla oluşturuldu.",
                201
            )
        );
    }

    /// <summary>
    /// Satışı günceller (fiyat, stok, kargo vs.)
    /// PUT /api/seller/listings/{id}
    /// </summary>
    [HttpPut("listings/{id:int}")]
    public async Task<IActionResult> UpdateListing(int id, SellerListingUpdateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var listing = await _context.SellerProducts
            .FirstOrDefaultAsync(sp => sp.SellerProductId == id && sp.SellerId == user.Id);

        if (listing == null)
            throw new NotFoundException($"ID '{id}' ile satış bulunamadı.");

        listing.OriginalPrice = dto.OriginalPrice;
        listing.DiscountPercentage = dto.DiscountPercentage;
        listing.Stock = dto.Stock;
        listing.ShippingTimeInDays = dto.ShippingTimeInDays;
        listing.ShippingCost = dto.ShippingCost;
        listing.SellerNote = dto.SellerNote;
        listing.IsActive = dto.IsActive;
        listing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Satış başarıyla güncellendi."
        ));
    }

    /// <summary>
    /// Satışı kaldırır
    /// DELETE /api/seller/listings/{id}
    /// </summary>
    [HttpDelete("listings/{id:int}")]
    public async Task<IActionResult> DeleteListing(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var listing = await _context.SellerProducts
            .FirstOrDefaultAsync(sp => sp.SellerProductId == id && sp.SellerId == user.Id);

        if (listing == null)
            throw new NotFoundException($"ID '{id}' ile satış bulunamadı.");

        _context.SellerProducts.Remove(listing);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Satış başarıyla kaldırıldı."
        ));
    }

    // ==========================================
    // DASHBOARD & İSTATİSTİKLER
    // ==========================================

    /// <summary>
    /// Seller dashboard özet bilgileri
    /// GET /api/seller/dashboard
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        // Pending ürün sayıları
        var pendingCounts = await _context.ProductPendings
            .Where(p => p.SellerId == user.Id)
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // Aktif satış sayısı
        var activeListings = await _context.SellerProducts
            .CountAsync(sp => sp.SellerId == user.Id && sp.IsActive);

        var totalListings = await _context.SellerProducts
            .CountAsync(sp => sp.SellerId == user.Id);

        var dashboardData = new
        {
            PendingProducts = new
            {
                Waiting = pendingCounts.FirstOrDefault(c => c.Status == PendingStatus.Waiting)?.Count ?? 0,
                Approved = pendingCounts.FirstOrDefault(c => c.Status == PendingStatus.Approved)?.Count ?? 0,
                Rejected = pendingCounts.FirstOrDefault(c => c.Status == PendingStatus.Rejected)?.Count ?? 0,
                NeedsUpdate = pendingCounts.FirstOrDefault(c => c.Status == PendingStatus.NeedsUpdate)?.Count ?? 0
            },
            Listings = new
            {
                Active = activeListings,
                Total = totalListings
            }
        };

        return Ok(ApiResponse<object>.SuccessResponse(
            dashboardData,
            "Dashboard verileri başarıyla getirildi."
        ));
    }

    // ==========================================
    // HELPER METHODS
    // ==========================================

    private static SellerProductResponseDto ToSellerResponseDto(ProductPending p)
    {
        List<string>? imageGallery = null;
        if (!string.IsNullOrEmpty(p.ImageGalleryJson))
        {
            try
            {
                imageGallery = JsonSerializer.Deserialize<List<string>>(p.ImageGalleryJson);
            }
            catch { }
        }

        return new SellerProductResponseDto
        {
            ProductPendingId = p.ProductPendingId,
            Name = p.Name,
            Slug = p.Slug,
            Description = p.Description,
            BrandId = p.BrandId,
            BrandName = p.Brand?.Name,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.Name,
            SellerCategorySuggestion = p.SellerCategorySuggestion,
            ImageUrl = p.ImageUrl,
            ImageGallery = imageGallery,
            SellerSku = p.SellerSku,
            Barcode = p.Barcode,
            AttributesJson = p.AttributesJson,
            SellerNote = p.SellerNote,
            ProposedPrice = p.ProposedPrice,
            ProposedStock = p.ProposedStock,
            ShippingTimeInDays = p.ShippingTimeInDays,
            Status = p.Status.ToString(),
            AdminNote = p.AdminNote,
            ReviewedAt = p.ReviewedAt,
            CreatedAt = p.CreatedAt
        };
    }

    // ==========================================
    // SELLER ÜRÜN YÖNETİMİ (Onaylanmış Product)
    // ==========================================

    /// <summary>
    /// Seller kendi oluşturduğu ürünü pasif/aktif yapabilir
    /// PUT /api/seller/my-product/{id}/toggle
    /// </summary>
    [HttpPut("my-product/{id:int}/toggle")]
    public async Task<IActionResult> ToggleMyProduct(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == id && p.CreatedBySellerId == user.Id);

        if (product == null)
            throw new NotFoundException($"ID: {id} olan ürün bulunamadı veya size ait değil.");

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            new { isActive = product.IsActive },
            product.IsActive ? "Ürün aktif edildi." : "Ürün pasif edildi."
        ));
    }

    /// <summary>
    /// Seller'ın satışa sunduğu tüm ürünleri listeler (SellerProducts)
    /// Bu endpoint /api/seller/listings ile aynı işi yapar - satışları listeler
    /// GET /api/seller/my-products
    /// </summary>
    [HttpGet("my-products")]
    public async Task<IActionResult> GetMyProducts(
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            throw new UnauthorizedException("Kullanıcı bulunamadı.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        // Seller'ın kendi satışları (SellerProducts) - listings ile aynı
        var query = _context.SellerProducts
            .Include(sp => sp.Product)
                .ThenInclude(p => p.Brand)
            .Include(sp => sp.Product)
                .ThenInclude(p => p.Category)
            .Where(sp => sp.SellerId == user.Id);

        if (isActive.HasValue)
            query = query.Where(sp => sp.IsActive == isActive.Value);

        var totalCount = await query.CountAsync();
        var listings = await query
            .OrderByDescending(sp => sp.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = listings.Select(sp => new SellerListingResponseDto
        {
            ListingId = sp.SellerProductId,
            ProductId = sp.ProductId,
            ProductName = sp.Product.Name,
            ProductSlug = sp.Product.Slug,
            ProductImageUrl = sp.Product.ImageUrl,
            OriginalPrice = sp.OriginalPrice,
            DiscountPercentage = sp.DiscountPercentage,
            UnitPrice = sp.UnitPrice,
            Stock = sp.Stock,
            ShippingTimeInDays = sp.ShippingTimeInDays,
            ShippingCost = sp.ShippingCost,
            SellerNote = sp.SellerNote,
            IsActive = sp.IsActive,
            CreatedAt = sp.CreatedAt
        }).ToList();

        return Ok(PagedApiResponse<List<SellerListingResponseDto>>.SuccessResponse(
            response,
            page,
            pageSize,
            totalCount,
            "Satışlarınız başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Generates a unique slug for a listing combining product slug and seller slug
    /// Format: {product-slug}-{seller-slug} (e.g., "iphone-15-pro-techstore")
    /// If conflict exists, appends a number (e.g., "iphone-15-pro-techstore-2")
    /// </summary>
    private async Task<string> GenerateListingSlug(string productSlug, string sellerSlug)
    {
        var baseSlug = $"{productSlug}-{sellerSlug}";
        var slug = baseSlug;
        var counter = 1;

        // Check for conflicts and append number if needed
        while (await _context.SellerProducts.AnyAsync(sp => sp.Slug == slug))
        {
            counter++;
            slug = $"{baseSlug}-{counter}";
        }

        return slug;
    }
}
