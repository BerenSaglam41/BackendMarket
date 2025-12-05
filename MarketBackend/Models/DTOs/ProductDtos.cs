namespace MarketBackend.Models.DTOs;

/// <summary>
/// Ürün detay response - Satıcılar listesiyle birlikte
/// </summary>
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

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public bool IsActive { get; set; }
    public int ReviewCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Ürünü oluşturan Seller/Mağaza bilgisi (nullable - Admin ürünlerinde null)
    public string? CreatedBySellerId { get; set; }
    public ProductStoreInfoDto? Store { get; set; }

    // ⭐ Bu ürünü satan satıcılar
    public List<ProductSellerDto>? Sellers { get; set; }
    
    // ⭐ En düşük fiyat (hızlı görüntüleme için)
    public decimal? MinPrice { get; set; }
    public int? SellerCount { get; set; }
}

/// <summary>
/// Ürünü satan satıcı bilgisi
/// </summary>
public class ProductSellerDto
{
    public int SellerProductId { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string StoreSlug { get; set; } = string.Empty;
    public bool IsStoreVerified { get; set; }

    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal UnitPrice { get; set; }
    
    public int Stock { get; set; }
    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }
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

/// <summary>
/// Admin ürün oluşturma - Sadece ürün tanımı (fiyat/stok yok)
/// </summary>
public class ProductCreateDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

/// <summary>
/// Admin ürün güncelleme - Sadece ürün tanımı (fiyat/stok yok)
/// </summary>
public class ProductUpdateDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }

    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    
    public bool IsActive { get; set; }
}