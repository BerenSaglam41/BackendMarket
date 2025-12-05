namespace MarketBackend.Models;

public class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; }                     // Zorunlu
    public string Slug { get; set; }                     // Zorunlu, SEO URL

    public int? ParentCategoryId { get; set; }           // Alt kategori desteği
    public Category ParentCategory { get; set; }         // Navigation: ebeveyn kategori
    public ICollection<Category> SubCategories { get; set; } = new List<Category>(); // Navigation: alt kategoriler
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public string Description { get; set; }              
    public string ImageUrl { get; set; }                 

    public int OrderIndex { get; set; }                  
    public bool IsActive { get; set; }                   

    public string MetaTitle { get; set; }                
    public string MetaDescription { get; set; }          

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}