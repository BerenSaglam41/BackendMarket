namespace MarketBackend.Models.DTOs;

public class CategoryResponseDto
{
    public int CategoryId { get; set; }

    public string Name { get; set; }
    public string Slug { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Description { get; set; }
    public string ImageUrl { get; set; }

    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CategoryCreateDto
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Description { get; set; }
    public string ImageUrl { get; set; }

    public int OrderIndex { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
}

public class CategoryUpdateDto
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Description { get; set; }
    public string ImageUrl { get; set; }

    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }

    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
}
public class CategoryTreeDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string ImageUrl { get; set; }
    public int OrderIndex { get; set; }
    public bool IsActive { get; set; }

    public List<CategoryTreeDto> Children { get; set; } = new();
}