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
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Kullanıcının sepetini getir
    /// GET /api/cart
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        // Kullanıcının sepetini bul veya oluştur
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .Include(c => c.Items)
                .ThenInclude(i => i.Listing)
                    .ThenInclude(sp => sp.Seller)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null)
        {
            // Boş sepet dön
            var emptyCart = new
            {
                Items =new List<CartItemResponseDto>(),
                Summary = new
                {
                    TotalItems = 0,
                    SubTotal = 0m,
                    DiscountApplied = 0m,
                    Total = 0m,
                    AppliedCouponCode = (string?)null
                }
            };
            return Ok(ApiResponse<object>.SuccessResponse(
                emptyCart,
                "Sepet başarıyla getirildi."
            ));
        }

        // Sepet itemlarını DTO'ya çevir
        var items = cart.Items.Select(item => new CartItemResponseDto
        {
            CartItemId = item.CartItemId,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ProductImage = item.Product.ImageUrl ?? "",
            ListingId = item.ListingId,
            StoreName = item.Listing.Seller?.StoreName ?? "",
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice,
            AvailableStock = item.Listing.Stock,
            IsOutOfStock = item.Listing.Stock == 0 || !item.Listing.IsActive,
            IsSelectedForCheckout = item.IsSelectedForCheckout,
            AppliedCouponCode = item.AppliedCouponCode,
            DiscountApplied = item.DiscountApplied
        }).ToList();

        // Sepet özeti
        var summary = new
        {
            TotalItems = items.Count,
            SubTotal = items.Where(i => i.IsSelectedForCheckout).Sum(i => i.UnitPrice * i.Quantity),
            DiscountApplied = cart.DiscountApplied + items.Sum(i => i.DiscountApplied),
            Total = items.Where(i => i.IsSelectedForCheckout).Sum(i => i.TotalPrice) - cart.DiscountApplied,
            AppliedCouponCode = cart.AppliedCouponCode
        };

        var cartData = new { Items = items , Summary = summary };
        return Ok(ApiResponse<object>.SuccessResponse(
            cartData,
            "Sepet başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Sepete ürün ekle
    /// POST /api/cart
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart(CartAddDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        // SellerProduct kontrolü
        var listing = await _context.Listings
            .Include(sp => sp.Product)
            .Include(sp => sp.Seller)
            .FirstOrDefaultAsync(sp => sp.ListingId == dto.ListingId);

        if (listing == null)
            throw new NotFoundException("Satıcı ürünü bulunamadı.");

        if (!listing.IsActive)
            throw new BadRequestException("Bu ürün satışta değil.");

        if (!listing.Product.IsActive)
            throw new BadRequestException("Bu ürün katalogdan kaldırılmış.");

        if (listing.Stock < dto.Quantity)
            throw new BadRequestException($"Yetersiz stok. Mevcut stok: {listing.Stock}");

        // Kullanıcının sepetini bul veya oluştur
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null)
        {
            cart = new ShoppingCart
            {
                AppUserId = userId,
                SessionId = null,  
                IsActive = true,
                LastAccessed = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            };
            _context.ShoppingCarts.Add(cart);
            await _context.SaveChangesAsync();
            
            // Yeni sepet oluşturulduğunda Items boş liste olarak başlatılır
            cart.Items = new List<CartItem>();
        }

        // Aynı ürün zaten sepette mi?
        var existingItem = cart.Items.FirstOrDefault(i => 
            i.ListingId == dto.ListingId && 
            i.SelectedVariant == dto.SelectedVariant);

        if (existingItem != null)
        {
            // Miktarı artır
            var newQuantity = existingItem.Quantity + dto.Quantity;
            
            if (listing.Stock < newQuantity)
                throw new BadRequestException($"Yetersiz stok. Mevcut stok: {listing.Stock}, Sepetteki: {existingItem.Quantity}");

            existingItem.Quantity = newQuantity;
            existingItem.TotalPrice = listing.UnitPrice * newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Yeni item ekle
            var cartItem = new CartItem
            {
                ShoppingCartId = cart.ShoppingCartId,
                ProductId = listing.ProductId,
                ListingId = dto.ListingId,
                Quantity = dto.Quantity,
                UnitPrice = listing.UnitPrice,
                TotalPrice = listing.UnitPrice * dto.Quantity,
                SelectedVariant = dto.SelectedVariant,
                IsSelectedForCheckout = true,
                IsOutOfStock = false,
                DateAdded = DateTime.UtcNow
            };
            cart.Items.Add(cartItem);
        }

        cart.LastAccessed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var responseData = new { CartItemCount = cart.Items.Count };
        return Ok(ApiResponse<object>.SuccessResponse(
            responseData,
            "Ürün sepete eklendi."
        ));
    }

    /// <summary>
    /// Sepetteki ürün miktarını güncelle
    /// PUT /api/cart/{cartItemId}
    /// </summary>
    [HttpPut("{cartItemId:int}")]
    public async Task<IActionResult> UpdateCartItem(int cartItemId, CartUpdateQuantityDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var cartItem = await _context.CartItems
            .Include(ci => ci.ShoppingCart)
            .Include(ci => ci.Listing)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.ShoppingCart.AppUserId == userId);

        if (cartItem == null)
            throw new NotFoundException("Sepet ürünü bulunamadı.");

        if (cartItem.Listing.Stock < dto.Quantity)
            throw new BadRequestException($"Yetersiz stok. Mevcut stok: {cartItem.Listing.Stock}");

        cartItem.Quantity = dto.Quantity;
        cartItem.TotalPrice = cartItem.UnitPrice * dto.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;
        cartItem.ShoppingCart.LastAccessed = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Sepet ürünü başarıyla güncellendi."
        ));
    }

    /// <summary>
    /// Sepetten ürün çıkar
    /// DELETE /api/cart/{cartItemId}
    /// </summary>
    [HttpDelete("{cartItemId:int}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var cartItem = await _context.CartItems
            .Include(ci => ci.ShoppingCart)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.ShoppingCart.AppUserId == userId);

        if (cartItem == null)
            throw new NotFoundException("Sepet ürünü bulunamadı.");

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Ürün sepetten çıkarıldı."
        ));
    }

    /// <summary>
    /// Sepeti tamamen temizle
    /// DELETE /api/cart/clear
    /// </summary>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null || !cart.Items.Any())
            return Ok(ApiResponse.SuccessResponse(
                "Sepet zaten boş."
            ));

        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Sepet başarıyla temizlendi."
        ));
    }

    /// <summary>
    /// Sepetteki ürün sayısını getir
    /// GET /api/cart/count
    /// </summary>
    [HttpGet("count")]
    public async Task<IActionResult> GetCartCount()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        // Kullanıcının aktif sepetini bul
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null)
        {
            var emptyCountData = new { Count = 0 };
            return Ok(ApiResponse<object>.SuccessResponse(
                emptyCountData,
                "Sepetteki Bos."
            ));
        }
        var count = cart.Items.Sum(ci => ci.Quantity);
        var countData = new { Count = count };
        return Ok(ApiResponse<object>.SuccessResponse(
            countData,
            "Sepetteki ürün sayısı başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Sepetteki ürünün checkout seçimini toggle et (aç/kapat)
    /// PUT /api/cart/{cartItemId}/toggle-selection
    /// </summary>
    [HttpPut("{cartItemId:int}/toggle-selection")]
    public async Task<IActionResult> ToggleSelection(int cartItemId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var cartItem = await _context.CartItems
            .Include(ci => ci.ShoppingCart)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.ShoppingCart.AppUserId == userId);

        if (cartItem == null)
            throw new NotFoundException("Sepet ürünü bulunamadı.");

        // Toggle: true -> false, false -> true
        cartItem.IsSelectedForCheckout = !cartItem.IsSelectedForCheckout;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var responseData = new { IsSelected = cartItem.IsSelectedForCheckout };
        return Ok(ApiResponse<object>.SuccessResponse(
            responseData,
            "Sepet ürünü seçim durumu güncellendi."
        ));
    }
}
