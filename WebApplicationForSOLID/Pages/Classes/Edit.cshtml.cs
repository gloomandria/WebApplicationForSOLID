using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetScolariteSOLID.Application.CQRS.Classes.Commands;
using ProjetScolariteSOLID.Application.CQRS.Classes.Queries;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Repositories;

namespace ProjetScolariteSOLID.Pages.Classes;

public sealed class EditModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IReferentielRepository<Filiere> _filiereRepo;
    private readonly IReferentielRepository<AnneeAcademique> _anneeRepo;
    private readonly IReferentielRepository<Niveau> _niveauRepo;

    public EditModel(
        IMediator mediator,
        IReferentielRepository<Filiere> filiereRepo,
        IReferentielRepository<AnneeAcademique> anneeRepo,
        IReferentielRepository<Niveau> niveauRepo)
    {
        _mediator    = mediator;
        _filiereRepo = filiereRepo;
        _anneeRepo   = anneeRepo;
        _niveauRepo  = niveauRepo;
    }

    [BindProperty]
    public Classe Classe { get; set; } = new();

    public SelectList FilieresList     { get; private set; } = default!;
    public SelectList AnneesAcadList   { get; private set; } = default!;
    public SelectList NiveauxList      { get; private set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var classe = await _mediator.Send(new GetClasseByIdQuery(id), ct);
        if (classe is null) return NotFound();
        Classe = classe;
        await LoadListsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) { await LoadListsAsync(ct); return Page(); }

        var result = await _mediator.Send(new UpdateClasseCommand(Classe), ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage!);
            await LoadListsAsync(ct);
            return Page();
        }

        TempData["Success"] = "Classe mise à jour avec succès.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync(CancellationToken ct)
    {
        FilieresList   = new SelectList(await _filiereRepo.GetAllAsync(ct),  nameof(Filiere.Id),         nameof(Filiere.Libelle));
        AnneesAcadList = new SelectList(await _anneeRepo.GetAllAsync(ct),    nameof(AnneeAcademique.Id), nameof(AnneeAcademique.Libelle));
        NiveauxList    = new SelectList(await _niveauRepo.GetAllAsync(ct),   nameof(Niveau.Id),          nameof(Niveau.Libelle));
    }
}
