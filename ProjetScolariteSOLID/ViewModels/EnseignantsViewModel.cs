using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class EnseignantsViewModel
{
    public PagedResult<Enseignant> Enseignants      { get; init; } = new();
    public int                     CurrentPage      { get; init; } = 1;
    public Enseignant              Enseignant       { get; init; } = new();
    public int                     EnseignantId     { get; init; }
    public Enseignant?             SelectedEnseignant { get; init; }
    public SelectList              SpecialitesList  { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList              GradesList       { get; init; } = new SelectList(Enumerable.Empty<object>());
}
