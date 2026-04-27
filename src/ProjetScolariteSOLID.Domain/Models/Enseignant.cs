using System.ComponentModel.DataAnnotations;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Enseignant
{
    public int Id { get; set; }

    [Display(Name = "Matricule")]
    public string Matricule { get; set; } = string.Empty;

    [Display(Name = "Spécialité")]
    public int SpecialiteId { get; set; }

    public Specialite? Specialite { get; set; }

    [Display(Name = "Grade")]
    public int GradeId { get; set; }

    public Grade? Grade { get; set; }

    [Display(Name = "Date d'embauche")]
    public DateTime DateEmbauche { get; init; } = DateTime.UtcNow;

    /// <summary>Lien obligatoire vers le compte AspNetUsers.</summary>
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    // ── Propriétés lues depuis le compte Identity (pas en base sur Enseignants) ─
    public string Nom       => User?.Nom       ?? string.Empty;
    public string Prenom    => User?.Prenom    ?? string.Empty;
    public string Email     => User?.Email     ?? string.Empty;
    public string Telephone => User?.PhoneNumber ?? string.Empty;

    public string NomComplet => $"{Prenom} {Nom}".Trim();
}
