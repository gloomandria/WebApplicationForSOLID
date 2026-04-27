using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels;

namespace ProjetScolariteSOLID.Controllers;

/// <summary>
/// Contrôleur unique gérant le CRUD de tous les référentiels simples (Id + Libelle).
/// Le type est sélectionné via le paramètre de route "type".
/// </summary>
[Authorize(Roles = ApplicationRole.Administrateur)]
public sealed class ReferentielsController : Controller
{
    private readonly IReferentielRepository<AnneeAcademique>      _anneeRepo;
    private readonly IReferentielRepository<Filiere>              _filiereRepo;
    private readonly IReferentielRepository<Grade>                _gradeRepo;
    private readonly IReferentielRepository<Niveau>               _niveauRepo;
    private readonly IReferentielRepository<Specialite>           _specialiteRepo;
    private readonly IReferentielRepository<StatutInscriptionRef> _statutRepo;
    private readonly IReferentielRepository<TypeEvaluationRef>    _typeEvalRepo;

    public ReferentielsController(
        IReferentielRepository<AnneeAcademique>      anneeRepo,
        IReferentielRepository<Filiere>              filiereRepo,
        IReferentielRepository<Grade>                gradeRepo,
        IReferentielRepository<Niveau>               niveauRepo,
        IReferentielRepository<Specialite>           specialiteRepo,
        IReferentielRepository<StatutInscriptionRef> statutRepo,
        IReferentielRepository<TypeEvaluationRef>    typeEvalRepo)
    {
        _anneeRepo      = anneeRepo;
        _filiereRepo    = filiereRepo;
        _gradeRepo      = gradeRepo;
        _niveauRepo     = niveauRepo;
        _specialiteRepo = specialiteRepo;
        _statutRepo     = statutRepo;
        _typeEvalRepo   = typeEvalRepo;
    }

    // ── Résolution du type ────────────────────────────────────────────────────

