using System.ComponentModel.DataAnnotations;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Etudiant
{
    public int Id { get; set; }

    [Display(Name = "Numéro étudiant")]
    public string NumeroEtudiant { get; set; } = string.Empty;

    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateOnly DateNaissance { get; set; }

    [Display(Name = "Adresse")]
    public string Adresse { get; set; } = string.Empty;

    [Display(Name = "Date d'inscription")]
    public DateTime DateInscription { get; init; } = DateTime.UtcNow;

    /// <summary>Lien obligatoire vers le compte AspNetUsers.</summary>
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    // ── Propriétés lues depuis le compte Identity (pas en base sur Etudiants) ─
    public string Nom       => User?.Nom       ?? string.Empty;
    public string Prenom    => User?.Prenom    ?? string.Empty;
    public string Email     => User?.Email     ?? string.Empty;
    public string Telephone => User?.PhoneNumber ?? string.Empty;

    public string NomComplet => $"{Prenom} {Nom}".Trim();
}
