using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Commands;
using ProjetScolariteSOLID.Application.CQRS.Enseignants.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;

namespace ProjetScolariteSOLID.Pages.Enseignants;

public sealed class EditModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<Specialite> _specialiteRepo;
    private readonly IReferentielRepository<Grade> _gradeRepo;

    public EditModel(
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

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var enseignant = await _mediator.Send(new GetEnseignantByIdQuery(id), ct);
        if (enseignant is null) return NotFound();
        Enseignant = enseignant;
        await LoadListsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadListsAsync(ct); return Page(); }

        var result = await _mediator.Send(new UpdateEnseignantCommand(Enseignant), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadListsAsync(ct);
            return Page();
        }

        TempData["Success"] = "Enseignant mis a jour avec succes.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        SpecialitesList = new SelectList(await _specialiteRepo.GetAllAsync(ct), nameof(Specialite.Id), nameof(Specialite.Libelle));
        GradesList      = new SelectList(await _gradeRepo.GetAllAsync(ct),      nameof(Grade.Id),      nameof(Grade.Libelle));
    }
}
