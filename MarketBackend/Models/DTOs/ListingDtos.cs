namespace MarketBackend.Models.DTOs;

// Basic listing response for list views
public class ListingResponseDto
{
    public int ListingId { get; set; }
    public string Slug { get; set; } = string.Empty;
    
    // Product Info
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public string? ProductImageUrl { get; set; }
    
    // Category Info
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    
    // Brand Info
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string BrandSlug { get; set; } = string.Empty;
    public string? BrandLogoUrl { get; set; }
    
    // Seller Info
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string SellerSlug { get; set; } = string.Empty;
    
    // Pricing & Stock
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }
    public string? SellerNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Detailed listing response with other sellers and recommendations
public class ListingDetailResponseDto
{
    // Current Listing Details
    public int ListingId { get; set; }
    public string Slug { get; set; } = string.Empty;
    
    // Product Info
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductDescription { get; set; }
    public string? ProductImageUrl { get; set; }
    
    // Category Info
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    
    // Brand Info
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string BrandSlug { get; set; } = string.Empty;
    public string? BrandLogoUrl { get; set; }
    
    // Seller Info
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string SellerSlug { get; set; } = string.Empty;
    
    // Pricing & Stock
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }
    public string? SellerNote { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Other sellers of the same product
    public List<SellerComparisonDto> OtherSellers { get; set; } = new();
    
    // Similar/recommended products
    public List<ListingResponseDto> SimilarProducts { get; set; } = new();
}

// Seller comparison for price comparison feature
public class SellerComparisonDto
{
    public int ListingId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string SellerSlug { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int Stock { get; set; }
    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }
    public string? SellerNote { get; set; }
}

// Product with all its sellers for comparison page
public class ProductSellerComparisonDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string? BrandLogoUrl { get; set; }
    public List<SellerComparisonDto> Sellers { get; set; } = new();
}
