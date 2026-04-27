using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Enseignant
{
    public int Id { get; set; }

    [Display(Name = "Matricule")]
    public string Matricule { get; set; } = string.Empty;

    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Téléphone")]
    public string Telephone { get; set; } = string.Empty;

    [Display(Name = "Spécialité")]
    public int SpecialiteId { get; set; }

    public Specialite? Specialite { get; set; }

    [Display(Name = "Grade")]
    public int GradeId { get; set; }

    public Grade? Grade { get; set; }

    [Display(Name = "Date d'embauche")]
    public DateTime DateEmbauche { get; init; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}";
}
