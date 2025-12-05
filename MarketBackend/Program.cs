using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;

using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Extensions;
using MarketBackend.Services;

using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Development için hata detaylarını göster
if (builder.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true;
}

// 1) Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2) DbContext Kaydı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 3) Identity Kaydı - AddIdentityCore kullan (Cookie auth eklemez)
builder.Services.AddIdentityCore<AppUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
    .AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>()
    .AddSignInManager<SignInManager<AppUser>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 4) FluentValidation – Yeni yöntem
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Validator’ları otomatik tarayıp bulması için:
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// 5) Controllers
builder.Services.AddControllers();

// 6) JWT
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Roller seed ediliyor
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await RoleSeeder.SeedRoleAsync(roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// HTTPS redirect'i development'ta kaldır (HTTP kullanıyorsan)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();