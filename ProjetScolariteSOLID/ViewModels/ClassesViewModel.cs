using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class ClassesViewModel
{
    public IReadOnlyList<Classe> Classes        { get; init; } = [];
    public Classe                Classe         { get; init; } = new();
    public int                   ClasseId       { get; init; }
    public Classe?               SelectedClasse { get; init; }
    public SelectList            FilieresList   { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList            AnneesAcadList { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList            NiveauxList    { get; init; } = new SelectList(Enumerable.Empty<object>());
}
