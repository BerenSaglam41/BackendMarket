using MarketBackend.Models.Enums;

namespace MarketBackend.Models.DTOs;

public class OrderCreateDto
{
    public int ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    public string? CustomerNote { get; set; }
    public string? CouponCode { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
public class OrderItemResponseDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    public int SellerProductId { get; set; }
    public string SellerStoreName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? TrackingNumber { get; set; }
}
public class OrderResponseDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string OrderStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    
    // Adresler
    public AddressResponseDto ShippingAddress { get; set; } = null!;
    public AddressResponseDto BillingAddress { get; set; } = null!;
    
    // Fiyatlar
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    
    // Kargo
    public string? ShippingProvider { get; set; }
    public string? TrackingNumber { get; set; }
    
    public string? CustomerNote { get; set; }
    
    // Items
    public List<OrderItemResponseDto> Items { get; set; } = new();
    
    // Tarihler
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}
// Sipariş durumu güncelleme (Admin/Seller)
public class OrderUpdateStatusDto
{
    public OrderStatus NewStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? ShippingProvider { get; set; }
}

// Helper method - Address to DTO converter
public static class OrderDtoExtensions
{
    public static AddressResponseDto ToAddressDto(this Address address)
    {
        return new AddressResponseDto
        {
            AddressId = address.AddressId,
            Title = address.Title,
            ContactName = address.ContactName,
            ContactPhone = address.ContactPhone,
            Country = address.Country,
            City = address.City,
            District = address.District,
            Neighborhood = address.Neighborhood,
            FullAddress = address.FullAddress,
            PostalCode = address.PostalCode,
            IsDefault = address.IsDefault,
            AddressType = address.AddressType
        };
    }
}