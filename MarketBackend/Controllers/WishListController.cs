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
public class WishListController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public WishListController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Kullanici favorilerini getir
    [HttpGet]
    public async Task<IActionResult> GetMyWishList()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");
        var wishlistItems = await _context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.Brand)
            .Include(w => w.Product.Listings)
            .Where(w => w.AppUserId == userId)
            .OrderByDescending(w => w.DateAdded)
            .Select(w => new WishlistItemResponseDto
            {
                WishlistItemId = w.WishlistItemId,
                DateAdded = w.DateAdded,
                PriceAtAddition = w.PriceAtAddition,
                SelectedVariant = w.SelectedVariant,

                ProductId = w.Product.ProductId,
                ProductName = w.Product.Name,
                ProductSlug = w.Product.Slug,
                ProductImage = w.Product.ImageUrl,
                BrandName = w.Product.Brand != null ? w.Product.Brand.Name : null,

                // Sadece aktif ve stoklu satıcıların minimum fiyatını hesapla
                CurrentMinPrice = w.Product.Listings.Any(sp => sp.IsActive && sp.Stock > 0)
                    ? w.Product.Listings
                        .Where(sp => sp.IsActive && sp.Stock > 0)
                        .Min(sp => sp.UnitPrice)
                    : (decimal?)null,

                AvailableSellerCount = w.Product.Listings.Count(sp => sp.IsActive && sp.Stock > 0),
                IsAvailable = w.Product.Listings.Any(sp => sp.IsActive && sp.Stock > 0),
                PriceChanged = w.PriceAtAddition.HasValue && 
                              w.Product.Listings.Any(sp => sp.IsActive && sp.Stock > 0) &&
                              w.Product.Listings
                                .Where(sp => sp.IsActive && sp.Stock > 0)
                                .Min(sp => sp.UnitPrice) != w.PriceAtAddition
            })
            .ToListAsync();
        return Ok(ApiResponse<List<WishlistItemResponseDto>>.SuccessResponse(
            wishlistItems,
            "Favorileriniz başarıyla getirildi."
        ));
    }
    // Favorilere ürün ekle
    [HttpPost]
    public async Task<IActionResult> AddToWishList(WishlistAddDto dto)
    {
        var userID = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userID))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");

        // Ürün var mı ve aktif mi?
        var product = await _context.Products
            .Include(p => p.Listings.Where(sp => sp.IsActive && sp.Stock > 0))
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);
        
        if (product == null)
            throw new NotFoundException("Ürün bulunamadı");

        if (!product.IsActive)
            throw new BadRequestException("Bu ürün katalogdan kaldırılmış");
        
        // Zaten favoride mi?
        var existingItem = await _context.WishlistItems
            .AnyAsync(w => w.AppUserId == userID && w.ProductId == dto.ProductId);
        
        if (existingItem)
            throw new ConflictException("Bu ürün zaten favorilerinizde");
        
        // Güncel min fiyat (Satıcı yoksa null, varsa min fiyat)
        var currentMinPrice = product.Listings.Any(sp => sp.IsActive && sp.Stock > 0)
            ? product.Listings
                .Where(sp => sp.IsActive && sp.Stock > 0)
                .Min(s => s.UnitPrice)
            : (decimal?)null;
        
        var wishlistItem = new WishlistItem
        {
            AppUserId = userID,
            ProductId = dto.ProductId,
            DateAdded = DateTime.UtcNow,
            PriceAtAddition = currentMinPrice,
            SelectedVariant = dto.SelectedVariant
        };
        
        _context.WishlistItems.Add(wishlistItem);
        await _context.SaveChangesAsync();
        
        // Mesaj satıcı durumuna göre
        var message = currentMinPrice.HasValue
            ? "Ürün favorilere eklendi"
            : "Ürün favorilere eklendi. Stoğa girdiğinde size bildirim göndereceğiz";
        
        return Ok(ApiResponse<object>.SuccessResponse(
            new
            {
                wishlistItemId = wishlistItem.WishlistItemId,
                isAvailable = currentMinPrice.HasValue
            },
            message
        ));
    }
    // Favorilerden ürün çıkar
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishList(int id)
    {
        var userID = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userID))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");
        var wishlistItem = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.AppUserId == userID && w.ProductId == id);
        if (wishlistItem == null)
            throw new NotFoundException("Favorilerinizde bu ürün bulunamadı.");
        _context.WishlistItems.Remove(wishlistItem);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.SuccessResponse(
            "Ürün favorilerden çıkarıldı."
        ));
    }       
    // Tum Favorileri Temizle
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearWishList()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");
        var count = await _context.WishlistItems
            .Where(w => w.AppUserId == userId)
            .ExecuteDeleteAsync();
        return Ok(ApiResponse<object>.SuccessResponse(
            new { count },
            $"{count} ürün favorilerden çıkarıldı."
        ));   
    }
    // Check is it in Wishlist
    [HttpGet("check/{productId}")]
    public async Task<IActionResult> CheckInWIshlist(int productId)
    {
        var userId = _userManager.GetUserId(User);
        if(string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");
        var exists = await _context.WishlistItems
            .AnyAsync(w => w.AppUserId == userId && w.ProductId == productId);
        return Ok(ApiResponse<object>.SuccessResponse(
            new { IsInWishlist = exists },
            "Favori durumu başarıyla getirildi."
        ));
    }
    // Get Favorite Count
    [HttpGet("count")]
    public async Task<IActionResult> GetWishlistCount()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giris yapmaniz gerekiyor.");
        var count = await _context.WishlistItems
            .CountAsync(w => w.AppUserId == userId);
        return Ok(ApiResponse<object>.SuccessResponse(
            new { Count = count },
            "Favori sayısı başarıyla getirildi."
        ));
    }
}