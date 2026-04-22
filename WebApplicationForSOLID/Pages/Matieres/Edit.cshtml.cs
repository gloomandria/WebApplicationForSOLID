using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Commands;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Matieres;

public sealed class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public EditModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public Matiere Matiere { get; set; } = new();

    public SelectList EnseignantsList { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var matiere = await _mediator.Send(new GetMatiereByIdQuery(id), ct);
        if (matiere is null) return NotFound();
        Matiere = matiere;
        await LoadEnseignantsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadEnseignantsAsync(ct); return Page(); }

        var result = await _mediator.Send(new UpdateMatiereCommand(Matiere), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadEnseignantsAsync(ct);
            return Page();
        }

        TempData["Success"] = $"Matière \"{Matiere.Intitule}\" mise à jour avec succès.";
        return RedirectToPage("Index");
    }

    private async Task LoadEnseignantsAsync(CancellationToken ct)
    {
        var enseignants = await _mediator.Send(new GetAllEnseignantsQuery(), ct);
        EnseignantsList = new SelectList(enseignants, nameof(Enseignant.Id), nameof(Enseignant.NomComplet));
    }
}
