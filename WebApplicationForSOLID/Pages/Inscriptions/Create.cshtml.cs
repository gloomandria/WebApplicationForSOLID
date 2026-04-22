using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Inscriptions;

public sealed class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public int EtudiantId { get; set; }

    [BindProperty]
    public int ClasseId { get; set; }

    public SelectList EtudiantsList { get; private set; } = default!;
    public SelectList ClassesList { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct) => await LoadListsAsync(ct);

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadListsAsync(ct); return Page(); }

        var result = await _mediator.Send(new InscrireEtudiantCommand(EtudiantId, ClasseId), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadListsAsync(ct);
            return Page();
        }

        TempData["Success"] = "Inscription créée avec succès.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        var etudiants = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var classes   = await _mediator.Send(new GetAllClassesQuery(), ct);
        EtudiantsList = new SelectList(etudiants, nameof(Etudiant.Id), nameof(Etudiant.NomComplet));
        ClassesList   = new SelectList(classes,   nameof(Classe.Id),   nameof(Classe.Nom));
    }
}
