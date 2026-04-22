using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class EnseignantsViewModel
{
    public PagedResult<Enseignant>  Enseignants        { get; init; } = new();
    public int                      CurrentPage        { get; init; } = 1;
    public EnseignantFormModel      Enseignant         { get; init; } = new();
    public int                      EnseignantId       { get; init; }
    public Enseignant?              SelectedEnseignant { get; init; }
    public SelectList               SpecialitesList    { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList               GradesList         { get; init; } = new SelectList(Enumerable.Empty<object>());
}

/// <summary>Modèle de formulaire pour créer/modifier un enseignant.</summary>
public sealed class EnseignantFormModel
{
    public int    Id          { get; set; }
    public string Matricule   { get; set; } = string.Empty;
    public string Nom         { get; set; } = string.Empty;
    public string Prenom      { get; set; } = string.Empty;
    public string Email       { get; set; } = string.Empty;
    public string Telephone   { get; set; } = string.Empty;
    public int    SpecialiteId { get; set; }
    public int    GradeId     { get; set; }
    public DateTime DateEmbauche { get; set; } = DateTime.UtcNow;
}
