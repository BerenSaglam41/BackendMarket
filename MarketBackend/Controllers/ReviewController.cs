using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketBackend.Data;
using MarketBackend.Models;
using MarketBackend.Models.DTOs;
using MarketBackend.Models.Common;

namespace MarketBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public ReviewController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Ürüne ait onaylanmış yorumları listele
    /// GET /api/review/product/{productId}
    /// </summary>
    [HttpGet("product/{productId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(int productId, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new NotFoundException("Ürün bulunamadı.");

        var query = _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var reviewDtos = reviews.Select(r => ToReviewDto(r)).ToList();
        return Ok(PagedApiResponse<List<ReviewResponseDto>>.SuccessResponse(
            reviewDtos,
            page,
            pageSize,
            totalCount,
            "Yorumlar başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Kullanıcının kendi yorumlarını listele
    /// GET /api/review/my-reviews
    /// </summary>
    [HttpGet("my-reviews")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews(int page = 1, int pageSize = 10)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 50) pageSize = 50;

        var query = _context.Reviews
            .Include(r => r.Product)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var reviewDtos = reviews.Select(r => ToReviewDto(r)).ToList();
        return Ok(PagedApiResponse<List<ReviewResponseDto>>.SuccessResponse(
            reviewDtos,
            page,
            pageSize,
            totalCount,
            "Yorumlar başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Yeni yorum oluştur
    /// POST /api/review
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(ReviewCreateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            throw new NotFoundException("Ürün bulunamadı.");

        // Kullanıcı bu ürün için daha önce yorum yapmış mı kontrol et
        var existingReview = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

        if (existingReview != null)
            throw new BadRequestException("Bu ürün için zaten yorum yaptınız.");

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            ImageUrl = dto.ImageUrl,
            IsApproved = false,
            IsVerifiedBuyer = false,
            IsReported = false,
            ReportCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProductReviews), 
            new { productId = dto.ProductId }, 
            ApiResponse<object>.SuccessResponse(
            new { ReviewId = review.ReviewId } ,
            "Yorumunuz oluşturuldu. Admin onayından sonra yayınlanacaktır.",
            201
        ));
    }

    /// <summary>
    /// Kendi yorumunu güncelle
    /// PUT /api/review/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int id, ReviewUpdateDto dto)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        if (review.UserId != userId)
            throw new ForbiddenException("Bu yorumu düzenleme yetkiniz yok.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.ImageUrl = dto.ImageUrl;
        review.UpdatedAt = DateTime.UtcNow;
        review.IsApproved = false;

        await _context.SaveChangesAsync();
        var reviewDto = ToReviewDto(review);
        return Ok(ApiResponse<ReviewResponseDto>.SuccessResponse(
            reviewDto,
            "Yorumunuz güncellendi. Admin onayından sonra yeniden yayınlanacaktır."
        ));
    }

    /// <summary>
    /// Kendi yorumunu sil
    /// DELETE /api/review/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedException("Giriş yapmanız gerekiyor.");

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        if (review.UserId != userId)
            throw new ForbiddenException("Bu yorumu silme yetkiniz yok.");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Yorumunuz silindi."
        ));
    }

    /// <summary>
    /// Yorumu onayla (Admin)
    /// POST /api/review/admin/{id}/approve
    /// </summary>
    [HttpPost("admin/{id:int}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        if (review.IsApproved)
            throw new BadRequestException("Bu yorum zaten onaylanmış.");

        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        var reviewDto = ToReviewDto(review);
        return Ok(ApiResponse<ReviewResponseDto>.SuccessResponse(
            reviewDto,
            "Yorum onaylandı ve yayınlandı."
        ));
    }

    /// <summary>
    /// Yorumu reddet (Admin) - Sadece pending yorumlar için
    /// POST /api/review/admin/{id}/reject
    /// </summary>
    [HttpPost("admin/{id:int}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectReview(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        if (review.IsApproved)
            throw new BadRequestException("Zaten yayınlanmış yorumları reddedemezsiniz. Silme işlemi için admin-delete endpoint'ini kullanın.");

        // Yorumu veritabanından sil
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            new { ReviewId = id },
            "Yorum reddedildi ve silindi."
        ));
    }

    /// <summary>
    /// Yayınlanmış yorumu sil (Admin) - Uygunsuz içerik, şikayet vs. için
    /// DELETE /api/review/admin/{id}/delete
    /// </summary>
    [HttpDelete("admin/{id:int}/delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDeleteReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.SuccessResponse(
            new { ReviewId = id },
            "Yorum başarıyla silindi."
        ));
    }

    /// <summary>
    /// Yoruma admin cevabı ekle
    /// POST /api/review/admin/{id}/reply
    /// </summary>
    [HttpPost("admin/{id:int}/reply")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAdminReply(int id, ReviewAdminReplyDto dto)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        review.AdminReply = dto.AdminReply;
        review.RepliedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var reviewDto = ToReviewDto(review);
        return Ok(ApiResponse<ReviewResponseDto>.SuccessResponse(
            reviewDto,
            "Admin cevabı eklendi."
        ));
    }

    /// <summary>
    /// Yorumu şikayet edilmiş olarak işaretle
    /// POST /api/review/{id}/report
    /// </summary>
    [HttpPost("{id:int}/report")]
    [Authorize]
    public async Task<IActionResult> ReportReview(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        if (!review.IsApproved)
            throw new BadRequestException("Sadece yayınlanmış yorumları şikayet edebilirsiniz.");

        review.IsReported = true;
        review.ReportCount += 1;
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ApiResponse.SuccessResponse(
            "Yorum şikayet edildi. Teşekkürler. Admine bildirilecektir."
        ));
    }

    /// <summary>
    /// Onay bekleyen yorumları listele (Admin)
    /// GET /api/review/admin/pending
    /// </summary>
    [HttpGet("admin/pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingReviews(int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => !r.IsApproved)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var reviewDtos = reviews.Select(r => ToReviewDto(r)).ToList();
        return Ok(PagedApiResponse<List<ReviewResponseDto>>.SuccessResponse(
            reviewDtos,
            page,
            pageSize,
            totalCount,
            "Onay bekleyen yorumlar başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Tek bir yorumun detayını getir (Admin)
    /// GET /api/review/admin/{id}
    /// </summary>
    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
                .ThenInclude(p => p.Brand)
            .Include(r => r.Product)
                .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (review == null)
            throw new NotFoundException("Yorum bulunamadı.");

        var reviewDto = ToReviewDto(review);
        return Ok(ApiResponse<ReviewResponseDto>.SuccessResponse(
            reviewDto,
            "Yorum başarıyla getirildi."
        ));
    }

    /// <summary>
    /// Şikayet edilmiş yorumları listele (Admin)
    /// GET /api/review/admin/reported
    /// </summary>
    [HttpGet("admin/reported")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReportedReviews(int page = 1, int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.IsReported)
            .OrderByDescending(r => r.UpdatedAt);

        var totalCount = await query.CountAsync();

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var reviewDtos = reviews.Select(r => ToReviewDto(r)).ToList();  
        return Ok(PagedApiResponse<List<ReviewResponseDto>>.SuccessResponse(
            reviewDtos,
            page,
            pageSize,
            totalCount,
            "Şikayet edilmiş yorumlar başarıyla getirildi."
        ));
    }
    private static ReviewResponseDto ToReviewDto(Review r)
    {
        return new ReviewResponseDto
        {
            ReviewId = r.ReviewId,
            ProductId = r.ProductId,
            ProductName = r.Product?.Name ?? "",
            UserId = r.UserId,
            UserName = r.User?.UserName ?? "Kullanıcı",
            Rating = r.Rating,
            Comment = r.Comment,
            ImageUrl = r.ImageUrl,
            IsApproved = r.IsApproved,
            IsVerifiedBuyer = r.IsVerifiedBuyer,
            IsReported = r.IsReported,
            ReportCount = r.ReportCount,
            AdminReply = r.AdminReply,
            RepliedAt = r.RepliedAt,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        };
    }
}