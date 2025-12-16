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

    // Siparisi iptal et (Müşteri)
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
            if (item.Listing != null)
            {
                item.Listing.Stock += item.Quantity;
            }
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse("Sipariş başarıyla iptal edildi."));
    }

    // Tum siparisleri listele (Admin)
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

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateStatusDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        var order = await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Listing)
            .FirstOrDefaultAsync(o => o.OrderId == id);

        if (order == null)
            throw new NotFoundException("Sipariş bulunamadı.");

        // Sahiplik Kontrolü
        if (!order.Items.Any(i => i.SellerId == userId))
            throw new ForbiddenException("Bu sipariş sizin ürünlerinizi içermiyor.");

        // --- HIZLI ÇIKIŞ (PERFORMANS VE HATA ÖNLEME) ---
        // Eğer durum zaten buysa, hiçbir şey yapma ve başarılı dön.
        if (order.OrderStatus == dto.NewStatus)
        {
            return Ok(ApiResponse.SuccessResponse("Sipariş durumu zaten güncel."));
        }

        // Mantıksal Kontrol (Order nesnesini gönderiyoruz)
        ValidateStatusTransition(order, dto.NewStatus);

        // Güncelleme
        order.OrderStatus = dto.NewStatus;

        switch (dto.NewStatus)
        {
            case OrderStatus.Processing:
                if (!order.ProcessedAt.HasValue) 
                    order.ProcessedAt = DateTime.UtcNow;
                break;

            case OrderStatus.Shipped:
                order.ShippedAt = DateTime.UtcNow;
                order.TrackingNumber = dto.TrackingNumber;
                break;

            case OrderStatus.Delivered:
                order.DeliveredAt = DateTime.UtcNow;
                break;

            case OrderStatus.Cancelled:
                order.CancelledAt = DateTime.UtcNow;
                order.CancellationReason = dto.CancellationReason;
                // Stok iadesi
                foreach (var item in order.Items)
                {
                    if (item.Listing != null) item.Listing.Stock += item.Quantity;
                }
                break;

            case OrderStatus.Returned:
                order.CancellationReason = dto.CancellationReason;
                break;
        }

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse($"Sipariş durumu '{dto.NewStatus}' olarak güncellendi."));
    }
/// <summary>
    /// Sipariş durum geçişlerinin mantıklı olup olmadığını kontrol eder.
    /// </summary>
    private void ValidateStatusTransition(Order order, OrderStatus newStatus)
    {
        // 1. AYNI DURUM KONTROLÜ (Hatanızın asıl çözümü burası olabilir)
        // Eğer sipariş zaten "Processing" ise ve tekrar "Processing" isteniyorsa hata verme, işlemden çık.
        if (order.OrderStatus == newStatus) return;

        // 2. İPTAL/İADE KONTROLÜ
        if (order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Returned)
            throw new BadRequestException("İptal edilmiş veya iade alınmış siparişin durumu değiştirilemez.");

        switch (newStatus)
        {
            case OrderStatus.Processing:
                // Normalde sadece "AwaitingPayment"tan "Processing"e geçilir.
                // ANCAK: Eğer sipariş zaten Ödenmiş (Paid) ise ve sistem bir şekilde AwaitingPayment'ta kalmışsa
                // veya durum senkronizasyonu yapılıyorsa izin ver.
                // Hata fırlatma koşulu: Şu anki durum AwaitingPayment DEĞİLSE.
                if (order.OrderStatus != OrderStatus.AwaitingPayment)
                {
                    // Eğer zaten ileride bir aşamadaysa (Shipped, Delivered) geri dönemez.
                    if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Delivered)
                         throw new BadRequestException($"Sipariş '{order.OrderStatus}' aşamasında, geriye dönük 'Hazırlanıyor' yapılamaz.");
                         
                    // Diğer durumlarda (örneğin sistem hatasıyla Paid ama AwaitingPayment değilse) 
                    // burası loglanabilir ama şimdilik strict moda devam edelim.
                    // Hatanın sebebi muhtemelen yukarıdaki "order.OrderStatus == newStatus" kontrolünün eksik olmasıydı.
                }
                break;

            case OrderStatus.Shipped:
                // Sadece "Hazırlanıyor" -> "Kargolandı" olabilir.
                // Eğer sisteminizde "AwaitingPayment"tan direkt "Shipped"e geçiş varsa burayı esnetebilirsiniz.
                if (order.OrderStatus != OrderStatus.Processing)
                    throw new BadRequestException("Sipariş hazırlanmadan (Processing) kargoya verilemez.");
                break;

            case OrderStatus.Delivered:
                if (order.OrderStatus != OrderStatus.Shipped)
                    throw new BadRequestException("Sipariş kargoya verilmeden teslim edildi olarak işaretlenemez.");
                break;

            case OrderStatus.Returned:
                if (order.OrderStatus != OrderStatus.Delivered)
                    throw new BadRequestException("Teslim edilmemiş sipariş iade alınamaz.");
                break;

            case OrderStatus.Cancelled:
                if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Delivered)
                    throw new BadRequestException("Kargoya verilmiş veya teslim edilmiş sipariş iptal edilemez. Lütfen iade sürecini kullanın.");
                break;
        }
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
            // Dikkat: Burada toplam tutar sadece seller'ın kendi ürünlerinin toplamıdır.
            Subtotal = o.Items.Sum(i => i.TotalPrice), 
            TaxAmount = o.Items.Sum(i => i.TotalPrice * i.TaxRate),
            DiscountAmount = o.Items.Sum(i => i.DiscountApplied),
            ShippingCost = 0m, 
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