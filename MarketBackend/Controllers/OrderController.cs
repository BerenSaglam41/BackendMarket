using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public OrderController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrder(OrderCreateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        // Alışveriş sepetini getir
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items.Where(i => i.IsSelectedForCheckout))
                .ThenInclude(i => i.Product)
            .Include(c => c.Items.Where(i => i.IsSelectedForCheckout))
                .ThenInclude((i => i.SellerProduct))
                    .ThenInclude(sp => sp.Seller)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);
        if (cart == null || !cart.Items.Any())
            throw new BadRequestException("Sepetiniz boş.");

        // stok kontrol
        foreach (var item in cart.Items)
        {
            if (!item.SellerProduct.IsActive)
                throw new BadRequestException($"'{item.Product.Name}' ürünü satışta değil.");
            if (item.SellerProduct.Stock < item.Quantity)
                throw new BadRequestException($"'{item.Product.Name}' ürünü için yeterli stok yok. Mevcut stok: {item.SellerProduct.Stock}");
        }
        // Adres Kontrol
        var shipppingAddress = await _context.Addresses.FindAsync(dto.ShippingAddressId);
        if (shipppingAddress == null || shipppingAddress.AppUserId != userId)
            throw new BadRequestException("Geçersiz teslimat adresi.");
        var billingAddressId = dto.BillingAddressId ?? dto.ShippingAddressId;
        var billingAddress = await _context.Addresses.FindAsync(billingAddressId);
        if (billingAddress == null || billingAddress.AppUserId != userId)
            throw new BadRequestException("Geçersiz fatura adresi.");
        // Fiyat
        decimal subtotal = cart.Items.Sum(i => i.TotalPrice);
        decimal taxAmount = subtotal * 0.20m; // %20 KDV
        decimal shippingCost = 30m; // sabit kargo ücreti
        decimal discountAmount = 0m;
        // Kupon
        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == dto.CouponCode && c.IsActive);
            
            if (coupon != null && 
                coupon.ValidFrom <= DateTime.UtcNow && 
                coupon.ValidUntil >= DateTime.UtcNow && 
                coupon.CurrentUsageCount < coupon.MaxUsageCount)
            {
                // Minimum tutar kontrolü
                if (subtotal >= coupon.MinimumPurchaseAmount)
                {
                    // Yüzde indirim hesapla
                    discountAmount = subtotal * (coupon.DiscountPercentage / 100m);
                    
                    // Kupon kullanım sayısını artır
                    coupon.CurrentUsageCount++;
                }
            }
        }
        decimal totalAmount = subtotal + taxAmount + shippingCost - discountAmount;
        // ORder number
        var random = new Random();
        var orderNumber = $"MKT-{DateTime.UtcNow:yyyyMMdd}-{random.Next(1000, 9999)}";
        // 6. Order oluştur
        var order = new Order
        {
            OrderNumber = orderNumber,
            AppUserId = userId,
            ShippingAddressId = dto.ShippingAddressId,
            BillingAddressId = billingAddressId,
            OrderSource = "Web",
            CustomerNote = dto.CustomerNote,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DiscountAmount = discountAmount,
            ShippingCost = shippingCost,
            TotalAmount = totalAmount,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = PaymentStatus.Pending,
            OrderStatus = OrderStatus.AwaitingPayment,
            CreatedAt = DateTime.UtcNow
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        // 7. OrderItem'ları oluştur
        foreach (var cartItem in cart.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = cartItem.ProductId,
                SellerProductId = cartItem.SellerProductId,
                ProductName = cartItem.Product.Name,
                SellerId = cartItem.SellerProduct.SellerId,
                SellerStoreName = cartItem.SellerProduct.Seller?.StoreName ?? "",
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                DiscountApplied = cartItem.DiscountApplied,
                TaxRate = 0.20m,
                TotalPrice = cartItem.TotalPrice
            };

            _context.OrderItems.Add(orderItem);

            // Stok düş
            cartItem.SellerProduct.Stock -= cartItem.Quantity;
        }

        // 8. Sepeti temizle - eski item'ları sil ve yeni aktif sepet oluştur
        _context.CartItems.RemoveRange(cart.Items);
        cart.IsActive = false;
        
        // Yeni boş aktif sepet oluştur
        var newCart = new ShoppingCart
        {
            AppUserId = userId,
            IsActive = true,
            LastAccessed = DateTime.UtcNow
        };
        _context.ShoppingCarts.Add(newCart);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, new
        {
            Message = "Sipariş başarıyla oluşturuldu.",
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            TotalAmount = totalAmount
        });
    }
    // Siparisleri listele
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(int page = 1, int pageSize = 10)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 10;
        if (pageSize > 50) pageSize = 50;
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .Where(o => o.AppUserId == userId)
            .OrderByDescending(o => o.CreatedAt);
        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(new
        {
            Data = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                ShippingAddress = o.ShippingAddress.ToAddressDto(),
                BillingAddress = o.BillingAddress.ToAddressDto(),
                Subtotal = o.Subtotal,
                TaxAmount = o.TaxAmount,
                DiscountAmount = o.DiscountAmount,
                ShippingCost = o.ShippingCost,
                TotalAmount = o.TotalAmount,
                ShippingProvider = o.ShippingProvider,
                TrackingNumber = o.TrackingNumber,
                CustomerNote = o.CustomerNote,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.Product?.ImageUrl ?? "",
                    SellerProductId = i.SellerProductId,
                    SellerStoreName = i.SellerStoreName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountApplied = i.DiscountApplied,
                    TaxRate = i.TaxRate,
                    TotalPrice = i.TotalPrice,
                    TrackingNumber = i.TrackingNumber
                }).ToList(),
                CreatedAt = o.CreatedAt,
                ProcessedAt = o.ProcessedAt,
                ShippedAt = o.ShippedAt,
                DeliveredAt = o.DeliveredAt
            }),
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }
    // ID ile sipariş getir
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            throw new NotFoundException("Sipariş bulunamadı.");

        if (order.AppUserId != userId)
            throw new ForbiddenException("Bu siparişi görüntüleme yetkiniz yok.");
        return Ok(new OrderResponseDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            OrderStatus = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            PaymentMethod = order.PaymentMethod.ToString(),
            ShippingAddress = order.ShippingAddress.ToAddressDto(),
            BillingAddress = order.BillingAddress.ToAddressDto(),
            Subtotal = order.Subtotal,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            ShippingProvider = order.ShippingProvider,
            TrackingNumber = order.TrackingNumber,
            CustomerNote = order.CustomerNote,
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImage = i.Product?.ImageUrl ?? "",
                SellerProductId = i.SellerProductId,
                SellerStoreName = i.SellerStoreName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountApplied = i.DiscountApplied,
                TaxRate = i.TaxRate,
                TotalPrice = i.TotalPrice,
                TrackingNumber = i.TrackingNumber
            }).ToList(),
            CreatedAt = order.CreatedAt,
            ProcessedAt = order.ProcessedAt,
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt
        });
    }
    // Siparisi iptal et
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.SellerProduct)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            throw new NotFoundException("Sipariş bulunamadı.");

        if (order.AppUserId != userId)
            throw new ForbiddenException("Bu siparişi iptal etme yetkiniz yok.");

        if (order.OrderStatus != OrderStatus.AwaitingPayment && order.OrderStatus != OrderStatus.Processing)
            throw new BadRequestException("Bu sipariş artık iptal edilemez.");

        // Stokları geri ekle
        foreach (var item in order.Items)
        {
            item.SellerProduct.Stock += item.Quantity;
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Sipariş iptal edildi." });
    }
    // Tum siparisleri listele admin
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders(string? orderStatus = null, string? paymentStatus = null, int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .Include(o => o.AppUser)
            .AsQueryable();

        // OrderStatus filtresi
        if (!string.IsNullOrEmpty(orderStatus) && Enum.TryParse<OrderStatus>(orderStatus, true, out var parsedOrderStatus))
        {
            query = query.Where(o => o.OrderStatus == parsedOrderStatus);
        }

        // PaymentStatus filtresi
        if (!string.IsNullOrEmpty(paymentStatus) && Enum.TryParse<PaymentStatus>(paymentStatus, true, out var parsedPaymentStatus))
        {
            query = query.Where(o => o.PaymentStatus == parsedPaymentStatus);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(new
        {
            Data = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                ShippingAddress = o.ShippingAddress.ToAddressDto(),
                BillingAddress = o.BillingAddress.ToAddressDto(),
                Subtotal = o.Subtotal,
                TaxAmount = o.TaxAmount,
                DiscountAmount = o.DiscountAmount,
                ShippingCost = o.ShippingCost,
                TotalAmount = o.TotalAmount,
                ShippingProvider = o.ShippingProvider,
                TrackingNumber = o.TrackingNumber,
                CustomerNote = o.CustomerNote,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.Product?.ImageUrl ?? "",
                    SellerProductId = i.SellerProductId,
                    SellerStoreName = i.SellerStoreName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountApplied = i.DiscountApplied,
                    TaxRate = i.TaxRate,
                    TotalPrice = i.TotalPrice,
                    TrackingNumber = i.TrackingNumber
                }).ToList(),
                CreatedAt = o.CreatedAt,
                ProcessedAt = o.ProcessedAt,
                ShippedAt = o.ShippedAt,
                DeliveredAt = o.DeliveredAt
            }),
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }
    // Siparis durum guncelle
        [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderUpdateStatusDto dto)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            throw new NotFoundException("Sipariş bulunamadı.");

        order.OrderStatus = dto.NewStatus;

        if (dto.NewStatus == OrderStatus.Processing && !order.ProcessedAt.HasValue)
        {
            order.ProcessedAt = DateTime.UtcNow;
        }
        else if (dto.NewStatus == OrderStatus.Shipped)
        {
            order.ShippedAt = DateTime.UtcNow;
            order.TrackingNumber = dto.TrackingNumber;
            order.ShippingProvider = dto.ShippingProvider;
        }
        else if (dto.NewStatus == OrderStatus.Delivered)
        {
            order.DeliveredAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Sipariş durumu güncellendi." });
    }
    // Seller siparisleri listele
