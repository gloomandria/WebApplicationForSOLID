using System.ComponentModel.DataAnnotations;

namespace ProjetScolariteSOLID.Domain.Models;

public sealed class Inscription
{
    public int Id { get; set; }

    [Display(Name = "Étudiant")]
    public int EtudiantId { get; set; }

    public Etudiant? Etudiant { get; set; }

    [Display(Name = "Classe")]
    public int ClasseId { get; set; }

    public Classe? Classe { get; set; }

    [Display(Name = "Date d'inscription")]
    public DateTime DateInscription { get; init; } = DateTime.UtcNow;

    [Display(Name = "Statut")]
    public int StatutId { get; set; }

    public StatutInscriptionRef? Statut { get; set; }
}
