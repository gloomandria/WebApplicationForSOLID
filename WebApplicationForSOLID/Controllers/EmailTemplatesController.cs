using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Application.Contracts;
using ProjetScolariteSOLID.Domain.Models;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.ViewModels.Admin;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = ApplicationRole.Administrateur)]
public sealed class EmailTemplatesController : Controller
{
    private readonly IEmailTemplateService _templateService;

    public EmailTemplatesController(IEmailTemplateService templateService)
        => _templateService = templateService;

    // ── Liste ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var templates = await _templateService.GetAllAsync(ct);
        return View(new EmailTemplateListViewModel { Templates = templates });
    }

    // ── Table partielle (AJAX) ────────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> Table(CancellationToken ct)
    {
        var templates = await _templateService.GetAllAsync(ct);
        return PartialView("_EmailTemplatesTable", new EmailTemplateListViewModel { Templates = templates });
    }

    // ── Formulaire création (AJAX) ────────────────────────────────────
    [HttpGet]
    public IActionResult FormCreate()
        => PartialView("_FormEdit", new EmailTemplateFormModel());

    // ── Formulaire édition (AJAX) ─────────────────────────────────────
    [HttpGet]
    public async Task<IActionResult> FormEdit(int id, CancellationToken ct)
    {
        var template = await _templateService.GetByIdAsync(id, ct);
        if (template is null) return NotFound();
        return PartialView("_FormEdit", new EmailTemplateFormModel
        {
            Id          = template.Id,
            Code        = template.Code,
            Nom         = template.Nom,
            Sujet       = template.Sujet,
            Corps       = template.Corps,
            Description = template.Description,
            EstActif    = template.EstActif
        });
    }

    // ── Créer (AJAX) ──────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAjax(EmailTemplateFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });

        await _templateService.CreateAsync(new EmailTemplate
        {
            Code        = model.Code,
            Nom         = model.Nom,
            Sujet       = model.Sujet,
            Corps       = model.Corps,
            Description = model.Description,
            EstActif    = model.EstActif
        }, ct);

        return Json(new { success = true, message = "Template créé avec succès." });
    }

    // ── Modifier (AJAX) ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAjax(EmailTemplateFormModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });

        var existing = await _templateService.GetByIdAsync(model.Id, ct);
        if (existing is null) return Json(new { success = false, message = "Template introuvable." });

        existing.Code        = model.Code;
        existing.Nom         = model.Nom;
        existing.Sujet       = model.Sujet;
        existing.Corps       = model.Corps;
        existing.Description = model.Description;
        existing.EstActif    = model.EstActif;

        await _templateService.UpdateAsync(existing, ct);
        return Json(new { success = true, message = "Template modifié avec succès." });
    }

    // ── Supprimer (AJAX) ──────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAjax(int id, CancellationToken ct)
    {
        await _templateService.DeleteAsync(id, ct);
        return Json(new { success = true, message = "Template supprimé avec succès." });
    }
}
