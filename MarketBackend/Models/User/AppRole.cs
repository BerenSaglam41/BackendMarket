using Microsoft.AspNetCore.Identity;

namespace MarketBackend.Models;

public class AppRole : IdentityRole
{
    public string? Description { get; set; }
}