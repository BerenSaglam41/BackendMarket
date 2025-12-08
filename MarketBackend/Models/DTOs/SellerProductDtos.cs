namespace MarketBackend.Models.DTOs;

public class SellerProductCreateDto
{
    // Temel bilgiler
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Kategori ve Marka önerileri
    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public string? SellerCategorySuggestion { get; set; }  // Admin yoksa öneri

    // Görseller
    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }  // Çoklu resim

    // Seller özel alanları
    public string? SellerSku { get; set; }      // Seller'ın kendi stok kodu
    public string? Barcode { get; set; }         // Barkod
    public string? AttributesJson { get; set; }  // Teknik özellikler JSON
    public string? SellerNote { get; set; }      // Admin'e not

    // Fiyat & Stok
    public decimal ProposedPrice { get; set; }
    public int ProposedStock { get; set; }

    // Kargo
    public int ShippingTimeInDays { get; set; } = 2;
}

/// <summary>
/// Seller'ın kendi pending ürününü güncellemek için DTO
/// PUT /api/seller/products/{id}
/// </summary>
public class SellerProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }
    public string? SellerCategorySuggestion { get; set; }

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? SellerSku { get; set; }
    public string? Barcode { get; set; }
    public string? AttributesJson { get; set; }
    public string? SellerNote { get; set; }

    public decimal ProposedPrice { get; set; }
    public int ProposedStock { get; set; }

    public int ShippingTimeInDays { get; set; } = 2;
}

/// <summary>
/// Seller'a dönen pending ürün bilgisi
/// GET /api/seller/products, GET /api/seller/products/{id}
/// </summary>
public class SellerProductResponseDto
{
    public int ProductPendingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SellerCategorySuggestion { get; set; }

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? SellerSku { get; set; }
    public string? Barcode { get; set; }
    public string? AttributesJson { get; set; }
    public string? SellerNote { get; set; }

    // Fiyat & Stok
    public decimal ProposedPrice { get; set; }
    public int ProposedStock { get; set; }

    // Kargo
    public int ShippingTimeInDays { get; set; }

    // Durum bilgileri
    public string Status { get; set; } = string.Empty;  // "Waiting", "Approved", "Rejected", "NeedsUpdate"
    public string? AdminNote { get; set; }              // Admin'in notu (red/güncelleme sebebi)
    public DateTime? ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
public class AdminPendingProductResponseDto
{
    public int ProductPendingId { get; set; }

    // Seller bilgisi
    public string SellerId { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string SellerEmail { get; set; } = string.Empty;

    // Ürün bilgileri
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? SellerCategorySuggestion { get; set; }

    public string? ImageUrl { get; set; }
    public List<string>? ImageGallery { get; set; }

    public string? SellerSku { get; set; }
    public string? Barcode { get; set; }
    public string? AttributesJson { get; set; }
    public string? SellerNote { get; set; }

    public decimal ProposedPrice { get; set; }
    public int ProposedStock { get; set; }
    public int ShippingTimeInDays { get; set; }

    // Durum
    public string Status { get; set; } = string.Empty;
    public string? AdminNote { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class AdminApproveDto
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public int? CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class AdminRejectDto
{
    public string AdminNote { get; set; } = string.Empty;  // Red sebebi zorunlu
}


public class AdminRequestUpdateDto
{
    public string AdminNote { get; set; } = string.Empty;  // Ne düzeltilmeli
}

public class SellerListingCreateDto
{
    public int ProductId { get; set; }  // Mevcut ürün ID
    
    // Fiyat bilgileri
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; } = 0;
    
    public int Stock { get; set; }

    public int ShippingTimeInDays { get; set; } = 2;
    public decimal ShippingCost { get; set; } = 0;

    public string? SellerNote { get; set; }
}

public class SellerListingUpdateDto
{
    // Fiyat bilgileri
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    
    public int Stock { get; set; }

    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }

    public string? SellerNote { get; set; }
    public bool IsActive { get; set; }
}
public class SellerListingResponseDto
{
    public int ListingId { get; set; }

    // Ürün bilgileri
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSlug { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }

    // Seller fiyat bilgileri
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal UnitPrice { get; set; }
    
    public int Stock { get; set; }

    // Kargo
    public int ShippingTimeInDays { get; set; }
    public decimal ShippingCost { get; set; }

    public string? SellerNote { get; set; }

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
