using System.Security.Claims;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.Common;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketBackend.Controllers
{
    [ApiController]
    [Route("api/admin/sellers")]
    [Authorize(Roles = "Admin")]
    public class AdminSellerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminSellerController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? search = null,
            bool? isActive = null,
            bool? isVerified = null,
            bool? isBanned = null,
            int page = 1,
            int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize > 100) pageSize = 100;

            var sellerRoleId = await _context.Roles
                .Where(r => r.Name == "Seller")
                .Select(r => r.Id)
                .FirstAsync();

            var query =
                from u in _context.Users
                join ur in _context.UserRoles on u.Id equals ur.UserId
                where ur.RoleId == sellerRoleId
                select u;

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    (x.StoreName ?? "").Contains(search) ||
                    (x.Email ?? "").Contains(search));

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            if (isVerified.HasValue)
                query = query.Where(x => x.IsStoreVerified == isVerified.Value);

            if (isBanned.HasValue)
                query = query.Where(x => x.IsBanned == isBanned.Value);

            var totalCount = await query.CountAsync();

            var sellers = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Id,
                    s.Email,
                    s.FirstName,
                    s.LastName,
                    s.StoreName,
                    s.StoreSlug,
                    s.StoreLogoUrl,
                    s.IsActive,
                    s.IsBanned,
                    s.IsStoreVerified,
                    s.CreatedAt,

                    TotalListings = _context.Listings.Count(l => l.SellerId == s.Id && !s.IsBanned),
                    ActiveListings = _context.Listings.Count(l => l.SellerId == s.Id && l.IsActive && !s.IsBanned),

                    TotalOrders = _context.OrderItems
                        .Where(o => o.SellerId == s.Id && !s.IsBanned)
                        .Select(o => o.OrderId)
                        .Distinct()
                        .Count(),

                    TotalRevenue = _context.OrderItems
                        .Where(o =>
                            o.SellerId == s.Id &&
                            o.Order.OrderStatus == OrderStatus.Delivered &&
                            !s.IsBanned)
                        .Sum(o => (decimal?)o.TotalPrice) ?? 0
                })
                .ToListAsync();

            return Ok(PagedApiResponse<object>.SuccessResponse(
                sellers,
                page,
                pageSize,
                totalCount,
                "Satıcılar başarıyla getirildi."
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(string id)
        {
            var seller = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (seller == null)
                throw new NotFoundException("Satıcı bulunamadı.");

            var detail = new
            {
                seller.Id,
                seller.FirstName,
                seller.LastName,
                seller.Email,
                seller.StoreName,
                seller.StoreSlug,
                seller.StorePhone,
                seller.StoreDescription,
                seller.StoreLogoUrl,
                seller.IsActive,
                seller.IsBanned,
                seller.BanReason,
                seller.CreatedAt,
                seller.IsStoreVerified,
            
                TotalOrders = await _context.OrderItems
                    .Where(o => o.SellerId == id)
                    .Select(o => o.OrderId)
                    .Distinct()
                    .CountAsync(),

                TotalRevenue = await _context.OrderItems
                    .Where(o =>
                        o.SellerId == id &&
                        o.Order.OrderStatus == OrderStatus.Delivered)
                    .SumAsync(o => (decimal?)o.TotalPrice) ?? 0,

                seller.BannedAt,
                seller.BannedByAdminId,
            };

            return Ok(ApiResponse<object>.SuccessResponse(detail));
        }


        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var seller = await _userManager.FindByIdAsync(id);
            if (seller == null)
                throw new NotFoundException("Satıcı bulunamadı.");
                        // Satıcı kontrolü ekle
            var sellerRoleId = await _context.Roles
                .Where(r => r.Name == "Seller")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var isSeller = await _context.UserRoles.AnyAsync(ur => ur.UserId == id && ur.RoleId == sellerRoleId);
            if (!isSeller)
                throw new BadRequestException("Bu kullanıcı bir satıcı değil.");
            if (seller.IsBanned)
                throw new BadRequestException("Banlı kullanıcı aktif edilemez.");

            seller.IsActive = !seller.IsActive;
            await _userManager.UpdateAsync(seller);

            return Ok(ApiResponse.SuccessResponse(
                seller.IsActive ? "Satıcı aktif edildi." : "Satıcı pasif edildi."
            ));
        }

        [HttpPost("{id}/ban")]
        public async Task<IActionResult> Ban(string id, [FromBody] AdminSellerDto dto)
        {
            var seller = await _userManager.FindByIdAsync(id);
            if (seller == null)
                throw new NotFoundException("Satıcı bulunamadı.");

            // Satıcı kontrolü ekle
            var sellerRoleId = await _context.Roles
                .Where(r => r.Name == "Seller")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var isSeller = await _context.UserRoles.AnyAsync(ur => ur.UserId == id && ur.RoleId == sellerRoleId);
            if (!isSeller)
                throw new BadRequestException("Bu kullanıcı bir satıcı değil.");

            seller.IsBanned = true;
            seller.IsActive = false;
            seller.BanReason = dto.BanReason;
            seller.BannedAt = DateTime.UtcNow;
            seller.BannedByAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _userManager.UpdateAsync(seller);

            // 🔥 KRİTİK: TÜM LİSTINGLERİ KAPAT
            var listings = await _context.Listings
                .Where(l => l.SellerId == id && l.IsActive)
                .ToListAsync();

            foreach (var l in listings)
                l.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse.SuccessResponse("Satıcı banlandı."));
        }

        [HttpPost("{id}/unban")]
        public async Task<IActionResult> Unban(string id)
        {
            var seller = await _userManager.FindByIdAsync(id);
            if (seller == null)
                throw new NotFoundException("Satıcı bulunamadı.");
                        // Satıcı kontrolü ekle
            var sellerRoleId = await _context.Roles
                .Where(r => r.Name == "Seller")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var isSeller = await _context.UserRoles.AnyAsync(ur => ur.UserId == id && ur.RoleId == sellerRoleId);
            if (!isSeller)
                throw new BadRequestException("Bu kullanıcı bir satıcı değil.");
            seller.IsBanned = false;
            seller.IsActive = true;
            seller.BanReason = null;
            seller.BannedAt = null;
            seller.BannedByAdminId = null;

            await _userManager.UpdateAsync(seller);

            return Ok(ApiResponse.SuccessResponse("Satıcı banı kaldırıldı."));
        }
    }
}