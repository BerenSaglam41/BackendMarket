using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public PaymentController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Ödeme başlat - Sepetten ödeme işlemi oluştur
    /// </summary>
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment(PaymentInitiateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        // Seller kontrolü
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _userManager.GetRolesAsync(user!);
        if (roles.Contains("Seller"))
            throw new ForbiddenException("Satıcılar ödeme başlatamaz.");

        // Sepeti getir
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items.Where(i => i.IsSelectedForCheckout))
                .ThenInclude(i => i.Product)
            .Include(c => c.Items.Where(i => i.IsSelectedForCheckout))
                .ThenInclude(i => i.Listing)
                    .ThenInclude(sp => sp.Seller)
            .FirstOrDefaultAsync(c => c.AppUserId == userId && c.IsActive);

        if (cart == null || !cart.Items.Any())
            throw new BadRequestException("Sepetiniz boş.");

        // Stok kontrolü
        foreach (var item in cart.Items)
        {
            if (!item.Listing.IsActive)
                throw new BadRequestException($"'{item.Product.Name}' ürünü artık satışta değil.");
            if (item.Listing.Stock < item.Quantity)
                throw new BadRequestException($"'{item.Product.Name}' için yetersiz stok. Mevcut: {item.Listing.Stock}");
        }

        // Adres kontrolü
        Address? shippingAddress;
        Address? billingAddress;

        // Teslimat adresi
        if (dto.ShippingAddressId.HasValue)
        {
            shippingAddress = await _context.Addresses.FindAsync(dto.ShippingAddressId.Value);
            if (shippingAddress == null || shippingAddress.AppUserId != userId)
                throw new BadRequestException("Geçersiz teslimat adresi.");
        }
        else if (dto.ShippingAddress != null)
        {
            shippingAddress = new Address
            {
                AppUserId = userId,
                Title = "Ödeme Teslimat Adresi",
                ContactName = dto.ShippingAddress.ContactName,
                ContactPhone = dto.ShippingAddress.ContactPhone,
                Country = dto.ShippingAddress.Country,
                City = dto.ShippingAddress.City,
                District = dto.ShippingAddress.District,
                Neighborhood = dto.ShippingAddress.Neighborhood,
                FullAddress = dto.ShippingAddress.FullAddress,
                PostalCode = dto.ShippingAddress.PostalCode,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Addresses.Add(shippingAddress);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException("Teslimat adresi gereklidir.");
        }

        // Fatura adresi
        if (dto.BillingAddressId.HasValue)
        {
            billingAddress = await _context.Addresses.FindAsync(dto.BillingAddressId.Value);
            if (billingAddress == null || billingAddress.AppUserId != userId)
                throw new BadRequestException("Geçersiz fatura adresi.");
        }
        else if (dto.BillingAddress != null)
        {
            billingAddress = new Address
            {
                AppUserId = userId,
                Title = "Ödeme Fatura Adresi",
                ContactName = dto.BillingAddress.ContactName,
                ContactPhone = dto.BillingAddress.ContactPhone,
                Country = dto.BillingAddress.Country,
                City = dto.BillingAddress.City,
                District = dto.BillingAddress.District,
                Neighborhood = dto.BillingAddress.Neighborhood,
                FullAddress = dto.BillingAddress.FullAddress,
                PostalCode = dto.BillingAddress.PostalCode,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Addresses.Add(billingAddress);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Fatura adresi belirtilmemişse teslimat adresi kullan
            if (shippingAddress == null)
                throw new BadRequestException("Teslimat adresi bulunamadı.");
            billingAddress = shippingAddress;
        }

        // Fiyat hesaplama
        decimal subtotal = cart.Items.Sum(i => i.TotalPrice);
        decimal taxAmount = subtotal * 0.20m;
        decimal shippingCost = 30m;
        decimal discountAmount = 0m;

        // Kupon kontrolü
        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == dto.CouponCode && c.IsActive);

            if (coupon != null &&
                coupon.ValidFrom <= DateTime.UtcNow &&
                coupon.ValidUntil >= DateTime.UtcNow &&
                (!coupon.MaxUsageCount.HasValue || coupon.CurrentUsageCount < coupon.MaxUsageCount.Value))
            {
                if (subtotal >= coupon.MinimumPurchaseAmount)
                {
                    discountAmount = subtotal * (coupon.DiscountPercentage / 100m);
                }
            }
        }

        decimal totalAmount = subtotal + taxAmount + shippingCost - discountAmount;

        // Sepet snapshot (JSON olarak sakla)
        var cartSnapshot = new
        {
            Items = cart.Items.Select(i => new
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ListingId = i.ListingId,
                SellerId = i.Listing.SellerId,
                SellerStoreName = i.Listing.Seller?.StoreName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountApplied = i.DiscountApplied,
                TotalPrice = i.TotalPrice
            }).ToList(),
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            ShippingCost = shippingCost,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            CouponCode = dto.CouponCode
        };

        // Payment kaydı oluştur
        var payment = new Models.Payment.Payment
        {
            AppUserId = userId,
            PaymentMethod = dto.PaymentMethod!.Value,  // Validator'dan geçtiyse kesinlikle var
            PaymentStatus = PaymentStatus.Pending,
            Amount = totalAmount,
            Currency = "TRY",
            ShippingAddressId = shippingAddress.AddressId,
            BillingAddressId = billingAddress.AddressId,
            CartSnapshotJson = JsonSerializer.Serialize(cartSnapshot),
            CustomerNote = dto.CustomerNote,
            CouponCode = dto.CouponCode,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // Ödeme gateway'ine göre işlem yap
        string? paymentUrl = null;
        string? transactionId = null;

        switch (dto.PaymentMethod!.Value)
        {
            case PaymentMethod.CreditCard:
                // Gerçek entegrasyon: Stripe, iyzico vb.
                // Şimdilik mock response
                paymentUrl = $"https://payment-gateway.com/pay/{payment.PaymentId}";
                transactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";
                payment.PaymentGateway = "MockGateway";
                payment.TransactionId = transactionId;
                break;

            case PaymentMethod.BankTransfer:
                // Banka havalesi bilgileri göster
                paymentUrl = null;
                transactionId = $"BANK-{payment.PaymentId}";
                payment.TransactionId = transactionId;
                break;

            case PaymentMethod.CashOnDelivery:
                // Kapıda ödeme - direkt onaylı
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.CompletedAt = DateTime.UtcNow;
                payment.TransactionId = $"COD-{payment.PaymentId}";
                transactionId = payment.TransactionId;
                
                // Sipariş oluştur
                await CreateOrderFromPayment(payment);
                break;
        }

        await _context.SaveChangesAsync();

        var response = new PaymentResponseDto
        {
            PaymentId = payment.PaymentId,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentGateway = payment.PaymentGateway,
            TransactionId = transactionId,
            PaymentUrl = paymentUrl,
            OrderId = payment.OrderId,
            CreatedAt = payment.CreatedAt
        };

        return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(
            response,
            "Ödeme işlemi başlatıldı",
            201
        ));
    }

    /// <summary>
    /// Ödeme onaylama - Gateway'den dönüş sonrası
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmPayment(PaymentConfirmDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        var payment = await _context.Payments
            .Include(p => p.Order)  // Order relation'ı yükle
            .FirstOrDefaultAsync(p => p.PaymentId == dto.PaymentId && p.AppUserId == userId);

        if (payment == null)
            throw new NotFoundException("Ödeme bulunamadı.");

        // Sadece kendi ödemeni onaylayabilirsin
        if (payment.AppUserId != userId)
            throw new ForbiddenException("Bu ödemeyi onaylama yetkiniz yok.");

        if (payment.PaymentStatus != PaymentStatus.Pending)
            throw new BadRequestException("Bu ödeme zaten işleme alınmış.");

        // TransactionId kontrolü - Gateway'den gelen ID ile eşleşmeli
        if (string.IsNullOrEmpty(payment.TransactionId))
            throw new BadRequestException("Bu ödeme için henüz transaction ID oluşturulmamış.");
        
        if (payment.TransactionId != dto.TransactionId)
            throw new BadRequestException("Transaction ID eşleşmiyor. Geçersiz ödeme onayı.");

        // Gateway'den gelen transaction ID'yi doğrula (gerçek entegrasyonda)
        // Gerçek sistemde burada gateway'e API çağrısı yapılıp ödeme doğrulanır
        payment.PaymentToken = dto.PaymentToken;
        payment.PaymentStatus = PaymentStatus.Paid;
        payment.CompletedAt = DateTime.UtcNow;

        // Sipariş oluştur
        await CreateOrderFromPayment(payment);

        await _context.SaveChangesAsync();

        var response = new PaymentResponseDto
        {
            PaymentId = payment.PaymentId,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            Amount = payment.Amount,
            Currency = payment.Currency,
            TransactionId = payment.TransactionId,
            OrderId = payment.OrderId,
            OrderNumber = payment.Order?.OrderNumber,
            CompletedAt = payment.CompletedAt
        };

        return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(
            response,
            "Ödeme başarıyla tamamlandı, siparişiniz oluşturuldu"
        ));
    }

    /// <summary>
    /// Ödeme başarısız - Gateway'den hata dönüşü
    /// </summary>
    [HttpPost("{id:int}/fail")]
    public async Task<IActionResult> FailPayment(int id, [FromBody] PaymentFailDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == id && p.AppUserId == userId);

        if (payment == null)
            throw new NotFoundException("Ödeme bulunamadı.");

        if (payment.PaymentStatus != PaymentStatus.Pending)
            throw new BadRequestException("Bu ödeme zaten işleme alınmış.");

        payment.PaymentStatus = PaymentStatus.Failed;
        payment.FailedAt = DateTime.UtcNow;
        payment.ErrorMessage = dto.ErrorMessage;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse("Ödeme başarısız olarak işaretlendi"));
    }

    /// <summary>
    /// Payment'tan Order oluştur (ödeme başarılı olduktan sonra)
    /// </summary>
    private async Task CreateOrderFromPayment(Models.Payment.Payment payment)
    {
        // Sepet snapshot'ını deserialize et
        var cartSnapshot = JsonSerializer.Deserialize<CartSnapshotDto>(payment.CartSnapshotJson);
        if (cartSnapshot == null)
            throw new BadRequestException("Sepet bilgisi bulunamadı.");

        // Order number oluştur
        var random = new Random();
        var orderNumber = $"MKT-{DateTime.UtcNow:yyyyMMdd}-{random.Next(1000, 9999)}";

        // Order oluştur
        var order = new Order
        {
            OrderNumber = orderNumber,
            AppUserId = payment.AppUserId,
            ShippingAddressId = payment.ShippingAddressId ?? throw new BadRequestException("Teslimat adresi eksik."),
            BillingAddressId = payment.BillingAddressId!.Value,
            OrderSource = "Web",
            CustomerNote = payment.CustomerNote,
            Subtotal = cartSnapshot.Subtotal,
            TaxAmount = cartSnapshot.TaxAmount,
            DiscountAmount = cartSnapshot.DiscountAmount,
            ShippingCost = cartSnapshot.ShippingCost,
            TotalAmount = cartSnapshot.TotalAmount,
            PaymentMethod = payment.PaymentMethod,
            PaymentStatus = PaymentStatus.Paid,
            PaymentTransactionId = payment.TransactionId,
            OrderStatus = OrderStatus.Processing,  // Ödeme alındı, işleme alındı
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // OrderItem'ları oluştur
        foreach (var item in cartSnapshot.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                ListingId = item.ListingId,
                ProductName = item.ProductName,
                SellerId = item.SellerId,
                SellerStoreName = item.SellerStoreName ?? "",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountApplied = item.DiscountApplied,
                TaxRate = 0.20m,
                TotalPrice = item.TotalPrice
            };

            _context.OrderItems.Add(orderItem);

            // Stok düş
            var listing = await _context.Listings.FindAsync(item.ListingId);
            if (listing != null)
            {
                listing.Stock -= item.Quantity;
            }
        }

        // Kupon kullanımını artır (varsa)
        if (!string.IsNullOrEmpty(payment.CouponCode))
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == payment.CouponCode);
            if (coupon != null)
            {
                coupon.CurrentUsageCount++;
            }
            // Kupon snapshot'ta vardı ama artık silinmişse sorun değil, fiyat zaten kaydedilmiş
        }

        // Payment'a OrderId ekle
        payment.OrderId = order.OrderId;

        // Sepetteki checkout için seçili ürünleri temizle
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items.Where(i => i.IsSelectedForCheckout))
            .FirstOrDefaultAsync(c => c.AppUserId == payment.AppUserId && c.IsActive);

        if (cart != null && cart.Items.Any())
        {
            // Sadece checkout için seçili itemları sil
            _context.CartItems.RemoveRange(cart.Items);
            
            // Sepette başka ürün kalmadıysa, yeni boş sepet oluştur
            var remainingItems = await _context.CartItems
                .CountAsync(ci => ci.ShoppingCartId == cart.ShoppingCartId);
            
            if (remainingItems == 0)
            {
                cart.IsActive = false;
                var newCart = new ShoppingCart
                {
                    AppUserId = payment.AppUserId,
                    IsActive = true,
                    LastAccessed = DateTime.UtcNow
                };
                _context.ShoppingCarts.Add(newCart);
            }
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Kullanıcının ödemelerini listele
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyPayments(int page = 1, int pageSize = 10)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmalısınız.");

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var query = _context.Payments
            .Where(p => p.AppUserId == userId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync();

        var payments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PaymentResponseDto
            {
                PaymentId = p.PaymentId,
                PaymentStatus = p.PaymentStatus,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Currency = p.Currency,
                PaymentGateway = p.PaymentGateway,
                TransactionId = p.TransactionId,
                OrderId = p.OrderId,
                OrderNumber = p.Order != null ? p.Order.OrderNumber : null,
                ErrorMessage = p.ErrorMessage,
                CreatedAt = p.CreatedAt,
                CompletedAt = p.CompletedAt
            })
            .ToListAsync();

        return Ok(PagedApiResponse<List<PaymentResponseDto>>.SuccessResponse(
            payments,
            page,
            pageSize,
            totalCount,
            "Ödemeler başarıyla getirildi"
        ));
    }
}

// Helper DTO for cart snapshot
internal class CartSnapshotDto
{
    public List<CartSnapshotItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
}

internal class CartSnapshotItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ListingId { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public string? SellerStoreName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal TotalPrice { get; set; }
}
