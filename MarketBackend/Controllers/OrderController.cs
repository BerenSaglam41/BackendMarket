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
    
    // Siparisleri listele
    [HttpGet]
    [Authorize]
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
                .ThenInclude(i => i.Product)
            .Include(o => o.ShippingAddress)
            .Include(o => o.BillingAddress)
            .Where(o => o.AppUserId == userId)
            .OrderByDescending(o => o.CreatedAt);
        var totalCount = await query.CountAsync();
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        var orderDtos = orders.Select(o => new OrderResponseDto
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
                ListingId = i.ListingId,
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
        }).ToList();

        return Ok(PagedApiResponse<List<OrderResponseDto>>.SuccessResponse(
            orderDtos,
            page,
            pageSize,
            totalCount,
            "Siparişler başarıyla getirildi"
        ));
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
            
        var orderDto = new OrderResponseDto
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
                ListingId = i.ListingId,
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
        };
        
        return Ok(ApiResponse<OrderResponseDto>.SuccessResponse(orderDto, "Sipariş detayları getirildi"));
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
                .ThenInclude(i => i.Listing)
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
            item.Listing.Stock += item.Quantity;
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse("Sipariş başarıyla iptal edildi."));
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
                .ThenInclude(i => i.Product)
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
            
        var orderDtos = orders.Select(o => new OrderResponseDto
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
                ListingId = i.ListingId,
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
        }).ToList();
        
        return Ok(PagedApiResponse<List<OrderResponseDto>>.SuccessResponse(
            orderDtos,
            page,
            pageSize,
            totalCount,
            "Tüm siparişler başarıyla getirildi"
        ));
    }
    // Siparis durum guncelle (Seller)
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderUpdateStatusDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == id);
            
        if (order == null)
            throw new NotFoundException("Sipariş bulunamadı.");

        // Seller sadece kendi ürünlerinin bulunduğu siparişleri güncelleyebilir
        if (!order.Items.Any(i => i.SellerId == userId))
            throw new ForbiddenException("Bu siparişi güncelleme yetkiniz yok.");

        // İptal edilmiş siparişin durumu değiştirilemez
        if (order.OrderStatus == OrderStatus.Cancelled)
            throw new BadRequestException("İptal edilmiş siparişin durumu değiştirilemez.");

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

        return Ok(ApiResponse.SuccessResponse("Sipariş durumu başarıyla güncellendi."));
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
            
        var orderDtos = orders.Select(o => new OrderResponseDto
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
                ListingId = i.ListingId,
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
        }).ToList();
        
        return Ok(PagedApiResponse<List<OrderResponseDto>>.SuccessResponse(
            orderDtos,
            page,
            pageSize,
            totalCount,
            "Satıcı siparişleri başarıyla getirildi"
        ));
    }
}