using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetScolariteSOLID.Domain.Models.Auth;
using ProjetScolariteSOLID.Domain.Repositories;
using ProjetScolariteSOLID.ViewModels.Admin;

namespace ProjetScolariteSOLID.Controllers;

[Authorize(Roles = ApplicationRole.Administrateur)]
public sealed class AuditController : Controller
{
    private readonly IAuditLogRepository _auditRepo;
    private const int PageSize = 50;

    public AuditController(IAuditLogRepository auditRepo) => _auditRepo = auditRepo;

    // ── Liste paginée avec filtres ────────────────────────────────────────────
    public async Task<IActionResult> Index(
        string? table   = null,
        string? userId  = null,
        int     page    = 1,
        CancellationToken ct = default)
    {
        var logs = string.IsNullOrWhiteSpace(table) && string.IsNullOrWhiteSpace(userId)
            ? await _auditRepo.GetRecentAsync(PageSize * page, ct)
            : !string.IsNullOrWhiteSpace(table)
                ? await _auditRepo.GetByTableAsync(table, ct)
                : await _auditRepo.GetByUserAsync(userId!, ct);

        // Tables distinctes pour le filtre
        var allRecent = await _auditRepo.GetRecentAsync(500, ct);
        var tables    = allRecent.Select(l => l.TableName).Distinct().OrderBy(t => t).ToList();

        var paged = logs
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        return View(new AuditListViewModel
        {
            Logs        = paged,
            TableFilter = table,
            UserFilter  = userId,
            Page        = page,
            PageSize    = PageSize,
            Total       = logs.Count,
            Tables      = tables
        });
    }

    // ── Détail d'un log ───────────────────────────────────────────────────────
    public async Task<IActionResult> Details(long id, CancellationToken ct)
    {
        var recent = await _auditRepo.GetRecentAsync(1000, ct);
        var log    = recent.FirstOrDefault(l => l.Id == id);
        if (log is null) return NotFound();
        return View(new AuditDetailViewModel { Log = log });
    }
}
