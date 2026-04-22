using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Classe
{
    public int Id { get; set; }

    [Display(Name = "Nom")]
    public string Nom { get; set; } = string.Empty;

    [Display(Name = "Niveau")]
    public int NiveauId { get; set; }

    public Niveau? Niveau { get; set; }

    [Display(Name = "Année académique")]
    public int AnneeAcademiqueId { get; set; }

    public AnneeAcademique? AnneeAcademique { get; set; }

    [Display(Name = "Capacité max.")]
    public int CapaciteMax { get; set; } = 30;

    [Display(Name = "Filière")]
    public int FiliereId { get; set; }

    public Filiere? Filiere { get; set; }
}
