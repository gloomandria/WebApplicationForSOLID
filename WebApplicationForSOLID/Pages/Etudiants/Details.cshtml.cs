using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Etudiants;

public sealed class DetailsModel : PageModel
{
    private readonly IMediator _mediator;

    public DetailsModel(IMediator mediator) => _mediator = mediator;

    public Etudiant Etudiant { get; private set; } = new();
    public BulletinEtudiant? Bulletin { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var etudiant = await _mediator.Send(new GetEtudiantByIdQuery(id), ct);
        if (etudiant is null) return NotFound();

        Etudiant = etudiant;
        Bulletin = await _mediator.Send(new GetEtudiantBulletinQuery(id), ct);
        return Page();
    }
}
