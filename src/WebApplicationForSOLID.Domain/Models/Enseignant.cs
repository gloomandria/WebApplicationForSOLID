using System.ComponentModel.DataAnnotations;

namespace WebApplicationForSOLID.Domain.Models;

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
    public string Specialite { get; set; } = string.Empty;

    [Display(Name = "Grade")]
    public GradeEnseignant Grade { get; set; }

    [Display(Name = "Date d'embauche")]
    public DateTime DateEmbauche { get; init; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}";
}

public enum GradeEnseignant
{
    [Display(Name = "Assistant")] Assistant,
    [Display(Name = "Maître-assistant")] MaitreAssistant,
    [Display(Name = "Maître de conférences")] MaitreDeConferences,
    [Display(Name = "Professeur")] Professeur
}
