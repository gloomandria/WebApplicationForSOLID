using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Matieres.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Matieres;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<Matiere> Matieres { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
        => Matieres = await _mediator.Send(new GetAllMatieresQuery(), ct);
}
