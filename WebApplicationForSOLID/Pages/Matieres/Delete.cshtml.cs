using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Commands;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Matieres;

public sealed class DeleteModel : PageModel
{
    private readonly IMediator _mediator;

    public DeleteModel(IMediator mediator) => _mediator = mediator;

    public Matiere Matiere { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var matiere = await _mediator.Send(new GetMatiereByIdQuery(id), ct);
        if (matiere is null) return NotFound();
        Matiere = matiere;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteMatiereCommand(id), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Matière supprimée avec succès."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }
}
