using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjetScolariteSOLID.Application.Contracts;

namespace ProjetScolariteSOLID.Infrastructure.Email;

/// <summary>
/// Service d'arrière-plan qui traite la file d'attente d'emails toutes les 30 secondes.
/// </summary>
public sealed class EmailQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<EmailQueueBackgroundService> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    public EmailQueueBackgroundService(IServiceProvider services, ILogger<EmailQueueBackgroundService> logger)
    {
        _services = services;
        _logger   = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailQueueBackgroundService démarré.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de la file d'emails.");
            }
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        await using var scope  = _services.CreateAsyncScope();
        var queue  = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();
        var sender = scope.ServiceProvider.GetRequiredService<ISmtpEmailSender>();

        var pending = await queue.GetPendingAsync(20, ct);
        foreach (var email in pending)
        {
            try
            {
                await sender.SendAsync(email.Destinataire, email.Sujet, email.Corps, email.EstHtml, ct);
                await queue.MarkSentAsync(email.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Échec envoi email {Id} à {To}", email.Id, email.Destinataire);
                await queue.MarkFailedAsync(email.Id, ex.Message, ct);
            }
        }
    }
}
