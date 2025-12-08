namespace MarketBackend.Models.DTOs;

public class ReviewCreateDto
{
    public int ProductId {get;set;}
    public int Rating {get;set;}
    public String? Comment {get;set;}
    public string? ImageUrl {get;set;}
}
public class ReviewUpdateDto
{
    public int Rating {get;set;}
    public string? Comment {get;set;}
    public string? ImageUrl {get;set;}
}
public class ReviewAdminReplyDto
{
    public string? AdminReply {get;set;} = string.Empty;
}
public class ReviewResponseDto
{
    public int ReviewId {get;set;}
    public int ProductId {get;set;}
    public string ProductName {get;set;} = string.Empty;
    // Kullanici bilgileri
    public string UserId {get;set;}= string.Empty;
    public string UserName {get;set;} = string.Empty;
    // Review icerik
    public int Rating {get;set;}
    public string? Comment {get;set;}
    public string? ImageUrl {get;set;}

    // Durum
    public bool IsApproved {get;set;}
    public bool IsVerifiedBuyer {get;set;}
    public bool IsReported {get;set;}
    public int ReportCount {get;set;}
    // Admin cevap
    public string? AdminReply {get;set;}
    public DateTime? RepliedAt {get;set;}
    // Zaman
    public DateTime CreatedAt {get;set;}
    public DateTime? UpdatedAt {get;set;}
}