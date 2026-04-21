using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WebApplicationForSOLID.Application.CQRS.Behaviors;

/// <summary>
/// SRP — Cross-cutting concern : log automatique de toutes les requêtes MediatR.
/// Mesure la durée d'exécution et avertit si trop longue (> 500ms).
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("[CQRS] → {Request}", requestName);

        try
        {
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
                _logger.LogWarning("[CQRS] ⚠ {Request} lent ({Elapsed}ms)", requestName, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("[CQRS] ✓ {Request} ({Elapsed}ms)", requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "[CQRS] ✗ {Request} échoué ({Elapsed}ms)", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
