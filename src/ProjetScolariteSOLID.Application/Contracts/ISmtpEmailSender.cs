namespace ProjetScolariteSOLID.Application.Contracts;

public interface ISmtpEmailSender
{
    Task SendAsync(string destinataire, string sujet, string corps, bool estHtml, CancellationToken ct = default);
}
