using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationForSOLID.Application.CQRS.Matieres.Queries;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Pages.Matieres;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<Matiere> Matieres { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
        => Matieres = await _mediator.Send(new GetAllMatieresQuery(), ct);
}
