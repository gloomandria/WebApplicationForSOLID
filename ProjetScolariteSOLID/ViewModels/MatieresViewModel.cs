using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class MatieresViewModel
{
    public PagedResult<Matiere> Matieres        { get; init; } = new();
    public int                  CurrentPage     { get; init; } = 1;
    public int                  PageSize        { get; init; } = 10;
    public Matiere              Matiere         { get; init; } = new();
    public int                  MatiereId       { get; init; }
    public Matiere?             SelectedMatiere { get; init; }
    public SelectList           EnseignantsList { get; init; } = new SelectList(Enumerable.Empty<object>());
}