[HttpGet("seller")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> GetSellerOrders(int page = 1, int pageSize = 20)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        // Seller'ın ürünlerinin bulunduğu siparişleri bul
        var query = _context.Orders
            .Include(o => o.Items.Where(i => i.SellerId == userId))
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingAddress)
            .Where(o => o.Items.Any(i => i.SellerId == userId))
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync();

        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return Ok(new
        {
            Data = orders.Select(o => new OrderResponseDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                PaymentMethod = o.PaymentMethod.ToString(),
                ShippingAddress = o.ShippingAddress.ToAddressDto(),
                BillingAddress = o.BillingAddress.ToAddressDto(),
                Subtotal = o.Items.Sum(i => i.TotalPrice), // Sadece seller'ın ürünlerinin toplamı
                TaxAmount = o.Items.Sum(i => i.TotalPrice * i.TaxRate),
                DiscountAmount = o.Items.Sum(i => i.DiscountApplied),
                ShippingCost = 0m, // Kargo seller bazlı hesaplanabilir
                TotalAmount = o.Items.Sum(i => i.TotalPrice),
                ShippingProvider = o.ShippingProvider,
                TrackingNumber = o.TrackingNumber,
                CustomerNote = o.CustomerNote,
                Items = o.Items.Select(i => new OrderItemResponseDto
                {
                    OrderItemId = i.OrderItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductImage = i.Product?.ImageUrl ?? "",
                    SellerProductId = i.SellerProductId,
                    SellerStoreName = i.SellerStoreName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountApplied = i.DiscountApplied,
                    TaxRate = i.TaxRate,
                    TotalPrice = i.TotalPrice,
                    TrackingNumber = i.TrackingNumber
                }).ToList(),
                CreatedAt = o.CreatedAt,
                ProcessedAt = o.ProcessedAt,
                ShippedAt = o.ShippedAt,
                DeliveredAt = o.DeliveredAt
            }),
            Pagination = new
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            }
        });
    }
}