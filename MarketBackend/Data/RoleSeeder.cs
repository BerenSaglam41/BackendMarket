using Microsoft.AspNetCore.Identity;
using MarketBackend.Models;

namespace MarketBackend.Data;

public static class RoleSeeder
{
    public static async Task SeedRoleAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new List <AppRole>
        {
            new AppRole { Name = "Admin", Description = "Full system access" },
            new AppRole { Name = "Seller", Description = "Manages assigned brand's products" },
            new AppRole { Name = "Support", Description = "Handles customer support tasks" },
            new AppRole { Name = "Customer", Description = "Regular platform user" }
        };
        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(role);
            }
        }   
    }
}