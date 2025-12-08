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
                .ThenInclude(i => i.SellerProduct)
                    .ThenInclude(sp => sp.Seller)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null)
        {
            // Boş sepet dön
            return Ok(new
            {
                Items = new List<CartItemResponseDto>(),
                Summary = new
                {
                    TotalItems = 0,
                    SubTotal = 0m,
                    DiscountApplied = 0m,
                    Total = 0m,
                    AppliedCouponCode = (string?)null
                }
            });
        }

        // Sepet itemlarını DTO'ya çevir
        var items = cart.Items.Select(item => new CartItemResponseDto
        {
            CartItemId = item.CartItemId,
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            ProductImage = item.Product.ImageUrl ?? "",
            ListingId = item.SellerProductId,
            StoreName = item.SellerProduct.Seller?.StoreName ?? "",
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice,
            AvailableStock = item.SellerProduct.Stock,
            IsOutOfStock = item.SellerProduct.Stock == 0 || !item.SellerProduct.IsActive,
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

        return Ok(new { Items = items, Summary = summary });
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
        var sellerProduct = await _context.SellerProducts
            .Include(sp => sp.Product)
            .Include(sp => sp.Seller)
            .FirstOrDefaultAsync(sp => sp.SellerProductId == dto.ListingId);

        if (sellerProduct == null)
            throw new NotFoundException("Satıcı ürünü bulunamadı.");

        if (!sellerProduct.IsActive)
            throw new BadRequestException("Bu ürün satışta değil.");

        if (!sellerProduct.Product.IsActive)
            throw new BadRequestException("Bu ürün katalogdan kaldırılmış.");

        if (sellerProduct.Stock < dto.Quantity)
            throw new BadRequestException($"Yetersiz stok. Mevcut stok: {sellerProduct.Stock}");

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
            i.SellerProductId == dto.ListingId && 
            i.SelectedVariant == dto.SelectedVariant);

        if (existingItem != null)
        {
            // Miktarı artır
            var newQuantity = existingItem.Quantity + dto.Quantity;
            
            if (sellerProduct.Stock < newQuantity)
                throw new BadRequestException($"Yetersiz stok. Mevcut stok: {sellerProduct.Stock}, Sepetteki: {existingItem.Quantity}");

            existingItem.Quantity = newQuantity;
            existingItem.TotalPrice = sellerProduct.UnitPrice * newQuantity;
            existingItem.LastUpdated = DateTime.UtcNow;
        }
        else
        {
            // Yeni item ekle
            var cartItem = new CartItem
            {
                ShoppingCartId = cart.ShoppingCartId,
                ProductId = sellerProduct.ProductId,
                SellerProductId = dto.ListingId,
                Quantity = dto.Quantity,
                UnitPrice = sellerProduct.UnitPrice,
                TotalPrice = sellerProduct.UnitPrice * dto.Quantity,
                SelectedVariant = dto.SelectedVariant,
                IsSelectedForCheckout = true,
                IsOutOfStock = false,
                DateAdded = DateTime.UtcNow
            };
            cart.Items.Add(cartItem);
        }

        cart.LastAccessed = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Ürün sepete eklendi.", cartItemCount = cart.Items.Count });
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
            .Include(ci => ci.SellerProduct)
            .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.ShoppingCart.AppUserId == userId);

        if (cartItem == null)
            throw new NotFoundException("Sepet ürünü bulunamadı.");

        if (cartItem.SellerProduct.Stock < dto.Quantity)
            throw new BadRequestException($"Yetersiz stok. Mevcut stok: {cartItem.SellerProduct.Stock}");

        cartItem.Quantity = dto.Quantity;
        cartItem.TotalPrice = cartItem.UnitPrice * dto.Quantity;
        cartItem.LastUpdated = DateTime.UtcNow;
        cartItem.ShoppingCart.LastAccessed = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Sepet güncellendi." });
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

        return Ok(new { message = "Ürün sepetten çıkarıldı." });
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
            return Ok(new { message = "Sepet zaten boş." });

        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sepet temizlendi." });
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
            return Ok(new { count = 0 });

        var count = cart.Items.Sum(ci => ci.Quantity);

        return Ok(new { count });
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
        cartItem.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Seçim güncellendi.",
            isSelected = cartItem.IsSelectedForCheckout
        });
    }
}
