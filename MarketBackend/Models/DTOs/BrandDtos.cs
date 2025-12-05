namespace MarketBackend.Models.DTOs;

public class BrandResponseDto
{
    public int BrandId { get; set; }

    public string Name { get; set; }
    public string Slug { get; set; }

    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public string WebsiteUrl { get; set; }

    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }

    public string Country { get; set; }
    public int? EstablishedYear { get; set; }

    public string SupportEmail { get; set; }
    public string SupportPhone { get; set; }

    public int PriorityRank { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class BrandCreateDto
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public string WebsiteUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }

    public string Country { get; set; }
    public int? EstablishedYear { get; set; }

    public string SupportEmail { get; set; }
    public string SupportPhone { get; set; }

    public int PriorityRank { get; set; } = 0;
}

public class BrandUpdateDto
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public string LogoUrl { get; set; }
    public string Description { get; set; }
    public string WebsiteUrl { get; set; }

    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }

    public string Country { get; set; }
    public int? EstablishedYear { get; set; }

    public string SupportEmail { get; set; }
    public string SupportPhone { get; set; }

    public int PriorityRank { get; set; }
}