    private static readonly Dictionary<string, (string Nom, string Icone)> _meta = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AnneesAcademiques"]  = ("Années académiques",    "📅"),
        ["Filieres"]           = ("Filières",              "🎓"),
        ["Grades"]             = ("Grades",                "🏅"),
        ["Niveaux"]            = ("Niveaux",               "📊"),
        ["Specialites"]        = ("Spécialités",           "🔬"),
        ["StatutsInscription"] = ("Statuts d'inscription", "📌"),
        ["TypesEvaluation"]    = ("Types d'évaluation",    "📝"),
    };

    private async Task<IReadOnlyList<ReferentielItemDto>> GetAllAsync(string type, CancellationToken ct)
        => type.ToLowerInvariant() switch
        {
            "anneesacademiques"  => Map(await _anneeRepo.GetAllAsync(ct),      x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "filieres"           => Map(await _filiereRepo.GetAllAsync(ct),    x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "grades"             => Map(await _gradeRepo.GetAllAsync(ct),      x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "niveaux"            => Map(await _niveauRepo.GetAllAsync(ct),     x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "specialites"        => Map(await _specialiteRepo.GetAllAsync(ct), x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "statutsinscription" => Map(await _statutRepo.GetAllAsync(ct),     x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            "typesevaluation"    => Map(await _typeEvalRepo.GetAllAsync(ct),   x => new ReferentielItemDto { Id = x.Id, Libelle = x.Libelle }),
            _                    => []
        };

    private async Task<ReferentielItemDto?> GetByIdAsync(string type, int id, CancellationToken ct)
        => type.ToLowerInvariant() switch
        {
            "anneesacademiques"  => Map(await _anneeRepo.GetByIdAsync(id, ct)),
            "filieres"           => Map(await _filiereRepo.GetByIdAsync(id, ct)),
            "grades"             => Map(await _gradeRepo.GetByIdAsync(id, ct)),
            "niveaux"            => Map(await _niveauRepo.GetByIdAsync(id, ct)),
            "specialites"        => Map(await _specialiteRepo.GetByIdAsync(id, ct)),
            "statutsinscription" => Map(await _statutRepo.GetByIdAsync(id, ct)),
            "typesevaluation"    => Map(await _typeEvalRepo.GetByIdAsync(id, ct)),
            _                    => null
        };

    private async Task CreateAsync(string type, string libelle, CancellationToken ct)
    {
        switch (type.ToLowerInvariant())
        {
            case "anneesacademiques":  await _anneeRepo.CreateAsync(new AnneeAcademique { Libelle = libelle }, ct); break;
            case "filieres":           await _filiereRepo.CreateAsync(new Filiere { Libelle = libelle }, ct); break;
            case "grades":             await _gradeRepo.CreateAsync(new Grade { Libelle = libelle }, ct); break;
            case "niveaux":            await _niveauRepo.CreateAsync(new Niveau { Libelle = libelle }, ct); break;
            case "specialites":        await _specialiteRepo.CreateAsync(new Specialite { Libelle = libelle }, ct); break;
            case "statutsinscription": await _statutRepo.CreateAsync(new StatutInscriptionRef { Libelle = libelle }, ct); break;
            case "typesevaluation":    await _typeEvalRepo.CreateAsync(new TypeEvaluationRef { Libelle = libelle }, ct); break;
        }
    }

    private async Task UpdateAsync(string type, int id, string libelle, CancellationToken ct)
    {
        switch (type.ToLowerInvariant())
        {
            case "anneesacademiques":  await _anneeRepo.UpdateAsync(new AnneeAcademique { Id = id, Libelle = libelle }, ct); break;
            case "filieres":           await _filiereRepo.UpdateAsync(new Filiere { Id = id, Libelle = libelle }, ct); break;
            case "grades":             await _gradeRepo.UpdateAsync(new Grade { Id = id, Libelle = libelle }, ct); break;
            case "niveaux":            await _niveauRepo.UpdateAsync(new Niveau { Id = id, Libelle = libelle }, ct); break;
            case "specialites":        await _specialiteRepo.UpdateAsync(new Specialite { Id = id, Libelle = libelle }, ct); break;
            case "statutsinscription": await _statutRepo.UpdateAsync(new StatutInscriptionRef { Id = id, Libelle = libelle }, ct); break;
            case "typesevaluation":    await _typeEvalRepo.UpdateAsync(new TypeEvaluationRef { Id = id, Libelle = libelle }, ct); break;
        }
    }

    private async Task DeleteAsync(string type, int id, CancellationToken ct)
    {
        switch (type.ToLowerInvariant())
        {
            case "anneesacademiques":  await _anneeRepo.DeleteAsync(id, ct); break;
            case "filieres":           await _filiereRepo.DeleteAsync(id, ct); break;
            case "grades":             await _gradeRepo.DeleteAsync(id, ct); break;
            case "niveaux":            await _niveauRepo.DeleteAsync(id, ct); break;
            case "specialites":        await _specialiteRepo.DeleteAsync(id, ct); break;
            case "statutsinscription": await _statutRepo.DeleteAsync(id, ct); break;
            case "typesevaluation":    await _typeEvalRepo.DeleteAsync(id, ct); break;
        }
    }

    // ── Helpers de mapping ────────────────────────────────────────────────────

    private static IReadOnlyList<ReferentielItemDto> Map<T>(IReadOnlyList<T> list, Func<T, ReferentielItemDto> selector)
        => list.Select(selector).ToList();

    private static ReferentielItemDto? Map<T>(T? entity) where T : class
    {
        if (entity is null) return null;
        var id  = (int)entity.GetType().GetProperty("Id")!.GetValue(entity)!;
        var lib = (string)(entity.GetType().GetProperty("Libelle")!.GetValue(entity) ?? string.Empty);
        return new ReferentielItemDto { Id = id, Libelle = lib };
    }

    private (string Nom, string Icone) GetMeta(string type)
        => _meta.TryGetValue(type, out var m) ? m : (type, "📋");

    private ReferentielViewModel BuildVm(string type, IReadOnlyList<ReferentielItemDto> items,
        ReferentielItemDto? item = null, ReferentielItemDto? selected = null, int itemId = 0)
    {
        var (nom, icone) = GetMeta(type);
        return new ReferentielViewModel
        {
            NomReferentiel = nom,
            Icone          = icone,
            Items          = items,
            Item           = item ?? new(),
            SelectedItem   = selected,
            ItemId         = itemId
        };
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    public async Task<IActionResult> Index(string type, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return NotFound();
        var items = await GetAllAsync(type, ct);
        ViewData["Type"] = type;
        return View(BuildVm(type, items));
    }

    [HttpGet]
    public async Task<IActionResult> Table(string type, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return NotFound();
        var items = await GetAllAsync(type, ct);
        ViewData["Type"] = type;
        return PartialView("_Table", BuildVm(type, items));
    }

    [HttpGet]
    public IActionResult FormCreate(string type)
    {
        if (!_meta.ContainsKey(type)) return NotFound();
        var (nom, icone) = GetMeta(type);
        ViewData["Type"] = type;
        return PartialView("_FormCreate", new ReferentielViewModel { NomReferentiel = nom, Icone = icone, Items = [] });
    }

    [HttpGet]
    public async Task<IActionResult> FormEdit(string type, int id, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return NotFound();
        var item = await GetByIdAsync(type, id, ct);
        ViewData["Type"] = type;
        return PartialView("_FormEdit", BuildVm(type, [], item, item));
    }

    [HttpGet]
    public async Task<IActionResult> FormDelete(string type, int id, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return NotFound();
        var item = await GetByIdAsync(type, id, ct);
        ViewData["Type"] = type;
        return PartialView("_FormDelete", BuildVm(type, [], null, item, item?.Id ?? 0));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax([FromForm] string type, [FromForm] string libelle, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return Json(new { success = false, message = "Type inconnu." });
        if (string.IsNullOrWhiteSpace(libelle)) return Json(new { success = false, message = "Le libellé est obligatoire." });
        await CreateAsync(type, libelle.Trim(), ct);
        return Json(new { success = true, message = $"Élément \"{libelle.Trim()}\" créé avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax([FromForm] string type, [FromForm] int id, [FromForm] string libelle, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return Json(new { success = false, message = "Type inconnu." });
        if (string.IsNullOrWhiteSpace(libelle)) return Json(new { success = false, message = "Le libellé est obligatoire." });
        await UpdateAsync(type, id, libelle.Trim(), ct);
        return Json(new { success = true, message = "Élément mis à jour avec succès." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax([FromForm] string type, [FromForm] int itemId, CancellationToken ct)
    {
        if (!_meta.ContainsKey(type)) return Json(new { success = false, message = "Type inconnu." });
        await DeleteAsync(type, itemId, ct);
        return Json(new { success = true, message = "Élément supprimé avec succès." });
    }
}
