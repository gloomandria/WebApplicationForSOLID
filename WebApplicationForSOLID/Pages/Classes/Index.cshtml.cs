using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Pages.Classes;

public sealed class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<Classe> Classes { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
        => Classes = await _mediator.Send(new GetAllClassesQuery(), ct);
}
