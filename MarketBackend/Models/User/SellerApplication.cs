namespace MarketBackend.Models;

public class SellerApplication
{
    public int SellerApplicationId {get;set;}
    public string AppUserId {get;set;} = null!;
    public AppUser AppUser {get;set;} = null!;
    public string? StoreLogoUrl {get;set;}
    public string StoreName {get;set;}
    public string StoreSlug {get;set;}
    public string StoreDescription {get;set;}
    public string StorePhone {get;set;}

    public SellerApplicationStatus Status {get;set;} = SellerApplicationStatus.Pending;
    public string? AdminNote {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    public DateTime? ReviewedAt {get;set;}
    public string? ReviewedByAdminId {get;set;}
    
}