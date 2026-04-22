using System.Net;
using System.Text.Json;

namespace ProjetScolariteSOLID.Web.Middleware;

/// <summary>
/// SRP — Responsabilité unique : intercepter et gérer toutes les exceptions non traitées.
/// </summary>
public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception non gérée sur {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                   || context.Request.Headers.Accept.Any(h => h?.Contains("application/json") == true);

        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = "Une erreur interne est survenue."
            }));
        }

        context.Response.Redirect("/Home/Error");
        return Task.CompletedTask;
    }
}
