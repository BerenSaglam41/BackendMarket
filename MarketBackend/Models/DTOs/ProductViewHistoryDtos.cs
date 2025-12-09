namespace MarketBackend.Models.DTOs;

public class RecordViewDto
{
    public int ListingId { get; set; }
    public string? Source { get; set; }
    public string? DeviceType { get; set; }
}

public class ProductViewHistoryResponseDto
{
    public int ProductViewHistoryId { get; set; }
    public int ListingId { get; set; }
    public string? ListingName { get; set; }
    public string? ListingSlug { get; set; }
    public int ViewCount { get; set; }
    public DateTime FirstViewedAt { get; set; }
    public DateTime LastViewedAt { get; set; }
    public string? Source { get; set; }
}

public class MostViewedListingDto
{
    public int ListingId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int TotalViews { get; set; }
    public int UniqueViewers { get; set; }
}
