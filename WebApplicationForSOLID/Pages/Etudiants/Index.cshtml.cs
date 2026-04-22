using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Etudiants.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Etudiants;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private const int PageSize = 8;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public PagedResult<Etudiant> Etudiants { get; private set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync(CancellationToken ct)
        => Etudiants = await _mediator.Send(new GetEtudiantsQuery(CurrentPage, PageSize), ct);
}
