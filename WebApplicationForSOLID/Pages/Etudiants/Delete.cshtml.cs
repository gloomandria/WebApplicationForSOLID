using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Etudiants;

public sealed class DeleteModel : PageModel
{
    private readonly IMediator _mediator;

    public DeleteModel(IMediator mediator) => _mediator = mediator;

    public Etudiant Etudiant { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(id), ct);
        if (etudiant is null) return NotFound();
        Etudiant = etudiant;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteEtudiantCommand(id), ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Étudiant supprimé avec succès."
            : result.ErrorMessage;
        return RedirectToPage("Index");
    }
}
