using Microsoft.AspNetCore.Identity;

namespace ProjetScolariteSOLID.Domain.Models.Auth;

/// <summary>
/// Rôle applicatif — étend IdentityRole avec une description.
/// </summary>
public sealed class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;

    // Rôles constants pour éviter les magic strings
    public const string Administrateur = "Administrateur";
    public const string Enseignant     = "Enseignant";
    public const string Etudiant       = "Etudiant";
    public const string Visiteur       = "Visiteur";

    public static readonly string[] TousLesRoles =
        [Administrateur, Enseignant, Etudiant, Visiteur];
}
