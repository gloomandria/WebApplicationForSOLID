using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Note
{
    public int Id { get; set; }

    [Display(Name = "Étudiant")]
    public int EtudiantId { get; set; }

    public Etudiant? Etudiant { get; set; }

    [Display(Name = "Matière")]
    public int MatiereId { get; set; }

    public Matiere? Matiere { get; set; }

    [Display(Name = "Valeur")]
    public decimal Valeur { get; set; }

    [Display(Name = "Type d'évaluation")]
    public TypeEvaluation TypeEvaluation { get; set; }

    [Display(Name = "Date")]
    [DataType(DataType.Date)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Commentaire")]
    public string Commentaire { get; set; } = string.Empty;
}

public enum TypeEvaluation
{
    [Display(Name = "Contrôle continu")] ControleContinu,
    [Display(Name = "Examen partiel")] ExamenPartiel,
    [Display(Name = "Examen final")] ExamenFinal,
    [Display(Name = "Rattrapage")] Rattrapage
}
