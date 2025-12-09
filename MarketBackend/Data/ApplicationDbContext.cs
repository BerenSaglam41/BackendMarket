using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Models;

namespace MarketBackend.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // MODELLERİN TABLOLARI
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductPending> ProductPendings { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ProductViewHistory> ProductViewHistories { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public DbSet<Address> Addresses { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentEvent> ShipmentEvents { get; set; }

    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<Models.Payment.Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ORDER → Shipping Address
        builder.Entity<Order>()
            .HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // ORDER → Billing Address
        builder.Entity<Order>()
            .HasOne(o => o.BillingAddress)
            .WithMany()
            .HasForeignKey(o => o.BillingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // PRODUCT → REVIEW (Cascade Delete)
        builder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // CATEGORY → PRODUCT (Restrict önerilir)
        builder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)  // ⚠ Burayı Products olarak düzeltmeliyiz
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // SHIPMENT → SHIPMENT EVENT
        builder.Entity<ShipmentEvent>()
            .HasOne(se => se.Shipment)
            .WithMany(s => s.Events)
            .HasForeignKey(se => se.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // CATEGORY → ParentCategory
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // SELLER PRODUCT → AppUser (Seller)
        builder.Entity<Listing>()
            .HasOne(sp => sp.Seller)
            .WithMany(u => u.Listings)
            .HasForeignKey(sp => sp.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        // SELLER PRODUCT → Product
        builder.Entity<Listing>()
            .HasOne(sp => sp.Product)
            .WithMany(p => p.Listings)
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // PRODUCT PENDING → AppUser (Seller)
        builder.Entity<ProductPending>()
            .HasOne(pp => pp.Seller)
            .WithMany(u => u.ProductPendings)
            .HasForeignKey(pp => pp.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        // PRODUCT PENDING → Brand (Optional)
        builder.Entity<ProductPending>()
            .HasOne(pp => pp.Brand)
            .WithMany()
            .HasForeignKey(pp => pp.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

        // PRODUCT PENDING → Category (Optional)
        builder.Entity<ProductPending>()
            .HasOne(pp => pp.Category)
            .WithMany()
            .HasForeignKey(pp => pp.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // PRODUCT → CreatedBySeller (Ürünü oluşturan Seller)
        builder.Entity<Product>()
            .HasOne(p => p.CreatedBySeller)
            .WithMany()
            .HasForeignKey(p => p.CreatedBySellerId)
            .OnDelete(DeleteBehavior.SetNull);  // Seller silinirse ürün kalır ama sahipsiz olur

        // CART ITEM -> Listing (Hangi satıcıdan alınıyor?)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Listing)
            .WithMany()
            .HasForeignKey(ci => ci.ListingId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Listing silinince CartItem de silinir

        // CART ITEM → Product (Genel ürün bilgisi)
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);  // Product silinirse CartItem Cascade ile zaten gitmiş olur

        // WISHLIST ITEM → Product
        builder.Entity<WishlistItem>()
            .HasOne(wi => wi.Product)
            .WithMany(p => p.WishlistItems)
            .HasForeignKey(wi => wi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);  // Product silinince WishlistItem de silinir

        // WISHLIST ITEM → User
        builder.Entity<WishlistItem>()
            .HasOne(wi => wi.AppUser)
            .WithMany(u => u.WishlistItems)
            .HasForeignKey(wi => wi.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);  // User silinince wishlist de silinir

        // ORDER ITEM -> Listing (Hangi satıcıdan alındı?)
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Listing)
            .WithMany()
            .HasForeignKey(oi => oi.ListingId)
            .OnDelete(DeleteBehavior.Restrict);  // Listing silinirse sipariş kaydı kalır

        // Coupon relationships
        builder.Entity<Coupon>()
            .HasOne(c => c.CreatedByAdmin)
            .WithMany(u => u.CreatedAdminCoupons)
            .HasForeignKey(c => c.CreatedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Coupon>()
            .HasOne(c => c.CreatedBySeller)
            .WithMany(u => u.CreatedSellerCoupons)
            .HasForeignKey(c => c.CreatedBySellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Coupon>()
            .HasIndex(c => c.Code)
            .IsUnique();

        builder.Entity<ShoppingCart>()
            .HasOne(sc => sc.AppliedCoupon)
            .WithMany()
            .HasForeignKey(sc => sc.AppliedCouponCode)
            .HasPrincipalKey(c => c.Code)
            .OnDelete(DeleteBehavior.SetNull);

        // Her kullanıcının sadece 1 aktif sepeti olabilir
        builder.Entity<ShoppingCart>()
            .HasIndex(sc => new { sc.AppUserId, sc.IsActive })
            .IsUnique()
            .HasFilter("IsActive = 1");

        // Product Slug unique index
        builder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        // ProductViewHistory - Her kullanıcı-listing kombinasyonu için tek kayıt
        builder.Entity<ProductViewHistory>()
            .HasIndex(pvh => new { pvh.UserId, pvh.ListingId })
            .IsUnique();

        // ProductViewHistory - Listing relationship
        builder.Entity<ProductViewHistory>()
            .HasOne(pvh => pvh.Listing)
            .WithMany()
            .HasForeignKey(pvh => pvh.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}