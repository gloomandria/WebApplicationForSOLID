using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class MatieresViewModel
{
    public IReadOnlyList<Matiere> Matieres        { get; init; } = [];
    public Matiere                Matiere         { get; init; } = new();
    public int                    MatiereId       { get; init; }
    public Matiere?               SelectedMatiere { get; init; }
    public SelectList             EnseignantsList { get; init; } = new SelectList(Enumerable.Empty<object>());
}
