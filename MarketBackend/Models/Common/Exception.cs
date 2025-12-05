namespace MarketBackend.Models.Common;
/// <summary>
/// Tum exceptionlarin sinifi
/// </summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public List<string>? Errors { get; }
    protected AppException(string message,int statusCode,List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

/// <summary>
/// 404 Not Found Exception
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message,StatusCodes.Status404NotFound)
    {
        
    }
}

/// <summary>
/// 400 Bad Request
/// </summary>
public class BadRequestException : AppException
{
    public BadRequestException(string message ,List<string>? errors = null)
        : base(message,StatusCodes.Status400BadRequest,errors)
    {
        
    }
}

/// <summary>
/// 401 Unauthorized için (kimlik doğrulama hatası)
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message, List<string>? errors = null)
        : base(message, StatusCodes.Status401Unauthorized, errors)
    {
    }
}

/// <summary>
/// 403 Forbidden için (yetki yok)
/// </summary>
public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Bu işlem için yetkiniz yok") 
        : base(message, StatusCodes.Status403Forbidden)
    {
    }
}

/// <summary>
/// 409 Conflict için (veri çakışması, slug zaten var gibi)
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) 
        : base(message, StatusCodes.Status409Conflict)
    {
    }
}