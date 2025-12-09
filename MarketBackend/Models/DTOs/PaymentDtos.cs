using MarketBackend.Models.Enums;

namespace MarketBackend.Models.DTOs;

/// <summary>
/// Ödeme başlatma DTO
/// </summary>
public class PaymentInitiateDto
{
    public int? ShippingAddressId { get; set; }
    public ShippingAddressDto? ShippingAddress { get; set; }
    
    public int? BillingAddressId { get; set; }
    public BillingAddressDto? BillingAddress { get; set; }
    
    public PaymentMethod? PaymentMethod { get; set; }  // Nullable - zorunlu validasyon için
    public string? CustomerNote { get; set; }
    public string? CouponCode { get; set; }
}

/// <summary>
/// Ödeme başarısız DTO
/// </summary>
public class PaymentFailDto
{
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Ödeme onaylama DTO (Gateway'den dönüşte)
/// </summary>
public class PaymentConfirmDto
{
    public int PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string? PaymentToken { get; set; }
}

/// <summary>
/// Ödeme response DTO
/// </summary>
public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    
    public string? PaymentGateway { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentUrl { get; set; }  // Ödeme sayfası URL'i (redirect için)
    
    public int? OrderId { get; set; }
    public string? OrderNumber { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
