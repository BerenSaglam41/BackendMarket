using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MarketBackend.Models.Common;

namespace MarketBackend.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Bir hata oluştu: {Message}", exception.Message);

        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // Uygulama özel exceptionları
            AppException appEx => new
            {
                success = false,
                message = appEx.Message,
                errors = appEx.Errors,
                statusCode = appEx.StatusCode,
                stackTrace = _env.IsDevelopment() ? appEx.StackTrace : null
            },

            // Fluent Validation hataları
            FluentValidation.ValidationException valEx => new
            {
                success = false,
                message = "Doğrulama hatası",
                errors = valEx.Errors.Select(e => e.ErrorMessage).ToList(),
                statusCode = StatusCodes.Status400BadRequest,
                stackTrace = _env.IsDevelopment() ? valEx.StackTrace : null
            },

            // Bilinmeyen hatalar
            _ => new
            {
                success = false,
                message = _env.IsDevelopment()
                    ? exception.Message
                    : "Sunucu tarafında bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                errors = (List<string>?)null,
                statusCode = StatusCodes.Status500InternalServerError,
                stackTrace = _env.IsDevelopment() ? exception.StackTrace : null
            }
        };

        context.Response.StatusCode = response.statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}