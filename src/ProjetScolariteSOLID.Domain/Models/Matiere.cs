using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Matiere
{
    public int Id { get; set; }

    [Display(Name = "Code")]
    public string Code { get; set; } = string.Empty;

    [Display(Name = "Intitulé")]
    public string Intitule { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Coefficient")]
    public int Coefficient { get; set; } = 1;

    [Display(Name = "Volume horaire (h)")]
    public int VolumeHoraire { get; set; }

    [Display(Name = "Enseignant")]
    public int? EnseignantId { get; set; }

    public Enseignant? Enseignant { get; set; }
}
