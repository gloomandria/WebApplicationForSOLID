using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Inscriptions.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Inscriptions;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private const int PageSize = 10;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public PagedResult<Inscription> Inscriptions { get; private set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync(CancellationToken ct)
        => Inscriptions = await _mediator.Send(new GetInscriptionsQuery(CurrentPage, PageSize), ct);
}
