namespace MarketBackend.Models.DTOs;

public class ProductResponseDto
{
    public int ProductId { get; set; }

    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public string? BrandName { get; set; }

    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal UnitPrice { get; set; }

    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }

    public string? ImageUrl { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public int ReviewCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Ürünü oluşturan Seller/Mağaza bilgisi (nullable - Admin ürünlerinde null)
    public string? CreatedBySellerId { get; set; }
    public ProductStoreInfoDto? Store { get; set; }
}

/// <summary>
/// Ürünü oluşturan mağaza bilgisi
/// </summary>
public class ProductStoreInfoDto
{
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public string? StoreLogoUrl { get; set; }
    public bool IsStoreVerified { get; set; }
}

public class ProductCreateDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; } = 0; 

    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string? ImageUrl { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class ProductUpdateDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; } 

    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }

    public string? ImageUrl { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}