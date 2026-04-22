using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class EtudiantsViewModel
{
    public PagedResult<Etudiant> Etudiants        { get; init; } = new();
    public int                   CurrentPage      { get; init; } = 1;
    public Etudiant              Etudiant         { get; init; } = new();
    public int                   EtudiantId       { get; init; }
    public Etudiant?             SelectedEtudiant { get; init; }
    public BulletinEtudiant?     Bulletin         { get; init; }
}
