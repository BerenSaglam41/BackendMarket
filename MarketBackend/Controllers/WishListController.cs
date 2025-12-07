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
            .Include(w => w.Product.SellerProducts.Where(sp => sp.IsActive && sp.Stock > 0))
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

                // UnitPrice yerine OriginalPrice - (OriginalPrice * DiscountPercentage / 100) hesapla
                CurrentMinPrice = w.Product.SellerProducts.Any()
                    ? w.Product.SellerProducts.Min(sp => sp.OriginalPrice - (sp.OriginalPrice * sp.DiscountPercentage / 100))
                    : (decimal?)null,

                AvailableSellerCount = w.Product.SellerProducts.Count(),
                IsAvailable = w.Product.SellerProducts.Any(),
                PriceChanged = w.Product.SellerProducts.Any() &&
                              w.Product.SellerProducts.Min(sp => sp.OriginalPrice - (sp.OriginalPrice * sp.DiscountPercentage / 100)) != w.PriceAtAddition
            })
            .ToListAsync();
        return Ok(wishlistItems);
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
            .Include(p => p.SellerProducts.Where(sp => sp.IsActive && sp.Stock > 0))
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
        
        // Güncel min fiyat (Satıcı yoksa 0, varsa min fiyat)
        var currentMinPrice = product.SellerProducts.Any()
            ? product.SellerProducts.Min(s => s.OriginalPrice - (s.OriginalPrice * s.DiscountPercentage / 100))
            : 0;
        
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
        var message = product.SellerProducts.Any()
            ? "Ürün favorilere eklendi"
            : "Ürün favorilere eklendi. Stoğa girdiğinde size bildirim göndereceğiz";
        
        return Ok(new
        {
            message,
            wishlistItemId = wishlistItem.WishlistItemId,
            isAvailable = product.SellerProducts.Any()
        });
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
        return Ok(new { message = "Ürün favorilerden çıkarıldı." });
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
        return Ok(new { message = $"{count} ürün favorilerden çıkarıldı." });   
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
        return Ok(new { isInWishlist = exists });
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
        return Ok(new { count = count });
    }
}