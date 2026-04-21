using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationForSOLID.Application.CQRS.Classes.Queries;
using WebApplicationForSOLID.Application.CQRS.Enseignants.Queries;
using WebApplicationForSOLID.Application.CQRS.Etudiants.Queries;
using WebApplicationForSOLID.Application.CQRS.Matieres.Queries;

namespace WebApplicationForSOLID.Pages;

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
