using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class EtudiantsViewModel
{
    public PagedResult<Etudiant> Etudiants        { get; init; } = new();
    public int                   CurrentPage      { get; init; } = 1;
    public int                   PageSize         { get; init; } = 10;
    public EtudiantFormModel     Etudiant         { get; init; } = new();
    public int                   EtudiantId       { get; init; }
    public Etudiant?             SelectedEtudiant { get; init; }
    public BulletinEtudiant?     Bulletin         { get; init; }
}

/// <summary>Modèle de formulaire pour créer/modifier un étudiant.</summary>
public sealed class EtudiantFormModel
{
    public int    Id              { get; set; }
    public string NumeroEtudiant  { get; set; } = string.Empty;
    public string Nom             { get; set; } = string.Empty;
    public string Prenom          { get; set; } = string.Empty;
    public string Email           { get; set; } = string.Empty;
    public string Telephone       { get; set; } = string.Empty;
    public string Adresse         { get; set; } = string.Empty;
    public DateOnly DateNaissance { get; set; }
    public DateTime DateInscription { get; set; } = DateTime.UtcNow;
}
