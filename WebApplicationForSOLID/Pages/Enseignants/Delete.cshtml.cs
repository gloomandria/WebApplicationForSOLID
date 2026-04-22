using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Enseignants;

public sealed class DeleteModel : PageModel
{
    private readonly IMediator _mediator;

    public DeleteModel(IMediator mediator) => _mediator = mediator;

    public Enseignant Enseignant { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(id), ct);
        if (enseignant is null) return NotFound();
        Enseignant = enseignant;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteEnseignantCommand(id), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Enseignant supprime avec succes."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }
}