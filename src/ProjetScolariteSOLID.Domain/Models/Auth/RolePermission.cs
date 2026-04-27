namespace ProjetScolariteSOLID.Domain.Models.Auth;

/// <summary>
/// Matrice de permissions par rôle : quel rôle peut faire quoi sur quelle ressource.
/// </summary>
public sealed class RolePermission
{
    public int    Id         { get; set; }
    public string RoleId     { get; set; } = string.Empty;
    public string Ressource  { get; set; } = string.Empty;  // ex: "Etudiants"
    public bool   PeutVoir   { get; set; }
    public bool   PeutEditer { get; set; }
    public bool   PeutSupprimer { get; set; }

    public ApplicationRole? Role { get; set; }
}
