using Microsoft.AspNetCore.Identity;
using MarketBackend.Models;

namespace MarketBackend.Data;

public static class RoleSeeder
{
    public static async Task SeedRoleAsync(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
    {
        // Rolleri oluştur
        var roles = new List<AppRole>
        {
            new AppRole { Name = "Admin", Description = "Full system access" },
            new AppRole { Name = "Seller", Description = "Seller - manages products and sales" },
            new AppRole { Name = "Customer", Description = "Customer - regular platform user" },
            new AppRole { Name = "Support", Description = "Customer support team" }
        };
        
        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }
        
        // Admin kullanıcı oluştur
        var adminEmail = "admin@market.com";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        
        if (existingAdmin == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}