using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Etudiant
{
    public int Id { get; set; }

    [Display(Name = "Numéro étudiant")]
    public string NumeroEtudiant { get; set; } = string.Empty;

    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Display(Name = "Prénom")]
    public string Prenom { get; set; } = string.Empty;

    [Display(Name = "Date de naissance")]
    [DataType(DataType.Date)]
    public DateOnly DateNaissance { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Téléphone")]
    public string Telephone { get; set; } = string.Empty;

    [Display(Name = "Adresse")]
    public string Adresse { get; set; } = string.Empty;

    [Display(Name = "Date d'inscription")]
    public DateTime DateInscription { get; init; } = DateTime.UtcNow;

    public string NomComplet => $"{Prenom} {Nom}";
}
