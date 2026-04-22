using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Commands;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Inscriptions;

public sealed class DeleteModel : PageModel
{
    private readonly IMediator _mediator;

    public DeleteModel(IMediator mediator) => _mediator = mediator;

    public Inscription Inscription { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var inscription = await _mediator.Send(new GetInscriptionByIdQuery(id), ct);
        if (inscription is null) return NotFound();
        Inscription = inscription;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new SupprimerInscriptionCommand(id), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Inscription supprimée avec succès."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }
}
