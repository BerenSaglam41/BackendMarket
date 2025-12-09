using MarketBackend.Data;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using MarketBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ListingController> _logger;

    public ListingController(ApplicationDbContext context, ILogger<ListingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/listing - List all active listings (customer homepage)
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllListings(
        [FromQuery] int? categoryId,
        [FromQuery] int? brandId,
        [FromQuery] string? searchTerm,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy = "newest",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest(ApiResponse.ErrorResponse("Sayfa numarası ve sayfa boyutu pozitif olmalıdır"));
        }

        var query = _context.SellerProducts
            .Include(sp => sp.Product)
                .ThenInclude(p => p.Category)
            .Include(sp => sp.Product)
                .ThenInclude(p => p.Brand)
            .Include(sp => sp.Seller)
            .Where(sp => sp.IsActive && sp.Stock > 0)
            .AsQueryable();

        // Filters
        if (categoryId.HasValue)
        {
            query = query.Where(sp => sp.Product.CategoryId == categoryId.Value);
        }

        if (brandId.HasValue)
        {
            query = query.Where(sp => sp.Product.BrandId == brandId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(sp => 
                sp.Product.Name.ToLower().Contains(term) || 
                sp.Product.Description.ToLower().Contains(term) ||
                sp.Product.Brand.Name.ToLower().Contains(term) ||
                sp.Seller.StoreName.ToLower().Contains(term));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(sp => sp.UnitPrice >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(sp => sp.UnitPrice <= maxPrice.Value);
        }

        // Sorting
        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(sp => sp.UnitPrice),
            "price_desc" => query.OrderByDescending(sp => sp.UnitPrice),
            "discount" => query.OrderByDescending(sp => sp.DiscountPercentage),
            "newest" => query.OrderByDescending(sp => sp.CreatedAt),
            _ => query.OrderByDescending(sp => sp.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var listings = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var listingDtos = listings.Select(sp => new ListingResponseDto
        {
            ListingId = sp.SellerProductId,
            Slug = sp.Slug,
            ProductId = sp.ProductId,
            ProductName = sp.Product.Name,
            ProductSlug = sp.Product.Slug,
            ProductDescription = sp.Product.Description,
            ProductImageUrl = sp.Product.ImageUrl,
            
            CategoryId = sp.Product.CategoryId ?? 0,
            CategoryName = sp.Product.Category?.Name ?? string.Empty,
            CategorySlug = sp.Product.Category?.Slug ?? string.Empty,
            
            BrandId = sp.Product.BrandId ?? 0,
            BrandName = sp.Product.Brand?.Name ?? string.Empty,
            BrandSlug = sp.Product.Brand?.Slug ?? string.Empty,
            BrandLogoUrl = sp.Product.Brand?.LogoUrl,
            
            SellerId = sp.SellerId,
            SellerName = sp.Seller.StoreName,
            SellerSlug = sp.Seller.StoreSlug,
            
            OriginalPrice = sp.OriginalPrice,
            DiscountPercentage = sp.DiscountPercentage,
            UnitPrice = sp.UnitPrice,
            Stock = sp.Stock,
            ShippingTimeInDays = sp.ShippingTimeInDays,
            ShippingCost = sp.ShippingCost,
            SellerNote = sp.SellerNote,
            CreatedAt = sp.CreatedAt
        }).ToList();

        return Ok(PagedApiResponse<List<ListingResponseDto>>.SuccessResponse(
            listingDtos,
            page,
            pageSize,
            totalCount,
            "Listing'ler başarıyla getirildi"
        ));
    }

    // GET: api/listing/{slugOrId} - Get single listing with other sellers and recommendations
    [HttpGet("{slugOrId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListingDetail(string slugOrId, [FromQuery] string? userId = null)
    {
        SellerProduct? listing = null;

        // Try to parse as ID first
        if (int.TryParse(slugOrId, out int id))
        {
            listing = await _context.SellerProducts
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Category)
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(sp => sp.Seller)
                .FirstOrDefaultAsync(sp => sp.SellerProductId == id && sp.IsActive);
        }
        else
        {
            // Try to find by slug
            listing = await _context.SellerProducts
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Category)
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(sp => sp.Seller)
                .FirstOrDefaultAsync(sp => sp.Slug == slugOrId && sp.IsActive);
        }

        if (listing == null)
        {
            return NotFound(ApiResponse.ErrorResponse("Listing bulunamadı"));
        }

        // Track view history
        if (!string.IsNullOrEmpty(userId))
        {
            var viewHistory = new ProductViewHistory
            {
                UserId = userId,
                ProductId = listing.ProductId,
                ViewedAt = DateTime.UtcNow,
                ViewDuration = 0, // Will be updated by frontend
                Source = "direct"
            };
            _context.ProductViewHistories.Add(viewHistory);
            await _context.SaveChangesAsync();
        }

        // Get other sellers for the same product
        var otherSellers = await _context.SellerProducts
            .Include(sp => sp.Seller)
            .Where(sp => sp.ProductId == listing.ProductId && 
                         sp.SellerProductId != listing.SellerProductId &&
                         sp.IsActive &&
                         sp.Stock > 0)
            .OrderBy(sp => sp.UnitPrice)
            .Select(sp => new SellerComparisonDto
            {
                ListingId = sp.SellerProductId,
                Slug = sp.Slug,
                SellerId = sp.SellerId,
                SellerName = sp.Seller.StoreName,
                SellerSlug = sp.Seller.StoreSlug,
                UnitPrice = sp.UnitPrice,
                OriginalPrice = sp.OriginalPrice,
                DiscountPercentage = sp.DiscountPercentage,
                Stock = sp.Stock,
                ShippingTimeInDays = sp.ShippingTimeInDays,
                ShippingCost = sp.ShippingCost,
                SellerNote = sp.SellerNote
            })
            .ToListAsync();

        // Get similar products (same category, different product)
        var similarProducts = await _context.SellerProducts
            .Include(sp => sp.Product)
                .ThenInclude(p => p.Brand)
            .Include(sp => sp.Seller)
            .Where(sp => sp.Product.CategoryId == listing.Product.CategoryId &&
                         sp.ProductId != listing.ProductId &&
                         sp.IsActive &&
                         sp.Stock > 0)
            .OrderByDescending(sp => sp.CreatedAt)
            .Take(8)
            .Select(sp => new ListingResponseDto
            {
                ListingId = sp.SellerProductId,
                Slug = sp.Slug,
                ProductId = sp.ProductId,
                ProductName = sp.Product.Name,
                ProductSlug = sp.Product.Slug,
                ProductImageUrl = sp.Product.ImageUrl,
                BrandName = sp.Product.Brand.Name,
                BrandLogoUrl = sp.Product.Brand.LogoUrl,
                SellerName = sp.Seller.StoreName,
                OriginalPrice = sp.OriginalPrice,
                DiscountPercentage = sp.DiscountPercentage,
                UnitPrice = sp.UnitPrice,
                Stock = sp.Stock
            })
            .ToListAsync();

        var detailDto = new ListingDetailResponseDto
        {
            // Current Listing
            ListingId = listing.SellerProductId,
            Slug = listing.Slug,
            ProductId = listing.ProductId,
            ProductName = listing.Product.Name,
            ProductSlug = listing.Product.Slug,
            ProductDescription = listing.Product.Description,
            ProductImageUrl = listing.Product.ImageUrl,
            
            CategoryId = listing.Product.CategoryId ?? 0,
            CategoryName = listing.Product.Category?.Name ?? string.Empty,
            CategorySlug = listing.Product.Category?.Slug ?? string.Empty,
            
            BrandId = listing.Product.BrandId ?? 0,
            BrandName = listing.Product.Brand?.Name ?? string.Empty,
            BrandSlug = listing.Product.Brand?.Slug ?? string.Empty,
            BrandLogoUrl = listing.Product.Brand?.LogoUrl,
            
            SellerId = listing.SellerId,
            SellerName = listing.Seller.StoreName,
            SellerSlug = listing.Seller.StoreSlug,
            
            OriginalPrice = listing.OriginalPrice,
            DiscountPercentage = listing.DiscountPercentage,
            UnitPrice = listing.UnitPrice,
            Stock = listing.Stock,
            ShippingTimeInDays = listing.ShippingTimeInDays,
            ShippingCost = listing.ShippingCost,
            SellerNote = listing.SellerNote,
            CreatedAt = listing.CreatedAt,

            // Other sellers for comparison
            OtherSellers = otherSellers,

            // Similar products
            SimilarProducts = similarProducts
        };

        return Ok(ApiResponse<ListingDetailResponseDto>.SuccessResponse(
            detailDto,
            "Listing detayı başarıyla getirildi"
        ));
    }

    // GET: api/listing/product/{productId}/sellers - Compare all sellers for a specific product
    [HttpGet("product/{productId}/sellers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllSellersForProduct(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
        {
            return NotFound(ApiResponse.ErrorResponse("Ürün bulunamadı"));
        }

        var sellers = await _context.SellerProducts
            .Include(sp => sp.Seller)
            .Where(sp => sp.ProductId == productId && sp.IsActive && sp.Stock > 0)
            .OrderBy(sp => sp.UnitPrice)
            .Select(sp => new SellerComparisonDto
            {
                ListingId = sp.SellerProductId,
                Slug = sp.Slug,
                SellerId = sp.SellerId,
                SellerName = sp.Seller.StoreName,
                SellerSlug = sp.Seller.StoreSlug,
                UnitPrice = sp.UnitPrice,
                OriginalPrice = sp.OriginalPrice,
                DiscountPercentage = sp.DiscountPercentage,
                Stock = sp.Stock,
                ShippingTimeInDays = sp.ShippingTimeInDays,
                ShippingCost = sp.ShippingCost,
                SellerNote = sp.SellerNote
            })
            .ToListAsync();

        var response = new ProductSellerComparisonDto
        {
            ProductId = product.ProductId,
            ProductName = product.Name,
            ProductSlug = product.Slug,
            ProductImageUrl = product.ImageUrl,
            CategoryName = product.Category.Name,
            BrandName = product.Brand.Name,
            BrandLogoUrl = product.Brand.LogoUrl,
            Sellers = sellers
        };

        return Ok(ApiResponse<ProductSellerComparisonDto>.SuccessResponse(
            response,
            "Satıcı karşılaştırması başarıyla getirildi"
        ));
    }

    // GET: api/listing/recommendations - Get personalized recommendations
    [HttpGet("recommendations")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRecommendations(
        [FromQuery] string? userId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] int limit = 12)
    {
        var recommendations = new List<ListingResponseDto>();

        // If user is logged in, get recommendations based on view history
        if (!string.IsNullOrEmpty(userId))
        {
            var viewedProductIds = await _context.ProductViewHistories
                .Where(vh => vh.UserId == userId)
                .OrderByDescending(vh => vh.ViewedAt)
                .Take(10)
                .Select(vh => vh.ProductId)
                .ToListAsync();

            if (viewedProductIds.Any())
            {
                var viewedCategories = await _context.Products
                    .Where(p => viewedProductIds.Contains(p.ProductId))
                    .Select(p => p.CategoryId)
                    .Distinct()
                    .ToListAsync();

                recommendations = await _context.SellerProducts
                    .Include(sp => sp.Product)
                        .ThenInclude(p => p.Brand)
                    .Include(sp => sp.Seller)
                    .Where(sp => viewedCategories.Contains(sp.Product.CategoryId) &&
                                 !viewedProductIds.Contains(sp.ProductId) &&
                                 sp.IsActive &&
                                 sp.Stock > 0)
                    .OrderByDescending(sp => sp.DiscountPercentage)
                    .Take(limit)
                    .Select(sp => new ListingResponseDto
                    {
                        ListingId = sp.SellerProductId,
                        Slug = sp.Slug,
                        ProductId = sp.ProductId,
                        ProductName = sp.Product.Name,
                        ProductSlug = sp.Product.Slug,
                        ProductImageUrl = sp.Product.ImageUrl,
                        BrandName = sp.Product.Brand.Name,
                        BrandLogoUrl = sp.Product.Brand.LogoUrl,
                        SellerName = sp.Seller.StoreName,
                        OriginalPrice = sp.OriginalPrice,
                        DiscountPercentage = sp.DiscountPercentage,
                        UnitPrice = sp.UnitPrice,
                        Stock = sp.Stock
                    })
                    .ToListAsync();
            }
        }

        // If no user-specific recommendations or not enough, get category-based or popular products
        if (recommendations.Count < limit)
        {
            var query = _context.SellerProducts
                .Include(sp => sp.Product)
                    .ThenInclude(p => p.Brand)
                .Include(sp => sp.Seller)
                .Where(sp => sp.IsActive && sp.Stock > 0);

            if (categoryId.HasValue)
            {
                query = query.Where(sp => sp.Product.CategoryId == categoryId.Value);
            }

            var additionalRecommendations = await query
                .OrderByDescending(sp => sp.DiscountPercentage)
                .ThenByDescending(sp => sp.CreatedAt)
                .Take(limit - recommendations.Count)
                .Select(sp => new ListingResponseDto
                {
                    ListingId = sp.SellerProductId,
                    Slug = sp.Slug,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    ProductSlug = sp.Product.Slug,
                    ProductImageUrl = sp.Product.ImageUrl,
                    BrandName = sp.Product.Brand.Name,
                    BrandLogoUrl = sp.Product.Brand.LogoUrl,
                    SellerName = sp.Seller.StoreName,
                    OriginalPrice = sp.OriginalPrice,
                    DiscountPercentage = sp.DiscountPercentage,
                    UnitPrice = sp.UnitPrice,
                    Stock = sp.Stock
                })
                .ToListAsync();

            recommendations.AddRange(additionalRecommendations);
        }

        return Ok(ApiResponse<List<ListingResponseDto>>.SuccessResponse(
            recommendations,
            "Öneriler başarıyla getirildi"
        ));
    }

    // GET: api/listing/category/{categorySlugOrId} - Get listings by category
    [HttpGet("category/{categorySlugOrId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListingsByCategory(
        string categorySlugOrId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        int? categoryId = null;

        if (int.TryParse(categorySlugOrId, out int parsedCategoryId))
        {
            var category = await _context.Categories.FindAsync(parsedCategoryId);
            if (category != null)
            {
                categoryId = category.CategoryId;
            }
        }
        else
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlugOrId);
            if (category != null)
            {
                categoryId = category.CategoryId;
            }
        }

        if (categoryId == null)
        {
            return NotFound(ApiResponse.ErrorResponse("Kategori bulunamadı"));
        }

        return await GetAllListings(categoryId, null, null, null, null, "newest", page, pageSize);
    }

    // GET: api/listing/brand/{brandSlugOrId} - Get listings by brand
    [HttpGet("brand/{brandSlugOrId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListingsByBrand(
        string brandSlugOrId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        int? brandId = null;

        if (int.TryParse(brandSlugOrId, out int parsedBrandId))
        {
            var brand = await _context.Brands.FindAsync(parsedBrandId);
            if (brand != null)
            {
                brandId = brand.BrandId;
            }
        }
        else
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Slug == brandSlugOrId);
            if (brand != null)
            {
                brandId = brand.BrandId;
            }
        }

        if (brandId == null)
        {
            return NotFound(ApiResponse.ErrorResponse("Marka bulunamadı"));
        }

        return await GetAllListings(null, brandId, null, null, null, "newest", page, pageSize);
    }
}
