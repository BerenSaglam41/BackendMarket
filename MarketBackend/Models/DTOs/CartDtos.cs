public class CartAddDto
{
    public int ListingId {get;set;}  // SellerProduct (Listing) ID
    public int Quantity {get;set;}
    public string? SelectedVariant {get;set;}
}

public class CartItemResponseDto
{
    // Ürün bilgileri
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
    
    // Satıcı bilgileri
    public int ListingId { get; set; }  // SellerProduct (Listing) ID
    public string StoreName { get; set; } = string.Empty;
    
    // Fiyat bilgileri
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Stok durumu
    public int AvailableStock { get; set; }
    public bool IsOutOfStock { get; set; }
    
    // Seçim
    public bool IsSelectedForCheckout { get; set; }
    
    // Kupon (şimdilik null/0)
    public string? AppliedCouponCode { get; set; }  // null
    public decimal DiscountApplied { get; set; }    // 0
}