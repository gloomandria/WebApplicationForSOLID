namespace ProjetScolariteSOLID.Domain.Models.Auth;

/// <summary>
/// Représente un email en file d'attente persisté en base.
/// </summary>
public sealed class EmailQueue
{
    public int Id { get; set; }
    public string Destinataire  { get; set; } = string.Empty;
    public string Sujet         { get; set; } = string.Empty;
    public string Corps         { get; set; } = string.Empty;
    public bool   EstHtml       { get; set; } = true;
    public EmailStatut Statut   { get; set; } = EmailStatut.EnAttente;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public DateTime? DateEnvoi   { get; set; }
    public int NbTentatives     { get; set; } = 0;
    public string? MessageErreur { get; set; }
}

public enum EmailStatut
{
    EnAttente = 0,
    Envoye    = 1,
    Echoue    = 2
}
