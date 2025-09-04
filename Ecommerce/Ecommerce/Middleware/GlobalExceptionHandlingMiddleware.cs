using System.Net;
using System.Text.Json;
using Ecommerce.Utilities;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = new ApiResponse(
            statusCode: (int)HttpStatusCode.InternalServerError,
            success: false,
            message: ex.Message,
            data: null
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
