using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Enseignants;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private const int PageSize = 8;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public PagedResult<Enseignant> Enseignants { get; private set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync(CancellationToken ct)
        => Enseignants = await _mediator.Send(new GetEnseignantsQuery(CurrentPage, PageSize), ct);
}
