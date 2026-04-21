using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplicationForSOLID.Application.CQRS.Enseignants.Queries;
using WebApplicationForSOLID.Application.CQRS.Matieres.Commands;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Pages.Matieres;

public sealed class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public Matiere Matiere { get; set; } = new();

    public SelectList EnseignantsList { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct) => await LoadEnseignantsAsync(ct);

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadEnseignantsAsync(ct); return Page(); }

        var result = await _mediator.Send(new CreateMatiereCommand(Matiere), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadEnseignantsAsync(ct);
            return Page();
        }

        TempData["Success"] = $"Matiere \"{result.Value!.Intitule}\" creee avec succes.";
        return RedirectToPage("Index");
    }

    private async Task LoadEnseignantsAsync(CancellationToken ct)
    {
        var enseignants = await _mediator.Send(new GetAllEnseignantsQuery(), ct);
        EnseignantsList = new SelectList(enseignants, nameof(Enseignant.Id), nameof(Enseignant.NomComplet));
    }
}