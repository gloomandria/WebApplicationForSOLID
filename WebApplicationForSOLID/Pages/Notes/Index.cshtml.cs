using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Notes.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Notes;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private const int PageSize = 10;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public PagedResult<Note> Notes { get; private set; } = new();

    [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public async Task OnGetAsync(CancellationToken ct)
        => Notes = await _mediator.Send(new GetNotesQuery(CurrentPage, PageSize), ct);
}
