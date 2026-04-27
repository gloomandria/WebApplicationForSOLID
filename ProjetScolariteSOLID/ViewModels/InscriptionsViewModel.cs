using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.ViewModels;

public sealed class InscriptionsViewModel
{
    public PagedResult<Inscription> Inscriptions        { get; init; } = new();
    public int                      CurrentPage         { get; init; } = 1;
    public int                      PageSize            { get; init; } = 10;
    public int                      EtudiantId          { get; init; }
    public int                      ClasseId            { get; init; }
    public int                      InscriptionId       { get; init; }
    public int                      StatutId            { get; init; }
    public Inscription?             SelectedInscription { get; init; }
    public SelectList               EtudiantsList       { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList               ClassesList         { get; init; } = new SelectList(Enumerable.Empty<object>());
    public SelectList               StatutsList         { get; init; } = new SelectList(Enumerable.Empty<object>());
}
