using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationForSOLID.Application.CQRS.Etudiants.Commands;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Pages.Etudiants;

public sealed class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public Etudiant Etudiant { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        var result = await _mediator.Send(new CreateEtudiantCommand(Etudiant), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return Page();
        }

        TempData["Success"] = $"Étudiant {result.Value!.NomComplet} ({result.Value.NumeroEtudiant}) créé avec succès.";
        return RedirectToPage("Index");
    }
}
