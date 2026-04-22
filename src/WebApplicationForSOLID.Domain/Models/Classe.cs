using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Classe
{
    public int Id { get; set; }

    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Display(Name = "Niveau")]
    public NiveauClasse Niveau { get; set; }

    [Display(Name = "Année académique")]
    public string AnneeAcademique { get; set; } = string.Empty;

    [Display(Name = "Capacité max.")]
    public int CapaciteMax { get; set; } = 30;

    [Display(Name = "Filière")]
    public string Filiere { get; set; } = string.Empty;
}

public enum NiveauClasse
{
    [Display(Name = "Licence 1")] L1,
    [Display(Name = "Licence 2")] L2,
    [Display(Name = "Licence 3")] L3,
    [Display(Name = "Master 1")] M1,
    [Display(Name = "Master 2")] M2,
    [Display(Name = "Doctorat")] Doctorat
}
