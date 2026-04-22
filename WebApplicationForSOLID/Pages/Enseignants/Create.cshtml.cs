using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;

namespace ProjetScolariteSOLID.Pages.Enseignants;

public sealed class CreateModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<Specialite> _specialiteRepo;
    private readonly IReferentielRepository<Grade> _gradeRepo;

    public CreateModel(
        IMediator mediator,
        IReferentielRepository<Specialite> specialiteRepo,
        IReferentielRepository<Grade> gradeRepo)
    {
        _mediator       = mediator;
        _specialiteRepo = specialiteRepo;
        _gradeRepo      = gradeRepo;
    }

    [BindProperty]
    public Enseignant Enseignant { get; set; } = new();

    public SelectList SpecialitesList { get; private set; } = default!;
    public SelectList GradesList      { get; private set; } = default!;

    public async Task OnGetAsync(CancellationToken ct) => await LoadListsAsync(ct);

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadListsAsync(ct); return Page(); }

        var result = await _mediator.Send(new CreateEnseignantCommand(Enseignant), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadListsAsync(ct);
            return Page();
        }

        TempData["Success"] = $"Enseignant {result.Value!.NomComplet} cree avec succes.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        SpecialitesList = new SelectList(await _specialiteRepo.GetAllAsync(ct), nameof(Specialite.Id), nameof(Specialite.Libelle));
        GradesList      = new SelectList(await _gradeRepo.GetAllAsync(ct),      nameof(Grade.Id),      nameof(Grade.Libelle));
    }
}
