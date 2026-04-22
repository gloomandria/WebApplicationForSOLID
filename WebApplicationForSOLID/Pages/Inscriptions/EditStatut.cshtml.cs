using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;

namespace ProjetScolariteSOLID.Pages.Inscriptions;

public sealed class EditStatutModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<StatutInscriptionRef> _statutRepo;

    public EditStatutModel(
        IMediator mediator,
        IReferentielRepository<StatutInscriptionRef> statutRepo)
    {
        _mediator   = mediator;
        _statutRepo = statutRepo;
    }

    [BindProperty]
    public int InscriptionId { get; set; }

    [BindProperty]
    public int StatutId { get; set; }

    public Inscription Inscription { get; private set; } = new();

    public SelectList StatutsList { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var inscription = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        if (inscription is null) return NotFound();

        Inscription   = inscription;
        InscriptionId = inscription.Id;
        StatutId      = inscription.StatutId;
        await LoadListsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new ModifierStatutInscriptionCommand(InscriptionId, StatutId), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Statut mis à jour avec succès."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        StatutsList = new SelectList(await _statutRepo.GetAllAsync(ct), nameof(StatutInscriptionRef.Id), nameof(StatutInscriptionRef.Libelle));
    }
}
