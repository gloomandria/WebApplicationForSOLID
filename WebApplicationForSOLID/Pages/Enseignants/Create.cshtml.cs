using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplicationForSOLID.Application.CQRS.Enseignants.Commands;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Pages.Enseignants;

public sealed class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public Enseignant Enseignant { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        var result = await _mediator.Send(new CreateEnseignantCommand(Enseignant), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            return Page();
        }

        TempData["Success"] = $"Enseignant {result.Value!.NomComplet} cree avec succes.";
        return RedirectToPage("Index");
    }
}