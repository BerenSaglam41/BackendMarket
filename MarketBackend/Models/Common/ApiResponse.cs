namespace MarketBackend.Models.Common;

/// <summary>
/// Standart API yanıt formatı - Tüm endpoint'lerde kullanılır
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Başarılı yanıt (data ile)
    public static ApiResponse<T> SuccessResponse(T data, string message = "İşlem başarılı", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    // Başarılı yanıt (sadece mesaj)
    public static ApiResponse<T> SuccessResponse(string message = "İşlem başarılı", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = default,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }

    // Hata yanıtı
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Data olmayan yanıtlar için kısayol
/// </summary>
public class ApiResponse : ApiResponse<object>
{
}

/// <summary>
/// Sayfalama ile birlikte yanıt
/// </summary>
public class PagedApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "İşlem başarılı";
    public T? Data { get; set; }
    public PaginationInfo? Pagination { get; set; }
    public int StatusCode { get; set; } = 200;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static PagedApiResponse<T> SuccessResponse(
        T data, 
        int currentPage, 
        int pageSize, 
        int totalCount,
        string message = "İşlem başarılı")
    {
        return new PagedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = new PaginationInfo
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            },
            StatusCode = 200,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
