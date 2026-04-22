namespace ProjetScolariteSOLID.Domain.Models;

/// <summary>
/// Enregistre chaque opération (INSERT, UPDATE, DELETE) sur les entités métier.
/// </summary>
public sealed class AuditLog
{
    public long   Id            { get; set; }

    /// <summary>Nom de la table / entité concernée.</summary>
    public string TableName     { get; set; } = string.Empty;

    /// <summary>INSERT | UPDATE | DELETE</summary>
    public string Action        { get; set; } = string.Empty;

    /// <summary>Clé primaire de l'entité modifiée (sérialisée en JSON).</summary>
    public string KeyValues     { get; set; } = string.Empty;

    /// <summary>Valeurs avant modification (null pour INSERT).</summary>
    public string? OldValues    { get; set; }

    /// <summary>Valeurs après modification (null pour DELETE).</summary>
    public string? NewValues    { get; set; }

    /// <summary>Identifiant (UserId) de l'utilisateur ayant déclenché l'action.</summary>
    public string? UserId       { get; set; }

    /// <summary>Horodatage UTC de l'action.</summary>
    public DateTime Timestamp   { get; set; } = DateTime.UtcNow;
}
