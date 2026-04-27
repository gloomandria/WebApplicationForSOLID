using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using ProjetScolariteSOLID.Application.Contracts;

namespace ProjetScolariteSOLID.Infrastructure.Email;

public sealed class SmtpEmailSender : ISmtpEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendAsync(string destinataire, string sujet, string corps, bool estHtml, CancellationToken ct = default)
    {
        var smtpSection = _config.GetSection("Smtp");
        var host     = smtpSection["Host"]     ?? "localhost";
        var port     = int.Parse(smtpSection["Port"] ?? "587");
        var user     = smtpSection["User"]     ?? string.Empty;
        var password = smtpSection["Password"] ?? string.Empty;
        var from     = smtpSection["From"]     ?? user;
        var fromName = smtpSection["FromName"] ?? "Gestion Scolarité";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, from));
        message.To.Add(MailboxAddress.Parse(destinataire));
        message.Subject = sujet;
        message.Body    = estHtml
            ? new BodyBuilder { HtmlBody = corps }.ToMessageBody()
            : new BodyBuilder { TextBody = corps }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTlsWhenAvailable, ct);
        if (!string.IsNullOrEmpty(user))
            await client.AuthenticateAsync(user, password, ct);
        await client.SendAsync(message, ct);
        await client.DisconnectAsync(true, ct);
        _logger.LogInformation("Email envoyé à {To} — sujet: {Subject}", destinataire, sujet);
    }
}
