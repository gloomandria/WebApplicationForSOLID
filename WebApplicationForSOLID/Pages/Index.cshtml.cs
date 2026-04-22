using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;

namespace ProjetScolariteSOLID.Pages;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public int NombreEtudiants { get; private set; }
    public int NombreEnseignants { get; private set; }
    public int NombreMatieres { get; private set; }
    public int NombreClasses { get; private set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        var etudiants   = await _mediator.Send(new GetAllEtudiantsQuery(), ct);
        var enseignants = await _mediator.Send(new GetAllEnseignantsQuery(), ct);
        var matieres    = await _mediator.Send(new GetAllMatieresQuery(), ct);
        var classes     = await _mediator.Send(new GetAllClassesQuery(), ct);

        NombreEtudiants   = etudiants.Count;
        NombreEnseignants = enseignants.Count;
        NombreMatieres    = matieres.Count;
        NombreClasses     = classes.Count;
    }
}
