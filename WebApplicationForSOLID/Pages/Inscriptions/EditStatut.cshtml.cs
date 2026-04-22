using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Inscriptions;

public sealed class EditStatutModel : PageModel
{
    private readonly IMediator _mediator;

    public EditStatutModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public int InscriptionId { get; set; }

    [BindProperty]
    public StatutInscription Statut { get; set; }

    public Inscription Inscription { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var inscription = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        if (inscription is null) return NotFound();

        Inscription   = inscription;
        InscriptionId = inscription.Id;
        Statut        = inscription.Statut;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var result = await _mediator.Send(new ModifierStatutInscriptionCommand(InscriptionId, Statut), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Statut mis à jour avec succès."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }
}
