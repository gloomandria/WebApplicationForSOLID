using Microsoft.AspNetCore.Identity;

namespace ProjetScolariteSOLID.Domain.Models.Auth;

/// <summary>
/// Utilisateur de l'application — étend IdentityUser avec les champs métier.
/// </summary>
public sealed class ApplicationUser : IdentityUser
{
    public string Prenom { get; set; } = string.Empty;
    public string Nom    { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    public bool EstActif { get; set; } = true;

    /// <summary>Clé étrangère optionnelle vers la fiche Etudiant.</summary>
    public int? EtudiantId { get; set; }

    /// <summary>Clé étrangère optionnelle vers la fiche Enseignant.</summary>
    public int? EnseignantId { get; set; }

    public string NomComplet => $"{Prenom} {Nom}".Trim();
}
